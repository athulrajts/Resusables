namespace KEI.Infrastructure
{
    public class SnapShotItem : BindableObject
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private string type;
        public string Type
        {
            get { return type; }
            set { SetProperty(ref type, value); }
        }

        private object _value;
        public object Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        public SnapShotItem(DataObject obj)
        {
            Name = obj.Name;
            Type = obj.Type;
            Value = obj.GetValue();
        }

        public SnapShotItem() { }
    }
}
