using System.IO;

namespace KEI.Infrastructure
{
    public class ByteDataObject : DataObject<byte>, IWriteToBinaryStream
    {
        public override string Type => "byte";

        public override bool ValidateForType(string value)
        {
            return byte.TryParse(value, out _);
        }

        public void WriteBytes(BinaryWriter writer)
        {
            writer.Write(Value);
        }
    }
}
