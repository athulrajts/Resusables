using System.IO;

namespace KEI.Infrastructure
{
    public class IntDataObject : DataObject<int>, IWriteToBinaryStream
    {
        public override string Type => "int";

        public override bool ValidateForType(string value)
        {
            return int.TryParse(value, out _);
        }

        public void WriteBytes(BinaryWriter writer)
        {
            writer.Write(Value);
        }
    }
}
