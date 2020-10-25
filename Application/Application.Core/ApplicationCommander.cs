using KEI.Infrastructure;
using KEI.Infrastructure.Server;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Application.Core
{
    public class ApplicationCommander : Commander
    {
        protected override void InitializeCommandMap(CommandMap commandMap)
        {
            commandMap.AddCommand<ShowDialogCommand>();
        }
    }

    public class ShowDialogCommand : TcpIPCommand
    {
        public override uint MessageID => 100;

        public override string CommandName => "Test Command";

        public override void InternalExecute(ITcpIPResponder responder, InputParameterCollection inputs)
        {
            inputs.TryGetValue<string>("Message", out string msg);

            CommonServiceLocator.ServiceLocator.Current.GetInstance<IViewService>().Inform(msg);
        }

        protected override void ConfigureIputs(InputParameterInfoCollection inputInfos)
        {
            inputInfos.Add<string>("Message");
        }
    }
}
