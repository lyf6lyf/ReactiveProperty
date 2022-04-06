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

        public ReadOnlyProperty(IObservable<T> source, bool deferSubscribe = false)
        {
            _source = source;
            if (!deferSubscribe)
            {
                SubscribeTo(_source);
            }
            else
            {
                //TODO
            }
        }

        public T Value
        {
            get => _value;
            private set
            {
                if (!EqualityComparer<T>.Default.Equals(_value, value))
                {
                    _value = value;
                    PropertyChanged?.Invoke(this, Constants.ValuePropertyChangedEventArgs);
                }
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _source.Subscribe(observer);
        }

        private void SubscribeTo(IObservable<T> source)
        {
            var weakProperty = new WeakReference<ReadOnlyProperty<T>>(this);
            IDisposable disposable = null;
            disposable = source.Subscribe(x =>
            {
                if (weakProperty.TryGetTarget(out var target))
                {
                    target.Value = x;
                }
                else
                {
                    // disposable won't be null because "this"" won't be GCed before SubscribeTo returns.
                    disposable.Dispose();
                }
            });
        }
    }
}
