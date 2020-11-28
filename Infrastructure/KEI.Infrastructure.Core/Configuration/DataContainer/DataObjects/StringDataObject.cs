using System.IO;
using System.Text;

namespace KEI.Infrastructure
{
    public class StringDataObject : DataObject<string>, IWriteToBinaryStream
    {

        public override string Type => "string";

        protected override void OnStringValueChanged(string value)
        {
            _value = value;

            RaisePropertyChanged(nameof(Value));
        }

        public void WriteBytes(BinaryWriter writer)
        {
            writer.Write((uint)Value.Length);
            writer.Write(Encoding.ASCII.GetBytes(Value));
        }
    }
}
