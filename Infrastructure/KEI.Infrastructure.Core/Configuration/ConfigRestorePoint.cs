namespace KEI.Infrastructure.Configuration
{
    public class ConfigHistoryItem
    {
        public string Name { get; set; }
        public object OldValue { get; set; }
        public bool HasChanges { get; set; }

        private object newValue;
        public object NewValue
        {
            get => newValue;
            set
            {
                newValue = value;

                HasChanges = newValue.ToString() != OldValue.ToString();
            }
        }


        private IPropertyContainer _original;
        private IPropertyContainer _clone;
        private string _fullName;
        public ConfigHistoryItem(IPropertyContainer original, IPropertyContainer clone, string fullName)
        {
            _original = original;
            _clone = clone;
            _fullName = fullName;

            object orig = null;
            _original.GetValue(fullName, ref orig);

            object copy = null;
            _clone.GetValue(fullName, ref copy);

            OldValue = orig;
            NewValue = copy;
        }

        public void Commit()
        {
            _original.SetValue(_fullName, NewValue);
            _original.Store();
        }

        public void Revert() => _clone.SetValue(_fullName, OldValue);

    }
}
