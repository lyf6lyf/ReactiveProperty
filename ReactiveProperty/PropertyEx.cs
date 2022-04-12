using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReactiveProperty
{
    public static class PropertyEx
    {
        /// <summary>
        /// Convert an <see cref="IObservable{T}"/> to an <see cref="IReadOnlyProperty{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="deferSubscription">Set it true if we want to subscribe to <paramref name="source"/> when the first call to <see cref="IReadOnlyProperty{T}.Value"/></param>
        /// <returns></returns>
        public static IReadOnlyProperty<T> ToProperty<T>(this IObservable<T> source, bool deferSubscription = false)
        {
            return new ReadOnlyProperty<T>(source, deferSubscription);
        }

        /// <summary>
        /// Create an <see cref="IProperty{T}"/> with initial value.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="initialValue">The initial value, optional.</param>
        /// <returns></returns>
        public static IProperty<T> CreateProperty<T>(T initialValue=default)
        {
            return new Property<T>(initialValue);
        }

        /// <summary>
        /// Create a <see cref="IObservableCommand{TInput, TOutput}"/>.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
        /// <typeparam name="TOutput">The output type.</typeparam>
        /// <param name="execute">The function to execute when the command is executed.</param>
        /// <param name="isEnabled">The observable controlling the command execution state.</param>
        /// <param name="canExecuteOnInput">
        /// The function to evaluate if the command can be executed with the received <typeparamref name="TInput"/> value.
        /// The result of this function will be combined with <paramref name="isEnabled"/> to determine if the command can be executed or not.
        /// It can be null if not used/needed.
        /// </param>
        /// <param name="initialValue">The initial value.</param>
        /// <returns></returns>
        public static IObservableCommand<TInput, TOutput> CreateCommand<TInput, TOutput>(
            Func<TInput, Task<TOutput>> execute,
            IReadOnlyProperty<bool> isEnabled,
            Predicate<TInput> canExecuteOnInput = null,
            TOutput initialValue = default)
        {
            return new ObservableCommand<TInput, TOutput>(execute, isEnabled, canExecuteOnInput, initialValue);
        }
    }
}
