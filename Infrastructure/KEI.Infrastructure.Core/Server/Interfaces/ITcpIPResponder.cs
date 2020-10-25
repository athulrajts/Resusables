namespace KEI.Infrastructure.Server
{
    public interface ITcpIPResponder
    {
        public bool AddCRLF { get; }
        public void SendResponse(ITcpIPResponse response);
    }
}
