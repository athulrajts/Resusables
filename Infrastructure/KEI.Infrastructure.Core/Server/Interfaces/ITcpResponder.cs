namespace KEI.Infrastructure.Server
{
    public interface ITcpResponder
    {
        public bool AddCRLF { get; }
        public void SendResponse(ITcpResponse response);
    }
}
