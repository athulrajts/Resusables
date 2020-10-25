using KEI.Infrastructure.Server;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace TCPIPClient.ViewModels
{
    enum ReceiveState
    {
        Response,
        Size,
        Data,
        Complete
    }

    public class TCPClientWindowViewModel : BindableBase
    {
        private const uint DISCONNECT_RESPONSE = 11;
        private Socket _client;
        private byte[] _incomingDataBuffer;
        private ReceiveState _currentState = ReceiveState.Response;

        private byte[] _responseBuffer = new byte[4];
        private byte[] _sizeBuffer = new byte[4];
        private byte[] _dataBuffer;
        private int _responseBytesLeft;
        private int _sizeBytesLeft;
        private int _dataBytesLeft;
        private int _totalDataBytes;

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
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (var input in e.NewItems.Cast<Models.InputParameter>())
                {
                    input.PropertyChanged += Input_PropertyChanged;
                }

                SendCommand.RaiseCanExecuteChanged();
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
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
                    WaitForData();
                }
                catch
                {
                    _client = null;
                }
            }
        }

        private void Disconnect()
        {
            if (_client != null)
            {
                _client.Send(BitConverter.GetBytes(CommandServer.DISCONNECT_COMMAND));
                _client.Disconnect(false);
                _client.Shutdown(SocketShutdown.Both);
                _client.Close();
                _client = null;
                IsConnected = false;
            }
        }

        private void WaitForData()
        {
            try
            {
                _incomingDataBuffer = new byte[4];

                _client.BeginReceive(_incomingDataBuffer, 0, _incomingDataBuffer.Length, SocketFlags.None, new AsyncCallback(OnDataReceived), null);
            }
            catch
            {

            }
        }

        private void OnDataReceived(IAsyncResult ar)
        {
            if (null != _client && _client.Connected)
            {
                int recievedBytes = 0;
                try
                {
                    //need to try this, we may be disconnected, in that case
                    //we will catch the exception & look for a new client.
                    recievedBytes = _client.EndReceive(ar);

                    //we are not alive in this situation
                    if (recievedBytes == 0)
                    {
                        Disconnect();
                    }

                    int bytesRead = 0;

                    while (bytesRead < recievedBytes && _currentState != ReceiveState.Complete)
                    {
                        switch (_currentState)
                        {
                            case ReceiveState.Response:
                                {
                                    if (recievedBytes > 0)
                                    {
                                        //we have some bytes, read all that we can
                                        int startIndex = 4 - _responseBytesLeft;
                                        int stopIndex = Math.Min(recievedBytes, 4);
                                        for (int i = startIndex; i < stopIndex; i++)
                                        {
                                            //copy the values over
                                            _responseBuffer[i] = _incomingDataBuffer[bytesRead];
                                            _responseBytesLeft--;

                                            bytesRead++;
                                        }

                                        if (_responseBytesLeft == 0)
                                        {
                                            //reset the command bytes we are looking for
                                            _responseBytesLeft = 4;

                                            if (BitConverter.ToUInt32(_responseBuffer) == DISCONNECT_RESPONSE)
                                            {
                                                _currentState = ReceiveState.Complete;
                                            }
                                            else
                                            {
                                                _currentState = ReceiveState.Size;
                                            }

                                        }
                                    }
                                }
                                break;
                            case ReceiveState.Size:
                                {
                                    int bytesLeft = recievedBytes - bytesRead;
                                    if (bytesLeft > 0)
                                    {
                                        //we have some bytes, read all that we can
                                        int startIndex = 4 - _sizeBytesLeft;
                                        int stopIndex = Math.Min(bytesLeft, 4);
                                        for (int i = startIndex; i < stopIndex; i++)
                                        {
                                            //copy the values over
                                            _sizeBuffer[i] = _incomingDataBuffer[bytesRead];
                                            _sizeBytesLeft--;

                                            bytesRead++;
                                        }

                                        if (_sizeBytesLeft == 0)
                                        {
                                            //reset the size bytes we are looking for
                                            _sizeBytesLeft = 4;
                                            //analyze the number of bytes of data we are looking for:
                                            _totalDataBytes = BitConverter.ToInt32(_sizeBuffer, 0);
                                            _dataBytesLeft = _totalDataBytes;
                                            _dataBuffer = new byte[_dataBytesLeft + 4];
                                            Buffer.BlockCopy(_sizeBuffer, 0, _dataBuffer, 0, 4);
                                            if (_dataBytesLeft > 0)
                                            {
                                                _currentState = ReceiveState.Data;
                                            }
                                            else
                                            {
                                                _currentState = ReceiveState.Complete;
                                            }
                                        }
                                    }
                                }
                                break;
                            case ReceiveState.Data:
                                {
                                    int bytesLeft = recievedBytes - bytesRead;
                                    if (bytesLeft > 0)
                                    {
                                        //we have some bytes, read all that we can
                                        int startIndex = _totalDataBytes - _dataBytesLeft + 4; //offset by 4 for the size at the beginning
                                        int stopIndex = startIndex + bytesLeft;
                                        for (int i = startIndex; i < stopIndex; i++)
                                        {
                                            //copy the values over
                                            _dataBuffer[i] = _incomingDataBuffer[bytesRead];
                                            _dataBytesLeft--;

                                            bytesRead++;
                                        }

                                        if (_dataBytesLeft == 0)
                                        {
                                            _currentState = ReceiveState.Complete;
                                        }
                                    }
                                }
                                break;
                        }
                    }

                    if (_currentState == ReceiveState.Complete)
                    {
                        //now reset looking for a new command
                        _currentState = ReceiveState.Response;

                        uint responseId = BitConverter.ToUInt32(_responseBuffer, 0);

                        var fullMsg = BufferBuilder.Combine(_responseBuffer, _sizeBuffer);

                        if(BitConverter.ToUInt32(_sizeBuffer) > 0)
                        {
                            fullMsg = BufferBuilder.Combine(fullMsg, _dataBuffer);
                        }

                        TransferredBytesCollection.Add($"Response({fullMsg.Length}) : {BitConverter.ToString(fullMsg)}");

                        if (responseId == DISCONNECT_RESPONSE)
                        {
                            _client.Disconnect(true);
                            _client = null;
                            IsConnected = false;
                        }
                        else
                        {
                            // TODO
                        }

                    }

                    WaitForData();
                }
                catch
                {
                    IsConnected = false;
                    return;
                }
            }
        }
    }
}
