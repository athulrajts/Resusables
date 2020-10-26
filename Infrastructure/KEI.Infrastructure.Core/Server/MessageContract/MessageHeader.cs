using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KEI.Infrastructure.Server
{
    public interface IMessageHeader
    {
        public uint MessageLength { get; }

        public void WriteBytes(Stream stream);

        public void ReadBytes(Stream stream);
    }

    public class MessageHeader : IMessageHeader
    {
        public uint CommandID { get; set; }
        public uint MessageLength { get; set; }

        public void WriteBytes(Stream stream)
        {
            using var writer = new BinaryWriter(stream);

            writer.Write(CommandID);
            writer.Write(MessageLength);
        }

        public void ReadBytes(Stream stream)
        {
            using var reader = new BinaryReader(stream);

            CommandID = reader.ReadUInt32();
            MessageLength = reader.ReadUInt32();
        }
    }
}
