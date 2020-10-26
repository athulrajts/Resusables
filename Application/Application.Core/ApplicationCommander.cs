using KEI.Infrastructure;
using KEI.Infrastructure.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Application.Core
{
    public class ApplicationCommander : Commander
    {
        public ApplicationCommander(IServer server) : base(server) { }

        protected override void InitializeCommandMap(CommandMap commandMap)
        {
            commandMap.AddCommand<ShowDialogCommand>();
        }
    }

    public class ShowDialogCommand : TcpCommand
    {
        public class CommandMessage : MessageBody 
        {
            public string Message { get; set; }
        }

        public override uint MessageID => 100;

        public override string CommandName => "Test Command";

        public override void InternalExecute(ITcpResponder responder, Stream inputs)
        {
            var message = inputs.ReadBody<CommandMessage>();

            CommonServiceLocator.ServiceLocator.Current.GetInstance<IViewService>().Inform(message.Message);
        }
    }
}
