using System;
using System.ComponentModel;

namespace KEI.Infrastructure.Server
{
    public interface IServer : INotifyPropertyChanged
    {
        public bool IsConnected { get; }
        public bool IsRunning { get; }
        public bool StartServer(string ipAddress, ushort port);
        public bool StopServer();
    }
}
