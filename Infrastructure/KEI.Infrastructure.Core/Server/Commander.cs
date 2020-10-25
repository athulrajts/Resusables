using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace KEI.Infrastructure.Server
{
    public abstract class Commander : ITcpIPCommander, ITcpIPResponder, IDisposable
    {

        private readonly CommandMap _commandMap = new CommandMap();
        private Socket _client;

        public bool AddCRLF { get; set; }

        public Commander(Socket client, bool addCRLF = false)
        {
            _client = client;
            
            AddCRLF = addCRLF;
            InitializeCommandMap(_commandMap);
        }

        public Commander()
        {
            InitializeCommandMap(_commandMap);
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

        public void ExecuteCommand(uint commandID, byte[] buffer)
        {
            if(_client == null)
            {
                return;
            }

            if(_commandMap.ContainsKey(commandID))
            {
                _commandMap[commandID].ExecuteCommand(this, buffer);
            }
            else
            {
                ErrorResponse nack = new ErrorResponse(commandID, $"Unknown Command({commandID})");
                SendResponse(nack);
            }
        }

        public void SendResponse(ITcpIPResponse response)
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

    public class CommandMap : Dictionary<uint, ITcpIPCommand>
    {
        public void AddCommand(ITcpIPCommand command)
        {
            Add(command.MessageID, command);
        }

        //public void AddCommand<T>()
        //    where T : ITcpIPCommand, new()
        //{
        //    ITcpIPCommand command = new T();

        //    Add(command.MessageID, command);
        //}

        public void AddCommand<T>(params object[] args)
            where T : ITcpIPCommand
        {
            ITcpIPCommand command = (ITcpIPCommand)Activator.CreateInstance(typeof(T), args);

            Add(command.MessageID, command);
        }
    }
}
