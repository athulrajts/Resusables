using KEI.Infrastructure.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace KEI.Infrastructure.Server
{
    public abstract class TcpIPCommand : ITcpIPCommand
    {
        private readonly InputParameterInfoCollection inputInfo = new InputParameterInfoCollection();

        public TcpIPCommand()
        {
            ConfigureIputs(inputInfo);
        }

        protected virtual void ConfigureIputs(InputParameterInfoCollection inputInfos)
        {

        }

        public abstract uint MessageID { get; }

        public abstract string CommandName { get; }

        public void ExecuteCommand(ITcpIPResponder responder, byte[] inputBuffer)
        {
            try
            {
                InternalExecute(responder, inputInfo.Parse(inputBuffer));
            }
            catch(Exception ex)
            {
                ErrorResponse err = new ErrorResponse(MessageID, "Unknown Exception caught");
                responder.SendResponse(err);

                Logger.Error("Unknown Exception caught", ex);
            }
        }

        public abstract void InternalExecute(ITcpIPResponder responder, InputParameterCollection inputs);
    }
}
