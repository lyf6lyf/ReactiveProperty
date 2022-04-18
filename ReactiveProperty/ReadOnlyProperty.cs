using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;

namespace ReactiveProperty
{
    internal sealed class ReadOnlyProperty<T> : IReadOnlyProperty<T>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private T _value;
        private readonly IObservable<T> _source;

        public ReadOnlyProperty(T initialValue)
        {
            Value = initialValue;
            _source = Observable.Return(initialValue);
        }

        public ReadOnlyProperty(IObservable<T> source)
        {
            _source = source;

            var weakProperty = new WeakReference<ReadOnlyProperty<T>>(this);
            IDisposable disposable = null;
            disposable = _source.Subscribe(x =>
            {
                if (weakProperty.TryGetTarget(out var target))
                {
                    target.Value = x;
                }
                else
                {
                    // The "else" branch is executed after "this" has been GCed.
                    // So the constructor of "this" is completed and disposable can't be null.
                    disposable.Dispose();
                }
            });
        }

        public T Value
        {
            get => _value;
            private set
            {
                if (!EqualityComparer<T>.Default.Equals(_value, value))
                {
                    _value = value;
                    PropertyChanged?.Invoke(this, InternalConstants.ValuePropertyChangedEventArgs);
                }
            }
        }

        public IDisposable Subscribe(IObserver<T> observer) => _source.Subscribe(observer);
    }
}
