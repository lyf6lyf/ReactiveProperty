using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace ReactiveProperty
{
    internal sealed class LazyReadOnlyProperty<T> : IReadOnlyProperty<T>
    {
        private event PropertyChangedEventHandler InnerPropertyChanged;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                if (Interlocked.Increment(ref _eventHandlerCount) == 1)
                {
                    var disposable = Interlocked.Exchange(
                        ref _disposable, SubscribeToSource());
                    disposable?.Dispose();

                }

                InnerPropertyChanged += value;
            }
            remove
            {
                InnerPropertyChanged -= value;
                if (Interlocked.Decrement(ref _eventHandlerCount) == 0)
                {
                    var disposable = Interlocked.Exchange(ref _disposable, null);
                    disposable?.Dispose();
                }
            }
        }

        private T _value;
        private readonly IObservable<T> _source;
        private IDisposable _disposable;
        private int _eventHandlerCount;

        public LazyReadOnlyProperty(IObservable<T> source) => _source = source;

        public T Value
        {
            get => _value;
            private set
            {
                if (!EqualityComparer<T>.Default.Equals(_value, value))
                {
                    _value = value;
                    InnerPropertyChanged?.Invoke(this, InternalConstants.ValuePropertyChangedEventArgs);
                }
            }
        }

        public IDisposable Subscribe(IObserver<T> observer) => _source.Subscribe(observer);

        private IDisposable SubscribeToSource()
        {
            var weakProperty = new WeakReference<LazyReadOnlyProperty<T>>(this);
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
                    // So this.SubscribeToSource is completed and disposable can't be null.
                    //
                    // It is possible that the disposable has already be disposed by PropertyChanged.remove.
                    // According to https://docs.microsoft.com/en-us/dotnet/api/system.idisposable.dispose?view=netstandard-2.0,
                    // "If an object's Dispose method is called more than once, the object must ignore all calls after the first one.
                    // The object must not throw an exception if its Dispose method is called multiple times.",
                    // so it is safe to call dispose here.
                    disposable.Dispose();
                }
            });
            return disposable;
        }
    }
}
