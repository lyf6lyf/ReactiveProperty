using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Threading;

namespace ReactiveProperty
{
    internal sealed class ReadOnlyProperty<T> : IReadOnlyProperty<T>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private T _value;
        private readonly IObservable<T> _source;
        private volatile int _initialized;

        public ReadOnlyProperty(T initialValue)
        {
            Value = initialValue;
            _source = Observable.Return(initialValue);
            _initialized = 1;
        }

        public ReadOnlyProperty(IObservable<T> source, bool deferSubscription)
        {
            _source = source;
            if (!deferSubscription)
            {
                SubscribeToSource();
                _initialized = 1;
            }
        }

        public T Value
        {
            get
            {

                if (Interlocked.CompareExchange(ref _initialized, 1, 0) == 0)
                {
                    SubscribeToSource();
                }

                return _value;
            }
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

        private void SubscribeToSource()
        {
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
                    // disposable won't be null because "this"" won't be GCed before SubscribeTo returns.
                    disposable.Dispose();
                }
            });
        }
    }
}
