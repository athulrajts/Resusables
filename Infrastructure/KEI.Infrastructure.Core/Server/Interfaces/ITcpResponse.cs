using System.Net.Sockets;

namespace KEI.Infrastructure.Server
{
    public interface ITcpResponse
    {
        public uint ResponseID { get; }
        public string ResponseName { get; }
        public void ExecuteResponse(Socket client, bool addCRLF = false);
    }
}
