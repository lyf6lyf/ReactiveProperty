using System;
using System.ComponentModel;

namespace ReactiveProperty
{
    public interface IReadOnlyProperty<out T> : IObservable<T>, INotifyPropertyChanged
    {
        T Value { get; }
    }

    public interface IProperty<T> : IReadOnlyProperty<T>
    {
        new T Value { get; set; }
    }
}
