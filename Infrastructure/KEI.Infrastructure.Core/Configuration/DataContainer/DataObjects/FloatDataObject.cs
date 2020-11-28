using System.IO;

namespace KEI.Infrastructure
{
    public class FloatDataObject : DataObject<float>, IWriteToBinaryStream
    {
        public override string Type => "float";

        public override bool ValidateForType(string value)
        {
            return float.TryParse(value, out _);
        }

        public void WriteBytes(BinaryWriter writer)
        {
            writer.Write(Value);
        }
    }
}
