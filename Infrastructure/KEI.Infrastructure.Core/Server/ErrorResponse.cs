using System;

namespace KEI.Infrastructure.Server
{
    public class ErrorResponse : TcpIPResponse
    {
        public string ErrorMessage { get; set; }
        public uint CommandID { get; set; }

        public override uint ResponseID => 0;

        public override string ResponseName => "TCP/IP Command Error";

        public ErrorResponse(uint commandId, string errorMessage)
        {
            CommandID = commandId;
            ErrorMessage = errorMessage;
        }

        protected override byte[] GetDataBuffer()
        {
            return BufferBuilder.Create()
                .InsertUInt32(CommandID)
                .InsertString(ErrorMessage)
                .GetBytes();
        }
    }
}
