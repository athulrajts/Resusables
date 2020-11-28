using System.IO;

namespace KEI.Infrastructure
{
    public class DoubleDataObject : DataObject<double>, IWriteToBinaryStream
    {
        public override string Type => "double";

        public override bool ValidateForType(string value)
        {
            return double.TryParse(value, out _);
        }

        public void WriteBytes(BinaryWriter writer)
        {
            writer.Write(Value);
        }
    }
}
