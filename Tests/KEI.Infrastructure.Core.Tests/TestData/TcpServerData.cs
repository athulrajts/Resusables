using KEI.Infrastructure.Server;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading;

namespace KEI.Infrastructure.Core.Tests.TestData
{
    public class TestMessageBody : MessageBody
    {
        public uint MyProperty1 { get; set; }
        public double MyProperty2 { get; set; }
        public float MyProperty3 { get; set; }
        public string MyProperty4 { get; set; }
    }

    public class TcpServerProxy : InpcImplementation
    {

        public const ushort PORT = 8000;

        private readonly IServer server = new CommandServer();

        private AutoResetEvent connectionEvent = new AutoResetEvent(false);
        private AutoResetEvent messageEvent = new AutoResetEvent(false);
        private AutoResetEvent disconnectionEvent = new AutoResetEvent(false);

        public void StopServer()
        {
            server.StopServer();
        }

        public void WaitConnection(int timeout)
        {
            connectionEvent.WaitOne(timeout);
        }

        public void WaitCommand(int timeout)
        {
            messageEvent.WaitOne(timeout);
        }

        public void WaitDisconnection(int timeout)
        {
            disconnectionEvent.WaitOne(timeout);
        }

        private bool isConnected;
        public bool IsConnected
        {
            get { return isConnected; }
            set { SetProperty(ref isConnected, value); }
        }

        private bool commandRecieved;
        public bool CommandRecieved
        {
            get { return commandRecieved; }
            set { SetProperty(ref commandRecieved, value); }
        }

        public IMessageHeader LastHeader { get; set; }
        public MemoryStream LastBody { get; set; } = new MemoryStream();
        public uint LastBodyLength { get; set; }

        public TcpServerProxy()
        {
            server.OnClientConnected += Server_OnClientConnected;
            server.OnClientDisconnected += Server_OnClientDisconnected;
            server.OnCommandReceived += Server_OnCommandReceived;
            server.OnServerDisconnected += Server_OnServerDisconnected;

            server.StartServer(PORT);
        }

        private void Server_OnServerDisconnected()
        {
            IsConnected = false;
            disconnectionEvent.Set();
        }

        public void Server_OnCommandReceived(IMessageHeader header, Stream bodyStream)
        {
            CommandRecieved = true;
            LastHeader = header;
            bodyStream.CopyTo(LastBody);
            LastBody.Position = 0;
            messageEvent.Set();
        }

        public void Server_OnClientDisconnected()
        {
            IsConnected = false;
            disconnectionEvent.Set();
        }

        public void Server_OnClientConnected(System.Net.Sockets.Socket client)
        {
            IsConnected = true;
            connectionEvent.Set();
        }
    }

    //public class TcpMessage
    //{
    //    private readonly MessageHeader _header;
    //    private readonly MessageBody _body;

    //    public TcpMessage(uint ID, MessageBody body)
    //    {
    //        _header = new MessageHeader { ID = ID };
    //        _body = body;
    //        _body.PropertyChanged += body_PropertyChanged;
    //        RecalculateLength();
    //    } 

    //    private void body_PropertyChanged(object sender, PropertyChangedEventArgs e)
    //    {
    //        if (GetType().GetProperty(e.PropertyName) is PropertyInfo pi)
    //        {
    //            if (pi.PropertyType == typeof(string))
    //            {
    //                RecalculateLength();
    //            }
    //        }
    //    }

    //    private void RecalculateLength()
    //    {
    //        _header.MessageLength = 0;
            
    //        foreach (var prop in _body.GetType().GetProperties())
    //        {
    //            if (prop.PropertyType == typeof(string))
    //            {
    //                string value = (string)prop.GetValue(_body);

    //                _header.MessageLength += (uint)value.Length;
    //            }
    //            else if (prop.PropertyType == typeof(int) ||
    //                prop.PropertyType == typeof(uint) ||
    //                prop.PropertyType == typeof(float))
    //            {
    //                _header.MessageLength += 4;
    //            }
    //            else if (prop.PropertyType == typeof(double))
    //            {
    //                _header.MessageLength += 8;
    //            }
    //            else if (prop.PropertyType == typeof(bool))
    //            {
    //                _header.MessageLength += 1;
    //            }
    //        }
    //    }

    //    public void WriteBytes(Stream stream)
    //    {
    //        _header.WriteBytes(stream);
    //        _body.WriteBytes(stream);
    //    }
    //}
}
