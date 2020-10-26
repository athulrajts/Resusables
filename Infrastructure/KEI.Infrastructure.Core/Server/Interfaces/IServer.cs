using System;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;

namespace KEI.Infrastructure.Server
{
    public delegate void ClientConnectedDelagate(Socket client);
    public delegate void ServerDisconnectedDelegate();
    public delegate void ClientDisconnectedDelegate();
    public delegate void CommandRecievedDelegate(IMessageHeader header, Stream bodyStream);

    public interface IServer : INotifyPropertyChanged
    {
        public bool IsConnected { get; }
        public bool IsRunning { get; }
        public bool StartServer(ushort port);
        public bool StopServer();
        public void SetupForReconnection();

        public event ClientConnectedDelagate OnClientConnected;
        public event ServerDisconnectedDelegate OnServerDisconnected;
        public event ClientDisconnectedDelegate OnClientDisconnected;
        public event CommandRecievedDelegate OnCommandReceived;
    }
}
