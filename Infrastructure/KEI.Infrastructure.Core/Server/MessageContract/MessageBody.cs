using Prism.Mvvm;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KEI.Infrastructure.Server
{

    public interface IMessageBody
    {
        public void WriteBytes(Stream stream);

        public void ReadBytes(Stream stream);

        public byte[] GetBytes();
    }

    public abstract class MessageBody : BindableBase, IMessageBody
    {
        public virtual void ReadBytes(Stream stream)
        {
            var reader = new BinaryReader(stream);

            foreach (var prop in GetType().GetProperties())
            {
                if(prop.PropertyType == typeof(uint))
                {
                    prop.SetValue(this, reader.ReadUInt32());
                }
                else if(prop.PropertyType == typeof(int))
                {
                    prop.SetValue(this, reader.ReadInt32());
                }
                else if(prop.PropertyType == typeof(bool))
                {
                    prop.SetValue(this, reader.ReadBoolean());
                }
                else if(prop.PropertyType == typeof(float))
                {
                    prop.SetValue(this, reader.ReadSingle());
                }
                else if(prop.PropertyType == typeof(double))
                {
                    prop.SetValue(this, reader.ReadDouble());
                }
                else if(prop.PropertyType == typeof(string))
                {
                    prop.SetValue(this, reader.ReadUInt32PrefixedString());
                }
            }
        }

        public virtual void WriteBytes(Stream stream)
        {
            var writer = new BinaryWriter(stream);

            foreach (var prop in GetType().GetProperties())
            {
                if (prop.PropertyType == typeof(uint))
                {
                    writer.Write((uint)prop.GetValue(this));
                }
                else if (prop.PropertyType == typeof(int))
                {
                    writer.Write((int)prop.GetValue(this));
                }
                else if (prop.PropertyType == typeof(bool))
                {
                    writer.Write((bool)prop.GetValue(this));
                }
                else if (prop.PropertyType == typeof(float))
                {
                    writer.Write((float)prop.GetValue(this));
                }
                else if (prop.PropertyType == typeof(double))
                {
                    writer.Write((double)prop.GetValue(this));
                }
                else if (prop.PropertyType == typeof(string))
                {
                    writer.WriteUInt32PrefixedString((string)prop.GetValue(this));
                }
            }

        }

        public byte[] GetBytes()
        {
            using var stream = new MemoryStream();

            WriteBytes(stream);

            var response = stream.ToArray();

            return response;
        }
    }

    public static class BinaryReaderWriterExtensions
    {
        public static string ReadUInt32PrefixedString(this BinaryReader reader)
        {
            uint length = reader.ReadUInt32();

            var bytes = reader.ReadBytes((int)length);

            return Encoding.ASCII.GetString(bytes);
        }

        public static void WriteUInt32PrefixedString(this BinaryWriter writer, string str)
        {
            writer.Write((uint)Encoding.ASCII.GetByteCount(str));
            writer.Write(Encoding.ASCII.GetBytes(str));
        }

    }
}
