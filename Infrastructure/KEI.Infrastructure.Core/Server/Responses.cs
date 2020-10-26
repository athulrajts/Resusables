namespace KEI.Infrastructure.Server
{
    public class ServerDisconnectingResponse : TcpResponse
    {
        public override uint ResponseID => 11;

        public override string ResponseName => "TCP/IP Server stopped running";
    }

    public class ErrorResponse : TcpResponse
    {
        class ResponseMessage : MessageBody
        {
            public uint CommandID { get; set; }
            public string ErrorMessage { get; set; }
        }

        public override uint ResponseID => 0;

        public override string ResponseName => "TCP/IP Command Error";

        public ErrorResponse(uint commandId, string errorMessage)
        {
            ResponseBody = new ResponseMessage
            {
                CommandID = commandId,
                ErrorMessage = errorMessage
            };
        }
    }
}
