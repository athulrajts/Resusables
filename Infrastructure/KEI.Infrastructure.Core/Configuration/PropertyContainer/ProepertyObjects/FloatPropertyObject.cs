namespace KEI.Infrastructure
{
    public class FloatPropertyObject : PropertyObject<float>
    {
        public override EditorType Editor => EditorType.String;

        public override string Type => "float";
    }
}
