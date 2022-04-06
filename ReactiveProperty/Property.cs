namespace ReactiveProperty
{
    internal sealed class Property<T> : PropertySubject<T>, IProperty<T>
    {
        public Property(T value = default) : base(value)
        {
        }
        public new T Value { get => base.Value; set => base.Value = value; }
    }
}
