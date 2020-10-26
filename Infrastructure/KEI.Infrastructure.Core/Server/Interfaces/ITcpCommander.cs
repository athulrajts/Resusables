using System.IO;
using System.Net.Sockets;

namespace KEI.Infrastructure.Server
{
    public interface ITcpCommander
    {
        void SetClient(Socket client);
        void ExecuteCommand(uint commandID, Stream buffer);
        void DisconnectServerFromClient();
    }
}
