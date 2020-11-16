using System;
using System.IO;
using KEI.Infrastructure.Logging;

namespace KEI.Infrastructure.Server
{
    public abstract class TcpCommand : ITcpCommand
    {
        public abstract uint MessageID { get; }

        public abstract string CommandName { get; }

        public void ExecuteCommand(ITcpResponder responder, Stream inputBuffer)
        {
            try
            {
                InternalExecute(responder, inputBuffer);
            }
            catch (Exception ex)
            {
                ErrorResponse err = new ErrorResponse(MessageID, "Unknown Exception caught");
                responder.SendResponse(err);

                Logger.Error("Unknown Exception caught", ex);
            }
        }

        public virtual void InternalExecute(ITcpResponder responder, Stream inputBuffer) { }
    }
}
