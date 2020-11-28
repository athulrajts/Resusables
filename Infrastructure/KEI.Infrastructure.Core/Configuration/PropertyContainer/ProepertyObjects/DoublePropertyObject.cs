namespace KEI.Infrastructure
{
    public class DoublePropertyObject : PropertyObject<double>
    {
        public override EditorType Editor => EditorType.String;

        public override string Type => "double";
    }
}
