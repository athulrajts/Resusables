using KEI.Infrastructure.Logging;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace KEI.Infrastructure.Server
{
    enum ReceiveState
    {
        Command = 0, //currently looking for a command (4 bytes)
        Size = 1,    //currently looking for a size (4 bytes)
        Data = 2,    //waiting on data (variable bytes)
        Complete = 3 //the full command and data is complete, execute!
    };

    static class SocketExtensions
    {
        public static void DisconnectAndCleanup(this Socket s)
        {
            if (s != null)
            {
                //now we need to disconnect the client
                try
                {
                    if (s.Connected)
                    {
                        s.Disconnect(false);
                        s.Shutdown(SocketShutdown.Both);
                    }
                    s.Close();
                    s = null;
                }
                catch
                {
                    s = null;
                }
            }
        }
    }

    public class CommandServer : BindableBase, IServer
    {
        public const uint DISCONNECT_COMMAND = 10;

        #region Private Fields

        private Socket _listener;
        private Socket _client;
        private readonly Mutex startMutex = new Mutex();
        private ReceiveState _currentState;
        private IPAddress _currentIPAddress;

        private byte[] _commandBuffer = new byte[4];
        private byte[] _sizeBuffer = new byte[4];
        private byte[] _dataBuffer;
        private byte[] _incomingDataBuffer;
        private int _sendBufferSize = 500000;

        private int _commandBytesLeft;
        private int _sizeBytesLeft;
        private int _dataBytesLeft;
        private int _totalDataBytes;

        private Commander _commander;
        private readonly IViewService _viewService;

        #endregion

        #region Constructor

        public CommandServer(Commander commander, IViewService viewService)
        {
            _viewService = viewService;
            _commander = commander;
        }

        #endregion


        #region IServer Members

        public bool IsConnected => _client == null ? false : _client.Connected;

        public bool IsRunning => _listener != null;

        public bool StartServer(string ipAddress, ushort port)
        {
            bool isStarted = false;

            try
            {
                if (IsRunning == false && startMutex.WaitOne())
                {
                    _currentState = ReceiveState.Command;
                    _dataBytesLeft = 0;
                    _commandBytesLeft = 4;
                    _sizeBytesLeft = 4;
                    
                    if (string.IsNullOrEmpty(ipAddress))
                    {
                        ipAddress = "127.0.0.1";
                    }

                    IPAddress default_ip;
                    if (IPAddress.TryParse(ipAddress, out default_ip))
                    {
                        _currentIPAddress = default_ip;
                        _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                        {
                            SendBufferSize = _sendBufferSize,
                            NoDelay = true
                        };

                        IPEndPoint ipLocal = new IPEndPoint(default_ip, port);

                        _listener.Bind(ipLocal);
                        _listener.Listen(1);

                        _listener.BeginAccept(new AsyncCallback(OnClientConnected), null);

                        //IsConnected = true;
                        
                        isStarted = true;
                    }

                    startMutex.ReleaseMutex();
                }
            }
            catch (Exception e)
            {
                //clean up the listener if we did anything to it...
                if (null != _listener && _listener.Connected)
                {
                    _listener.Disconnect(false);
                    _listener.Shutdown(SocketShutdown.Both);
                    _listener.Close();
                    _listener = null;
                }

                //return false because we broke something
                isStarted = false;

                //catch any exception here, and just return false that we could not start the server
                Logger.Error("Exception Thrown during startup of TCP/IP Server", e);
            }

            RaisePropertyChanged(nameof(IsRunning));

            return isStarted;
        }

        public bool StopServer()
        {
            bool retValue = true;
            //shutdown the client & then the server
            _commander.DisconnectServerFromClient();
            _client.DisconnectAndCleanup();
            _listener.DisconnectAndCleanup();

            RaisePropertyChanged(nameof(IsConnected));
            RaisePropertyChanged(nameof(IsRunning));

            return retValue;
        }

        #endregion

        #region Private Functions

        private void OnClientConnected(IAsyncResult ar)
        {
            if (_listener != null && _listener.Connected == false)
            {
                try
                {
                    _client = _listener.EndAccept(ar);
                    _commander.SetClient(_client);
                  
                    WaitForData();
                    
                    Logger.Info("Client connected to server");
                }
                catch(Exception ex)
                {
                    _client = null;
                    //_Commander = null;
                    _listener = null;
                    Logger.Error("Unhandled Exception", ex);
                }

                RaisePropertyChanged(nameof(IsRunning));
                RaisePropertyChanged(nameof(IsConnected));
            }
        }

        private void WaitForData()
        {
            try
            {
                _incomingDataBuffer = new byte[4];
                
                _client.BeginReceive(_incomingDataBuffer, 0, _incomingDataBuffer.Length, SocketFlags.None, new AsyncCallback(OnDataReceived), null);
            }
            catch(Exception e)
            {
                Logger.Error("Exception Thrown during WaitForData of TCP/IP Server", e);
                SetupForReconnect();

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
                        Debug.WriteLine("received 0 bytes");
                        throw new SocketException();
                    }
                }
                catch
                {
                    //client has disconnected, lets look for a new one...
                    SetupForReconnect();
                    return;
                }

                int bytesRead = 0;
                // while we have bytes to read and we are not complete
                while (bytesRead < recievedBytes && _currentState != ReceiveState.Complete)
                {
                    switch (_currentState)
                    {
                        case ReceiveState.Command:
                            {
                                //read _uiCommandBytesLeft many bytes, if the bytes left go to 0, then
                                //upgrade to Size.
                                if (recievedBytes > 0)
                                {
                                    //we have some bytes, read all that we can
                                    int startIndex = 4 - _commandBytesLeft;
                                    int stopIndex = Math.Min(recievedBytes, 4);
                                    for (int i = startIndex; i < stopIndex; i++)
                                    {
                                        //copy the values over
                                        _commandBuffer[i] = _incomingDataBuffer[bytesRead];
                                        _commandBytesLeft--;
                                        
                                        bytesRead++;
                                    }

                                    if (_commandBytesLeft == 0)
                                    {
                                        //reset the command bytes we are looking for
                                        _commandBytesLeft = 4;

                                        if(BitConverter.ToUInt32(_commandBuffer) == DISCONNECT_COMMAND)
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
                    _currentState = ReceiveState.Command;

                    uint commandID = BitConverter.ToUInt32(_commandBuffer, 0);

                    if (commandID == DISCONNECT_COMMAND)
                    {
                        Logger.Info("Service recieved diconnect command");
                        _client.Disconnect(true);
                        SetupForReconnect();
                    }
                    else
                    {
                        _commander.ExecuteCommand(commandID, _dataBuffer);
                    }

                }

                WaitForData();
            }
            else  // the client is disconnected or null
            {
                SetupForReconnect();
                return;
            }
        }


        private void SetupForReconnect()
        {
            Logger.Info("Server setting up for a new connection");
            
            //tell the commander to disconnect & then clean up
            _client.DisconnectAndCleanup();

            //begin looking for a new connection
            if (_listener != null)
            {
                try
                {
                    _listener.BeginAccept(new AsyncCallback(OnClientConnected), null);
                }
                catch
                {
                    //if the listener is null, we need to catch & recover
                }

                RaisePropertyChanged(nameof(IsConnected));
            }
        }

        #endregion

    }
}
