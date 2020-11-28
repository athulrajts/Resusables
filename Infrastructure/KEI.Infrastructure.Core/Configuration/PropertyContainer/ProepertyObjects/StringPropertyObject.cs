namespace KEI.Infrastructure
{
    public class StringPropertyObject : PropertyObject<string>
    {
        public override EditorType Editor => EditorType.String;

        public override string Type => "string";

        protected override void OnStringValueChanged(string value)
        {
            _value = value;
            RaisePropertyChanged(nameof(value));
        }
    }
}
