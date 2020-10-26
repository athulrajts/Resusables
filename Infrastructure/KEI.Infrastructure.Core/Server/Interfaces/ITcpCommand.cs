using System.IO;
using System.Net.Sockets;

namespace KEI.Infrastructure.Server
{
    public interface ITcpCommand
    {
        public uint MessageID { get; }
        public string CommandName { get; }
        public void ExecuteCommand(ITcpResponder responder, Stream inputBuffer);
    }
}
