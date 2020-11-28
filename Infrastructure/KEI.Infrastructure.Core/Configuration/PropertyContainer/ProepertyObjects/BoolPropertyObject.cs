namespace KEI.Infrastructure
{
    public class BoolPropertyObject : PropertyObject<bool>
    {
        public override EditorType Editor => EditorType.Bool;

        public override string Type => "bool";

        protected override void OnStringValueChanged(string value)
        {
            if(bool.TryParse(value, out _value))
            {
                RaisePropertyChanged(nameof(Value));
            }

        }
    }
}
