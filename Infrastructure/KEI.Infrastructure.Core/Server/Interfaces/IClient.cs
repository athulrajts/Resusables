using System.Net;
using System.ComponentModel;
using System.IO;

namespace KEI.Infrastructure.Server
{
    public delegate void ConnectionLostDelegate();
    public delegate void ResponseRecievedDelegate(Stream stream);

    public interface IClient : INotifyPropertyChanged
    {
        public bool IsConnected { get; }

        public bool Connect(string ipAddress, int port);
        public bool Connect(IPAddress ipAddress, int port);
        public bool Disconnect();
        public void SendMessage(byte[] messageBytes);

        public event ConnectionLostDelegate ConnectionLost;
        public event ResponseRecievedDelegate ResponseRecieved;
    }

    public interface IClient<THeader> : IClient
        where THeader : IMessageHeader
    {
        public void SendMessage(TcpMessage<THeader> command);
    }
}
