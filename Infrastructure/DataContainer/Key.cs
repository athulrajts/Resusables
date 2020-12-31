namespace KEI.Infrastructure
{
    /// <summary>
    /// Typed key, so you don't know to wonder what type the result will be
    /// </summary>
    /// <typeparam name="T"><see cref="System.Type"/> of value</typeparam>
    public class Key<T>
    {
        public string Name { get; set; }

        public T DefaultValue { get; set; } = default;

        public Key(string name, T defaultValue = default)
        {
            Name = name;
            DefaultValue = defaultValue;
        }

    }

    #region Specific Implementations

    public class BoolKey : Key<bool>
    {
        public BoolKey(string name, bool defaultValue = default) : base(name, defaultValue) { }
    }

    public class IntKey : Key<int>
    {
        public IntKey(string name, int defaultValue = default) : base(name, defaultValue) { }
    }

    public class FloatKey : Key<float>
    {
        public FloatKey(string name, float defaultValue = default) : base(name, defaultValue) { }
    }

    public class DoubleKey : Key<double>
    {
        public DoubleKey(string name, double defaultValue = default) : base(name, defaultValue) { }
    }

    public class StringKey : Key<string>
    {
        public StringKey(string name, string defaultValue = default) : base(name, defaultValue) { }
    }

    #endregion

}
