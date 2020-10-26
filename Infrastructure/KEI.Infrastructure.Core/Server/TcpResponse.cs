using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace KEI.Infrastructure.Server
{
    public abstract class TcpResponse : ITcpResponse
    {
        public abstract uint ResponseID { get; }

        public abstract string ResponseName { get; }

        public IMessageBody ResponseBody { get; set; }

        public void ExecuteResponse(Socket client, bool addCRLF = false)
        {
            try
            {
                if (client != null)
                {
                    client.Send(GetResponseBuffer());
                    if (addCRLF)
                    {
                        byte[] abCRLF = Encoding.ASCII.GetBytes("\r\n");
                        client.Send(abCRLF);
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Gets the byte array to send of the socket.
        /// <remarks>
        /// [0-3] Responsonse ID
        /// [4-7] Length Of Data, 0 if no data
        /// [8 -] Data
        /// </remarks>
        /// </summary>
        /// <returns>response array</returns> 
        public byte[] GetResponseBuffer()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);

            writer.Write(ResponseID);
            
            if(ResponseBody == null)
            {
                writer.Write((uint)0);
            }
            else
            {
                var body = ResponseBody.GetBytes();

                uint length = body == null ? 0 : (uint)body.Length;

                writer.Write(length);
                writer.Write(body);
            }

            return stream.ToArray();
        }

    }
}
