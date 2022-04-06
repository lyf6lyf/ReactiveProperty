using System;
using System.Threading.Tasks;

namespace ReactiveProperty
{
    public static class PropertyEx
    {
        public static IReadOnlyProperty<T> ToProperty<T>(this IObservable<T> source)
        {
            return new ReadOnlyProperty<T>(source);
        }

        public static IProperty<T> CreateProperty<T>(T initialValue=default)
        {
            return new Property<T>(initialValue);
        }

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
