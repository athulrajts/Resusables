namespace KEI.Infrastructure.Server
{
    public class ServerDisconnectingResponse : TcpIPResponse
    {
        public override uint ResponseID => 11;

        public override string ResponseName => "TCP/IP Server stopped running";
    }
}
