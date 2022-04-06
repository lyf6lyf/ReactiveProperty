using System.ComponentModel;

namespace ReactiveProperty
{
    internal static class Constants
    {
        public static readonly PropertyChangedEventArgs ValuePropertyChangedEventArgs = new PropertyChangedEventArgs("Value");

        public static readonly IReadOnlyProperty<bool> True = new ReadOnlyProperty<bool>(true);
        public static readonly IReadOnlyProperty<bool> False = new ReadOnlyProperty<bool>(false);
    }
}
