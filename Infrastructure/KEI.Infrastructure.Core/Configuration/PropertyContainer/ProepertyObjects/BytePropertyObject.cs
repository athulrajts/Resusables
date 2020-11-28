namespace KEI.Infrastructure
{
    public class BytePropertyObject : PropertyObject<byte>
    {
        public override EditorType Editor => EditorType.String;

        public override string Type => "byte";
    }
}
