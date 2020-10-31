using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace TCPClient.Models
{
    public class InputParameterCollection : ObservableCollection<InputParameter>
    {
        public Tuple<uint, byte[]> GetBytesAndLength()
        {
            using var stream = new MemoryStream();

            foreach (var input in this)
            {
                input.WriteBytes(stream);
            }

            byte[] messageBytes = stream.ToArray();
            uint length = (uint)messageBytes.Length;

            return new Tuple<uint, byte[]>(length, messageBytes);
        }

        public bool IsValid() => !this.Any(x => x.IsValid() == false);
    }
}
