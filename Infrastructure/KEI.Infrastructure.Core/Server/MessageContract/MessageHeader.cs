using System.IO;

namespace KEI.Infrastructure.Server
{
    public interface IMessageHeader
    {
        public uint MessageLength { get; set; }

        public void WriteBytes(Stream stream);

        public void ReadBytes(Stream stream);
    }

    public class MessageHeader : IMessageHeader
    {
        public uint ID { get; set; }
        public uint MessageLength { get; set; }

        public MessageHeader(uint id)
        {
            ID = id;
        }

        public MessageHeader()
        {

        }

        public void WriteBytes(Stream stream)
        {
            var writer = new BinaryWriter(stream);

            writer.Write(ID);
            writer.Write(MessageLength);
        }

        public void ReadBytes(Stream stream)
        {
            var reader = new BinaryReader(stream);

            ID = reader.ReadUInt32();
            MessageLength = reader.ReadUInt32();
        }
    }
}
