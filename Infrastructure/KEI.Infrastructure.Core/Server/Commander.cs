using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Reflection;

namespace KEI.Infrastructure.Server
{
    public abstract class Commander : ICommander, IDisposable
    {
        public const uint DISCONNECT_COMMAND = 10;

        private readonly CommandMap _commandMap = new CommandMap();
        private Socket _client;
        private readonly IServer _server;


        public bool AddCRLF { get; set; }

        public Commander(IServer server)
        {
            _server = server;

            _server.OnClientConnected += (s) => SetClient(s);
            _server.OnClientDisconnected += () => _client.DisconnectAndCleanup();
            _server.OnServerDisconnected += () => DisconnectServerFromClient();
            _server.OnCommandReceived += OnCommandReceived;

            InitializeCommandMap(_commandMap);
        }

        private void OnCommandReceived(IMessageHeader header, Stream bodyStream)
        {
            if(header is MessageHeader mh)
            {
                if(mh.CommandID == DISCONNECT_COMMAND)
                {
                    _server.SetupForReconnection();
                }
                else
                {
                    ExecuteCommand(mh.CommandID, bodyStream);
                }
            }
            else if(header.GetType().GetProperty("CommandID") is PropertyInfo pi)
            {
                uint commandID = (uint)pi.GetValue(header);

                if(commandID == DISCONNECT_COMMAND)
                {
                    _server.SetupForReconnection();
                }
                else
                {
                    ExecuteCommand(commandID, bodyStream);
                }
            }
        }

        protected abstract void InitializeCommandMap(CommandMap commandMap);

        public void DisconnectServerFromClient()
        {
            //if we have a reference to the client and we are connected, send the disconnect
            //server command
            var disconnectResponse = new ServerDisconnectingResponse();
            disconnectResponse.ExecuteResponse(_client);

            _client.DisconnectAndCleanup();
        }

        public void ExecuteCommand(uint commandID, Stream stream)
        {
            if (_commandMap.ContainsKey(commandID))
            {
                _commandMap[commandID].ExecuteCommand(this, stream);
            }
            else
            {
                ErrorResponse nack = new ErrorResponse(commandID, $"Unknown Command({commandID})");
                SendResponse(nack);
            }
        }

        public void SendResponse(ITcpResponse response)
        {
            response.ExecuteResponse(_client, AddCRLF);
        }

        public void SetClient(Socket client)
        {
            _client = client;
        }

        public void Dispose()
        {
            DisconnectServerFromClient();
        }
    }

    public class CommandMap : Dictionary<uint, ITcpCommand>
    {
        public void AddCommand(ITcpCommand command)
        {
            Add(command.MessageID, command);
        }

        public void AddCommand<T>(params object[] args)
            where T : ITcpCommand
        {
            ITcpCommand command = (ITcpCommand)Activator.CreateInstance(typeof(T), args);

            Add(command.MessageID, command);
        }
    }

    public static class SocketExtensions
    {
        public static void DisconnectAndCleanup(this Socket s)
        {
            if (s != null)
            {
                //now we need to disconnect the client
                try
                {
                    if (s.Connected)
                    {
                        s.Disconnect(false);
                        s.Shutdown(SocketShutdown.Both);
                    }
                    s.Close();
                    s = null;
                }
                catch
                {
                    s = null;
                }
            }
        }
    }
}
