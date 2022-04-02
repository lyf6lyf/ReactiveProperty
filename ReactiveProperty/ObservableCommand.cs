using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using static ReactiveProperty.PropertyEx;

namespace ReactiveProperty
{
    internal class ObservableCommand<TInput, TOutput> : Property<TOutput>, IObservableCommand<TInput, TOutput>
    {
        public event EventHandler CanExecuteChanged;

        private readonly IProperty<bool> _isExecuting;
        private readonly Predicate<TInput> _canExecuteOnInput;
        private readonly Func<TInput, Task<TOutput>> _execute;
        private Task<TOutput> _previousExecutionTask;

        public ObservableCommand(
            Func<TInput, Task<TOutput>> execute,
            IReadOnlyProperty<bool> isEnabled,
            Predicate<TInput> canExecuteOnInput = null,
            TOutput initialValue = default)
            : base(initialValue)
        {
            _execute = execute;
            _canExecuteOnInput = canExecuteOnInput;

            IsEnabled = isEnabled;
            _isExecuting = CreateProperty(false);
            CanExecute = IsEnabled.CombineLatest(
                    _isExecuting,
                    (isEnabledValue, isExecutingValue) => isEnabledValue && !isExecutingValue)
                .ToProperty();

            _ = CanExecute.DistinctUntilChanged().Subscribe(x => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
            _previousExecutionTask = Task.FromResult(default(TOutput));
        }

        public IReadOnlyProperty<bool> IsEnabled { get; }

        public IReadOnlyProperty<bool> IsExecuting => _isExecuting;

        public IReadOnlyProperty<bool> CanExecute { get; }

        public Task<TOutput> ExecuteAsync(TInput input)
        {
            var canProcessValue = CanExecute.Value && _canExecuteOnInput?.Invoke(input) != false;
            if (!canProcessValue)
            {
                return Task.FromResult(Value);
            }

            return _previousExecutionTask = ChainOperations();

            async Task<TOutput> ChainOperations()
            {
                try
                {
                    _isExecuting.Value = true;
                    await _previousExecutionTask;
                    Value = await _execute(input);
                }
                finally
                {
                    _isExecuting.Value = false;
                }

                return Value;
            }
        }

        async void ICommand.Execute(object parameter)
        {
            var input = Cast<TInput>(parameter);
            await ExecuteAsync(input);
        }

        bool ICommand.CanExecute(object parameter)
        {
            var input = Cast<TInput>(parameter);
            return CanExecute.Value && _canExecuteOnInput?.Invoke(input) != false;
        }

        public static T Cast<T>(object parameter)
        {
            // Ensure that null is coerced to default(T) so that value types will be a sensible default.
            if (parameter == null)
            {
                parameter = default(T);
            }

            return (T)parameter;
        }
    }
}
