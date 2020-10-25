using System.Net.Sockets;

namespace KEI.Infrastructure.Server
{
    public interface ITcpIPCommander
    {
        void SetClient(Socket client);
        void ExecuteCommand(uint commandID, byte[] buffer);
        void DisconnectServerFromClient();
    }
}
