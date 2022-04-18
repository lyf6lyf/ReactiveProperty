using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Subjects;

namespace ReactiveProperty
{
    internal class Property<T> : IProperty<T>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly BehaviorSubject<T> _subject;

        public Property(T value = default) => _subject = new BehaviorSubject<T>(value);

        public IDisposable Subscribe(IObserver<T> observer) => _subject.Subscribe(observer);

        public T Value
        {
            get => _subject.Value;
            set
            {
                if (!EqualityComparer<T>.Default.Equals(_subject.Value, value))
                {
                    _subject.OnNext(value);
                    PropertyChanged?.Invoke(this, InternalConstants.ValuePropertyChangedEventArgs);
                }
            }
        }
    }
}
