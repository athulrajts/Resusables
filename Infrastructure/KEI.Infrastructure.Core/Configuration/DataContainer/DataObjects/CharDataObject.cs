using System.IO;

namespace KEI.Infrastructure
{
    public class CharDataObject : DataObject<char>, IWriteToBinaryStream
    {
        public override string Type => "char";

        public override bool ValidateForType(string value)
        {
            return char.TryParse(value, out _);
        }

        protected override void OnStringValueChanged(string value)
        {
            char.TryParse(value, out _value);
        }

        public void WriteBytes(BinaryWriter writer)
        {
            writer.Write(Value);
        }
    }
}
