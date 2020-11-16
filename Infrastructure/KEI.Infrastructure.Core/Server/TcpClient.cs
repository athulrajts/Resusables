using Prism.Mvvm;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace KEI.Infrastructure.Server
{
    public class TcpClient<THeader> : BindableBase, IClient<THeader>
        where THeader : IMessageHeader, new()
    {
        private System.Net.Sockets.TcpClient _client = new System.Net.Sockets.TcpClient(System.Net.Sockets.AddressFamily.InterNetwork);

        public bool IsConnected => _client.Connected;
        public int HeaderSize { get; set; } = 8;
        public int RecieveBufferSize { get; set; } = 128;

        public event ConnectionLostDelegate ConnectionLost;
        public event ResponseRecievedDelegate ResponseRecieved;

        public bool Connect(string ipAddress, int port)
        {
            if(IPAddress.TryParse(ipAddress, out IPAddress address) == false)
            {
                throw new ArgumentException("Invalid IP address");
            }

            return Connect(address, port);
        }

        public bool Connect(IPAddress ipAddress, int port)
        {
            try
            {
                _client.Connect(new IPEndPoint(ipAddress, port));

                Task.Factory.StartNew(() => WaitForData(), TaskCreationOptions.LongRunning);
            }
            catch (Exception)
            {
                _client.Dispose();
                _client.Close();
                _client = new System.Net.Sockets.TcpClient(System.Net.Sockets.AddressFamily.InterNetwork);
                return false;
            }
            finally
            {
                RaisePropertyChanged(nameof(IsConnected));
            }

            return true;
        }

        public bool Disconnect()
        {
            try
            {
                _client.Dispose();
                _client.Close();
                _client = new System.Net.Sockets.TcpClient(System.Net.Sockets.AddressFamily.InterNetwork);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public void SendMessage(TcpMessage<THeader> command)
        {
            var stream = _client.GetStream();

            command.WriteBytes(stream);
        }

        public void SendMessage(byte[] messageBytes)
        {
            var stream = _client.GetStream();

            stream.Write(messageBytes);
        }

        private async void WaitForData()
        {
            while (true)
            {
                if (_client == null || _client.Connected == false)
                {
                    break;
                }

                try
                {
                    var stream = _client.GetStream();

                    // Read Header
                    byte[] headerBytes = new byte[HeaderSize];

                    int size = await stream.ReadAsync(headerBytes, 0, HeaderSize);

                    if (size != HeaderSize)
                    {
                        throw new Exception("Invalid Header");
                    }

                    // Copy header to MemoryStream because NetworkStream doesn't support seeking
                    var responseStream = new MemoryStream();
                    responseStream.Write(headerBytes);

                    // Resen to begining to start reading header from start
                    responseStream.Position = 0;

                    // Parse header
                    var header = responseStream.ReadHeader<THeader>();

                    // TODO :: Consider using System.IO.Pipelines ??
                    // Start reading body
                    byte[] buffer = new byte[RecieveBufferSize];
                    int bytesRead = 0;

                    while (bytesRead < header.MessageLength)
                    {
                        int chunkSize = await stream.ReadAsync(buffer, 0, buffer.Length);
                        bytesRead += chunkSize;
                        responseStream.Write(buffer);
                    }

                    responseStream.Position = 0;

                    // Broadcast the command recieved
                    ResponseRecieved?.Invoke(responseStream);

                    // Dispose body stream
                    responseStream.Dispose();
                }
                catch (Exception)
                {
                    _client.Dispose();
                    _client.Close();
                    _client = new System.Net.Sockets.TcpClient(System.Net.Sockets.AddressFamily.InterNetwork);
                    ConnectionLost?.Invoke();
                }
            }
        }
    }
}
