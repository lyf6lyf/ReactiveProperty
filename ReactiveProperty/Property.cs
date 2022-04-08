using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Subjects;

namespace ReactiveProperty
{
    internal class Property<T> : IProperty<T>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private T _value;
        private readonly ISubject<T> _subject = new ReplaySubject<T>(1);

        public Property(T value = default)
        {
            Value = value;
            _subject.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _subject.Subscribe(observer);
        }

        public T Value
        {
            get => _value;
            set
            {
                if (!EqualityComparer<T>.Default.Equals(_value, value))
                {
                    _value = value;
                    PropertyChanged?.Invoke(this, Constants.ValuePropertyChangedEventArgs);
                    _subject.OnNext(_value);
                }
            }
        }
    }
}