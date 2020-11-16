using System.IO;

namespace KEI.Infrastructure.Server
{
    public interface ITcpCommand
    {
        public uint MessageID { get; }
        public string CommandName { get; }
        public void ExecuteCommand(ITcpResponder responder, Stream inputBuffer);
    }
}
