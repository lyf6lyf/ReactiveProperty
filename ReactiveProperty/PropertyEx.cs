using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;

namespace ReactiveProperty
{
    public static class PropertyEx
    {
        public static IReadOnlyProperty<T> ToProperty<T>(this IObservable<T> source)
        {
            var property = new Property<T>();
            var weakProperty = new WeakReference<Property<T>>(property);
            var disposables = new CompositeDisposable();
            disposables.Add(source.Subscribe(x =>
            {
                if (weakProperty.TryGetTarget(out var target))
                {
                    target.Value = x;
                }
                else
                {
                    disposables.Dispose();
                }
            }));

            return property;
        }

        public static IProperty<T> CreateProperty<T>(T initialValue=default)
        {
            return new Property<T>(initialValue);
        }
    }
}
