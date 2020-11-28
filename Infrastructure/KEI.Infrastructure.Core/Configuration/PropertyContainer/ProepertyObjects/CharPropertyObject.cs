namespace KEI.Infrastructure
{
    public class CharPropertyObject : PropertyObject<char>
    {
        public override EditorType Editor => EditorType.String;

        public override string Type => "char";
        
        protected override void OnStringValueChanged(string value)
        {
            if (char.TryParse(value, out _value))
            {
                RaisePropertyChanged(nameof(Value));
            }
        }
    }
}
