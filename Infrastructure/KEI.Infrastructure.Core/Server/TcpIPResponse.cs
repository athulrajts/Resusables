using System;
using System.Net.Sockets;
using System.Text;

namespace KEI.Infrastructure.Server
{
    public abstract class TcpIPResponse : ITcpIPResponse
    {
        public abstract uint ResponseID { get; }

        public abstract string ResponseName { get; }

        protected abstract byte[] GetDataBuffer();

        public void ExecuteResponse(Socket client, bool addCRLF = false)
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
            var dataBuffer = GetDataBuffer();

            return BufferBuilder.Combine(BitConverter.GetBytes(ResponseID),
                BitConverter.GetBytes((uint)dataBuffer.Length),
                dataBuffer);
        }

    }
}
