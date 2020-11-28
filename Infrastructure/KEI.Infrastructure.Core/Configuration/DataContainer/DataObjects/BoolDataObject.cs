using System.IO;

namespace KEI.Infrastructure
{
    public class BoolDataObject : DataObject<bool>, IWriteToBinaryStream
    {
        public override string Type => "bool";

        public override bool ValidateForType(string value)
        {
            return bool.TryParse(value, out _);
        }

        protected override void OnStringValueChanged(string value)
        {
            if (bool.TryParse(value, out _value))
            {
                RaisePropertyChanged(nameof(Value));
            }

        }

        public void WriteBytes(BinaryWriter writer)
        {
            writer.Write(Value);
        }
    }
}
