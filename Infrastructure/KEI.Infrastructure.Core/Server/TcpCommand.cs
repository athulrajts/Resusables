using KEI.Infrastructure.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KEI.Infrastructure.Server
{
    public abstract class TcpCommand : ITcpCommand
    {
        public abstract uint MessageID { get; }

        public abstract string CommandName { get; }

        public void ExecuteCommand(ITcpResponder responder, Stream stream)
        {
            try
            {
                InternalExecute(responder, stream);
            }
            catch (Exception ex)
            {
                ErrorResponse err = new ErrorResponse(MessageID, "Unknown Exception caught");
                responder.SendResponse(err);

                Logger.Error("Unknown Exception caught", ex);
            }
        }

        public virtual void InternalExecute(ITcpResponder responder, Stream inputs) { }
    }
}
