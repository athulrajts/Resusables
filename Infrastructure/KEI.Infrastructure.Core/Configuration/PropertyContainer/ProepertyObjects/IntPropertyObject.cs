using KEI.Infrastructure;

namespace KEI.Infrastructure
{
    public class IntPropertyObject : PropertyObject<int>
    {
        public override EditorType Editor => EditorType.String;

        public override string Type => "int";
    }
}
