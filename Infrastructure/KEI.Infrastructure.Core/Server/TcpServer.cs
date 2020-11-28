using KEI.Infrastructure.Configuration;
using KEI.Infrastructure.Logging;
using KEI.Infrastructure.Service;
using KEI.Infrastructure.Utils;
using Prism.Mvvm;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace KEI.Infrastructure.Server
{
    public abstract class TcpServer<THeader> : BindableBase, IServer
        where THeader: IMessageHeader, new()
    {
        private TcpListener _listener;
        private System.Net.Sockets.TcpClient _client;

        public event ClientConnectedDelagate OnClientConnected;
        public event ServerDisconnectedDelegate OnServerDisconnected;
        public event ClientDisconnectedDelegate OnClientDisconnected;
        public event CommandRecievedDelegate OnCommandReceived;

        public bool IsConnected => _client != null ? _client.Connected : false;

        public bool IsRunning => _listener != null;

        public abstract int HeaderSize { get; }

        public int RecieveBufferSize { get; set; }

        public int Port { get; set; }

        public bool StartServer(int port)
        {
            if (_listener == null)
            {
                _listener = new TcpListener(IPAddress.Any, port);
                _listener.Start();
                _listener.BeginAcceptTcpClient(new AsyncCallback(OnTcpClientConnected), null);

                return true;
            }

            // Already running
            else if (_listener.Server.Connected)
            {
                return false;
            }

            return false;
        }

        public bool StartServer() => StartServer(Port);

        private void OnTcpClientConnected(IAsyncResult ar)
        {
            try
            {
                _client = _listener?.EndAcceptTcpClient(ar);

                if (_client != null)
                {
                    Task.Factory.StartNew(() => WaitForData(), TaskCreationOptions.LongRunning);

                    OnClientConnected?.Invoke(_client.Client); 
                }

                RaisePropertyChanged(nameof(IsRunning));
            }
            catch { }
        }

        public bool StopServer()
        {
            OnServerDisconnected?.Invoke();

            if (_client != null)
            {
                var stream = _client.GetStream();
                stream.Write(BitConverter.GetBytes(Commander.DISCONNECT_COMMAND + 1));
                stream.Write(BitConverter.GetBytes((uint)4));

                _client.Close();
                _client.Dispose();
                _client = null; 
            }

            if (_listener != null)
            {
                _listener.Stop();
                _listener = null; 
            }


            RaisePropertyChanged(nameof(IsRunning));
            RaisePropertyChanged(nameof(IsConnected));

            return true;
        }

        private async void WaitForData()
        {
            while (true)
            {
                if (_client == null || IsRunning == false)
                {
                    break;
                }

                try
                {
                    var stream = _client.GetStream();

                    // Read Header
                    byte[] headerBytes = new byte[HeaderSize];

                    int size = await stream.ReadAsync(headerBytes, 0, HeaderSize);

                    if(size != HeaderSize)
                    {
                        throw new Exception("Invalid Header");
                    }

                    // Copy header to MemoeryStream because NetworkStream doesn't support seeking
                    var headerStream = new MemoryStream();
                    headerStream.Write(headerBytes);
                    
                    // Resen to begining to start reading header from start
                    headerStream.Position = 0;
                    
                    // Parse header
                    var header = headerStream.ReadHeader<THeader>();
                    
                    // Dispose header stream
                    headerStream.Dispose();

                    // TODO :: Consider using System.IO.Pipelines ??
                    // Start reading body
                    byte[] buffer = new byte[RecieveBufferSize];
                    int bytesRead = 0;
                    var bodyStream = new MemoryStream();
                    
                    while(bytesRead < header.MessageLength)
                    {
                        int chunkSize = await stream.ReadAsync(buffer, 0, buffer.Length);
                        bytesRead += chunkSize;
                        bodyStream.Write(buffer);
                    }

                    bodyStream.Position = 0;

                    // Broadcast the command recieved
                    OnCommandReceived?.Invoke(header, bodyStream);

                    // Dispose body stream
                    bodyStream.Dispose();
                }
                catch (Exception ex)
                {
                    SetupForReconnection();

                    Logger.Error("Unhandled exception", ex);
                }
            }
        }

        public void SetupForReconnection()
        {
            if (_client != null)
            {
                _client.Close();
                _client.Dispose();
                _client = null; 
            }

            OnClientDisconnected?.Invoke();

            _listener.BeginAcceptTcpClient(new AsyncCallback(OnTcpClientConnected), null);

            RaisePropertyChanged(nameof(IsConnected));
        }
    }


    public class CommandServer : TcpServer<MessageHeader>, IConfigurable, IInitializable
    {
        public override int HeaderSize => 8;

        public string ConfigPath => PathUtils.GetPath("Configs/Server.xcfg");

        public virtual bool LoadConfig()
        {
            IDataContainer config = DataContainerBuilder.FromFile(ConfigPath);

            if(config is null)
            {
                StoreConfig(ConfigPath);
                config = DataContainerBuilder.FromFile(ConfigPath);
            }

            Port = config.Get<ushort>(nameof(Port));
            RecieveBufferSize = config.Get<int>(nameof(RecieveBufferSize));

            return true;
        }

        public virtual bool StoreConfig(string path)
        {
            IDataContainer config = new DataContainer
            {
                { nameof(Port), Port },
                { nameof(RecieveBufferSize), RecieveBufferSize }
            };

            return config.Store(path);
        }

        public virtual bool ResetConfig()
        {
            Port = 8000;

            // Amount of data recieve should be small, since we're only sending
            // command + params
            // Might need to increase size, if volume of data is larger
            RecieveBufferSize = 128;

            return true;
        }

        public bool Initialize()
        {
            LoadConfig();

            return StartServer();
        }
    }
}
