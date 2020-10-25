using KEI.Infrastructure.Server;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Input;

namespace TCPIPClient.ViewModels
{
    public class TCPClientWindowViewModel : BindableBase
    {
        private Socket _client;

        public TCPClientWindowViewModel()
        {
            ConnectCommand = new DelegateCommand(Connect, () => !IsConnected).ObservesProperty(() => IsConnected);
            DisconnectCommand = new DelegateCommand(Disconnect, () => IsConnected).ObservesProperty(() => IsConnected);
            AddParameterCommand = new DelegateCommand(() => Inputs.Add(new Models.InputParameter()));
            SendCommand = new DelegateCommand(Send, CanSend);

            Inputs.CollectionChanged += Inputs_CollectionChanged;

        }

        ~TCPClientWindowViewModel()
        {
            Inputs.CollectionChanged -= Inputs_CollectionChanged;

            foreach (var input in Inputs)
            {
                input.PropertyChanged -= Input_PropertyChanged;
            }
        }

        private void Inputs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (var input in e.NewItems.Cast<Models.InputParameter>())
                {
                    input.PropertyChanged += Input_PropertyChanged;
                }

                SendCommand.RaiseCanExecuteChanged();
            }
            else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (var input in e.OldItems.Cast<Models.InputParameter>())
                {
                    input.PropertyChanged -= Input_PropertyChanged;
                }
            }
        }

        private void Input_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SendCommand.RaiseCanExecuteChanged();
        }

        private void Send()
        {
            var dataByteBuilder = BufferBuilder.Create();

            foreach (var input in Inputs)
            {
                dataByteBuilder.InsertBytes(input.GetBytes());
            }

            var dataBytes = dataByteBuilder.GetBytes();

            var commandBytes = BufferBuilder.Combine(BitConverter.GetBytes(CommandID),
                BitConverter.GetBytes((uint)dataBytes.Length), dataBytes);


            try
            {
                _client.Send(commandBytes);

                TransferredBytesCollection.Add($"Sent({commandBytes.Length}) : {BitConverter.ToString(commandBytes)}");

                TransferredMessagesCollection.Add($"Command ({CommandID}) => {string.Join(" ", Inputs.Select(x => string.Format("\"{0}\"", x.Value)))}");
            }
            catch { }

        }

        private bool CanSend()
        {
            return Inputs.Any(x => x.IsValid() == false) == false && _client != null;
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

        private bool isConnected;
        public bool IsConnected
        {
            get { return isConnected; }
            set { SetProperty(ref isConnected, value, () => SendCommand.RaiseCanExecuteChanged()); }
        }

        public ObservableCollection<Models.InputParameter> Inputs { get; set; } = new ObservableCollection<Models.InputParameter>();
        public ObservableCollection<string> TransferredBytesCollection { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> TransferredMessagesCollection { get; set; } = new ObservableCollection<string>();
        
        public DelegateCommand ConnectCommand { get; }
        public DelegateCommand DisconnectCommand { get; }
        public DelegateCommand AddParameterCommand { get; }
        public DelegateCommand SendCommand { get; }

        private void Connect()
        {
            IPAddress default_ip;
            if (System.Net.IPAddress.TryParse(ipAddress, out default_ip))
            {
                _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    SendBufferSize = 500000,
                    NoDelay = true
                };

                IPEndPoint ipLocal = new IPEndPoint(default_ip, port);

                try
                {
                    _client.Connect(ipLocal);
                    IsConnected = true;
                }
                catch
                {
                    _client = null;
                }
            }
        }

        private void Disconnect()
        {
            if(_client != null)
            {
                _client.Disconnect(false);
                _client.Shutdown(SocketShutdown.Both);
                _client = null;
                IsConnected = false;
            }
        }
    }
}
