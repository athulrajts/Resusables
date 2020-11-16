using KEI.Infrastructure;
using KEI.Infrastructure.Server;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using InputParameter = TCPClient.Models.InputParameter;
using InputParameterCollection = TCPClient.Models.InputParameterCollection;

namespace TCPClient.ViewModels
{

    public class TCPClientWindowViewModel : BindableBase
    {

        private readonly IClient<MessageHeader> client = new TcpClient<MessageHeader>();
        private readonly IViewService _viewService;

        public TCPClientWindowViewModel(IViewService viewService)
        {
            _viewService = viewService;

            ConnectCommand = new DelegateCommand(Connect, () => !IsConnected).ObservesProperty(() => IsConnected);
            DisconnectCommand = new DelegateCommand(Disconnect, () => IsConnected).ObservesProperty(() => IsConnected);
            AddParameterCommand = new DelegateCommand(() => Inputs.Add(new InputParameter()));
            SendCommand = new DelegateCommand(Send, CanSend);

            Inputs.CollectionChanged += Inputs_CollectionChanged;

            client.ConnectionLost += Client_ConnectionLost;
            client.ResponseRecieved += Client_ResponseRecieved;
        }

        private void Client_ResponseRecieved(Stream stream)
        {
            MessageHeader header = stream.ReadHeader<MessageHeader>();

            uint responseID = header.ID;
        }

        private void Client_ConnectionLost()
        {
            RaisePropertyChanged(nameof(IsConnected));
        }

        ~TCPClientWindowViewModel()
        {
            Inputs.CollectionChanged -= Inputs_CollectionChanged;

            foreach (var input in Inputs)
            {
                input.PropertyChanged -= Input_PropertyChanged;
            }
        }

        private string ipAddress = "127.0.0.1";
        public string IPAddress
        {
            get { return ipAddress; }
            set { SetProperty(ref ipAddress, value); }
        }

        private int port = 8000;
        public int Port
        {
            get { return port; }
            set { SetProperty(ref port, value); }
        }


        private uint commandID;
        public uint CommandID
        {
            get { return commandID; }
            set { SetProperty(ref commandID, value); }
        }

        public bool IsConnected => client.IsConnected;

        public InputParameterCollection Inputs { get; set; } = new InputParameterCollection();
        public ObservableCollection<string> TransferredBytesCollection { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> TransferredMessagesCollection { get; set; } = new ObservableCollection<string>();

        public DelegateCommand ConnectCommand { get; }
        public DelegateCommand DisconnectCommand { get; }
        public DelegateCommand AddParameterCommand { get; }
        public DelegateCommand SendCommand { get; }

        private void Connect()
        {
            if(client.Connect(IPAddress, Port) == false)
            {
                _viewService.Error("Unable to connect to server");
            }

            RaisePropertyChanged(nameof(IsConnected));
        }

        private void Disconnect()
        {
            client.SendMessage(BitConverter.GetBytes(Commander.DISCONNECT_COMMAND));
            client.Disconnect();
            RaisePropertyChanged(nameof(IsConnected));
        }

        private void Send()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);

            writer.Write(commandID);
            writer.Write(Inputs.MessageLength);
            Inputs.WriteBytes(stream);

            byte[] message = stream.ToArray();

            try
            {
                client.SendMessage(message);

                TransferredBytesCollection.Add($"Sent({message.Length}) : {BitConverter.ToString(message)}");

                TransferredMessagesCollection.Add($"Command ({CommandID}) => {Inputs}");
            }
            catch { }

        }

        private bool CanSend()
        {
            return Inputs.IsValid() && IsConnected;
        }

        private void Inputs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (var input in e.NewItems.Cast<InputParameter>())
                {
                    input.PropertyChanged += Input_PropertyChanged;
                }

                SendCommand.RaiseCanExecuteChanged();
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (var input in e.OldItems.Cast<InputParameter>())
                {
                    input.PropertyChanged -= Input_PropertyChanged;
                }
            }
        }

        private void Input_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SendCommand.RaiseCanExecuteChanged();
        }
    }
}
