using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace KEI.Infrastructure.Server
{
    public class BufferBuilder
    {
        private List<byte> buffer = new List<byte>();

        private BufferBuilder()
        {

        }

        public static BufferBuilder Create() => new BufferBuilder();

        public static byte[] Combine(byte[] buffer1, byte[] buffer2, params byte[][] buffern)
        {
            int totalLength = buffer1.Length + buffer2.Length + buffern.Sum(x => x.Length);

            byte[] combinedBuffer = new byte[totalLength];

            Buffer.BlockCopy(buffer1, 0, combinedBuffer, 0, buffer1.Length);
            Buffer.BlockCopy(buffer2, 0, combinedBuffer, buffer1.Length, buffer2.Length);

            int bytesCopied = buffer1.Length + buffer2.Length;

            foreach (var buffer in buffern)
            {
                Buffer.BlockCopy(buffer, 0, combinedBuffer, bytesCopied, buffer.Length);

                bytesCopied += buffer.Length;
            }

            return combinedBuffer;

        }

        public BufferBuilder InsertBytes(byte[] bytes)
        {
            buffer.AddRange(new List<byte>(bytes));

            return this;
        }

        public BufferBuilder InsertUInt32(uint value) 
            => InsertBytes(BitConverter.GetBytes(value));

        public BufferBuilder InsertUInt32(int value)
            => InsertUInt32((uint)value);

        public void InsertFloat(float value)
            => InsertBytes(BitConverter.GetBytes(value));

        public BufferBuilder InsertDouble(double value)
            => InsertBytes(BitConverter.GetBytes(value));

        public BufferBuilder InsertByte(byte value)
        {
            buffer.Add(value);
            return this;
        }

        public BufferBuilder InsertString(string value, bool insertLength = true)
        {
            if(insertLength)
            {
                InsertUInt32((uint)value.Length);
            }

            return InsertBytes(Encoding.ASCII.GetBytes(value));
        }

        public byte[] GetBytes() => buffer.ToArray();
    }
}
