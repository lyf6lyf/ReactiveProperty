using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ReactiveProperty
{
    public static class PropertyEx
    {
        /// <summary>
        /// Convert an <see cref="IObservable{T}"/> to an <see cref="IReadOnlyProperty{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="subscribeWhenNeeded">
        /// If it is true, the returned <see cref="IReadOnlyProperty{T}"/> will subscribe to <paramref name="source"/>
        /// when the first event handler is added to <see cref="IReadOnlyProperty{T}.PropertyChanged"/>,
        /// and dispose the subscription when all the event handler is removed.
        /// </param>
        /// <returns>The <see cref="IReadOnlyProperty{T}"/>.</returns>
        public static IReadOnlyProperty<T> ToProperty<T>(this IObservable<T> source, bool subscribeWhenNeeded = false)
            => subscribeWhenNeeded ? new LazyReadOnlyProperty<T>(source) : (IReadOnlyProperty<T>)new ReadOnlyProperty<T>(source);

        /// <summary>
        /// Create an <see cref="IProperty{T}"/> with initial value.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="initialValue">The initial value, optional.</param>
        /// <returns>The <see cref="IProgress{T}"/>.</returns>
        public static IProperty<T> CreateProperty<T>(T initialValue = default) => new Property<T>(initialValue);

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
        /// <returns>The command.</returns>
        public static IObservableCommand<TInput, TOutput> CreateCommand<TInput, TOutput>(
            Func<TInput, Task<TOutput>> execute,
            IReadOnlyProperty<bool> isEnabled,
            Predicate<TInput> canExecuteOnInput = null,
            TOutput initialValue = default) =>
            new ObservableCommand<TInput, TOutput>(execute, isEnabled, canExecuteOnInput, initialValue);

        /// <summary>
        /// Generate an <see cref="IReadOnlyProperty{TOut}"/> which will be updated every time the
        /// property <paramref name="propertyName"/> changes by calling <paramref name="transform"/>.
        /// This callback takes your base object and transform it as you want into a <typeparamref name="TOut"/>.
        /// </summary>
        /// <typeparam name="TIn">The input type of the callback. **It must correspond to the actual type of src otherwise
        /// it will fail**.</typeparam>
        /// <typeparam name="TOut">The output type of the observable.</typeparam>
        /// <param name="src">Your object to observe.</param>
        /// <param name="propertyName">The name of the property you want to track.</param>
        /// <param name="transform">The function to call each time the property changes.</param>
        /// <returns>An observable on the property <paramref name="propertyName"/> of <paramref name="src"/>.</returns>
        public static IReadOnlyProperty<TOut> ToProperty<TIn, TOut>(
            this TIn src,
            string propertyName,
            Func<TIn, TOut> transform)
            where TIn : INotifyPropertyChanged
        {
            var result = CreateProperty(transform(src));

            var weakResult = new WeakReference<IProperty<TOut>>(result);
            void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
            {
                if (weakResult.TryGetTarget(out var strongResult))
                {
                    if (e.PropertyName == propertyName)
                    {
                        strongResult.Value = transform(src);
                    }
                }
                else
                {
                    src.PropertyChanged -= PropertyChangedHandler;
                }
            }

            src.PropertyChanged += PropertyChangedHandler;

            return result;
        }
    }
}
