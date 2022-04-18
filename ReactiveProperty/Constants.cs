using System.ComponentModel;

namespace ReactiveProperty
{
    internal static class InternalConstants
    {
        public static readonly PropertyChangedEventArgs ValuePropertyChangedEventArgs = new PropertyChangedEventArgs("Value");
    }

    public static class Constants
    {
        public static readonly IReadOnlyProperty<bool> True = new ReadOnlyProperty<bool>(true);
        public static readonly IReadOnlyProperty<bool> False = new ReadOnlyProperty<bool>(false);
    }
}
