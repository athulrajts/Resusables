using System.Net.Sockets;

namespace KEI.Infrastructure.Server
{
    public interface ITcpIPCommand
    {
        public uint MessageID { get; }
        public string CommandName { get; }
        public void ExecuteCommand(ITcpIPResponder responder, byte[] inputBuffer);
    }
}
