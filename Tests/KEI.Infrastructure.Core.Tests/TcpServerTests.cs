using KEI.Infrastructure.Core.Tests.TestData;
using KEI.Infrastructure.Server;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Xunit;

namespace KEI.Infrastructure.Core.Tests
{
    public class TcpServerTests
    {
        [Fact]
        public void TcpServer_ClientCanConnect()
        {
            /// Function to Send connection request and wait for Min(1s, time to connect)
            void ConnectionRequest(TcpServerProxy server, TcpClient client, IPAddress address)
            {
                client.Connect(new IPEndPoint(address, TcpServerProxy.PORT));
                server.WaitConnection(1000);
            }

            var server = new TcpServerProxy();

            using TcpClient client = new TcpClient(AddressFamily.InterNetwork);

            IPAddress address = IPAddress.Parse("127.0.0.1");

            // No clients connected intially
            Assert.False(server.IsConnected);

            // Assert client connected
            Assert.PropertyChanged(server, "IsConnected", () => ConnectionRequest(server, client, address));

            server.StopServer();
            server.WaitDisconnection(1000);
        }

        [Fact]
        public void TcpServer_CanRecieveCommands()
        {
            /// Send Message and wait
            void SendMessage(TcpServerProxy server, TcpClient client)
            {
                using var stream = new MemoryStream();

                const string MESSAGE = "Hello World!";
                MessageBody messageBody = new TestMessageBody
                {
                    MyProperty1 = 14,
                    MyProperty2 = 1.14,
                    MyProperty3 = 3.14f,
                    MyProperty4 = MESSAGE
                };

                TcpMessage<MessageHeader> msg = new TcpMessage<MessageHeader>(messageBody, (uint)100);

                msg.WriteBytes(stream);

                NetworkStream network = client.GetStream();

                network.Write(stream.ToArray());

                server.WaitCommand(1000);
            }

            var server = new TcpServerProxy();

            using TcpClient client = new TcpClient(AddressFamily.InterNetwork);

            IPAddress address = IPAddress.Parse("127.0.0.1");

            client.Connect(new IPEndPoint(address, TcpServerProxy.PORT));

            // Assert Message recieved.
            Assert.PropertyChanged(server, "CommandRecieved", () => SendMessage(server, client));

            server.StopServer();
            server.WaitDisconnection(1000);
            server.StopServer();
        }

        [Fact]
        private void TcpServer_CanParseMessageFromNetwork()
        {
            const string MESSAGE = "Hello World!";

            // Send message and wait
            void SendMessage(TcpServerProxy server, TcpClient client)
            {
                using var stream = new MemoryStream();

                MessageBody messageBody = new TestMessageBody
                {
                    MyProperty1 = 14,
                    MyProperty2 = 1.14,
                    MyProperty3 = 3.14f,
                    MyProperty4 = MESSAGE
                };

                TcpMessage<MessageHeader> msg = new TcpMessage<MessageHeader>(messageBody, (uint)100);

                msg.WriteBytes(stream);

                NetworkStream network = client.GetStream();

                network.Write(stream.ToArray());

                server.WaitCommand(1000);
            }

            var server = new TcpServerProxy();

            using TcpClient client = new TcpClient(AddressFamily.InterNetwork);

            IPAddress address = IPAddress.Parse("127.0.0.1");

            client.Connect(new IPEndPoint(address, TcpServerProxy.PORT));

            SendMessage(server, client);

            Assert.NotNull(server.LastHeader);
            Assert.NotNull(server.LastBody);

            MessageHeader header = (MessageHeader)server.LastHeader;

            // Assert we have the correct ID
            Assert.Equal((uint)100, header.ID);

            var bodyBytes = server.LastBody.ToArray();

            /// The length of stream can be greater than <see cref="IMessageHeader.MessageLength"/>
            /// because we're reading in chunks for size <see cref="TcpServer{THeader}.RecieveBufferSize"/>
            /// so the remaining bytes after <see cref="IMessageHeader.MessageLength"/> will be 0s
            Assert.True(bodyBytes.Length >= header.MessageLength);

            TestMessageBody recievedBody = server.LastBody.ReadBody<TestMessageBody>();

            // Assert we have the correct body
            Assert.Equal((uint)14, recievedBody.MyProperty1);
            Assert.Equal(1.14, recievedBody.MyProperty2);
            Assert.Equal(3.14f, recievedBody.MyProperty3);
            Assert.Equal(MESSAGE, recievedBody.MyProperty4);

            server.StopServer();
            server.WaitDisconnection(1000);
            server.StopServer();
        }

        [Fact]
        private void TcpServer_StoppingServerWillNotifyClient()
        {
            var server = new TcpServerProxy();

            using TcpClient client = new TcpClient(AddressFamily.InterNetwork);

            IPAddress address = IPAddress.Parse("127.0.0.1");

            //Connect
            client.Connect(new IPEndPoint(address, TcpServerProxy.PORT));
            server.WaitConnection(1000);

            // Verify that we're connected
            Assert.True(server.IsConnected);

            // Stop server
            server.StopServer();

            // Read response
            NetworkStream network = client.GetStream();
            byte[] buffer = new byte[4];
            network.Read(buffer, 0, 4);
            uint command = BitConverter.ToUInt32(buffer);

            // Ensure that we got disconnect reponse
            Assert.Equal(Commander.DISCONNECT_COMMAND + 1, command);

            // Wait for server to update status
            server.WaitDisconnection(1000);

            // Verify if that status is correct
            Assert.False(server.IsConnected);
            client.Close();
        }


        [Fact]
        public void TcpServer_StreamExetensionsCanParseMessage()
        {
            TcpServerProxy server = new TcpServerProxy();

            using TcpClient client = new TcpClient(AddressFamily.InterNetwork);

            IPAddress address = IPAddress.Parse("127.0.0.1");

            client.Connect(new IPEndPoint(address, TcpServerProxy.PORT));

            using var messageStream = new MemoryStream();

            const string MESSAGE = "Hello World!";

            MessageBody messageBody = new TestMessageBody
            {
                MyProperty1 = 14,
                MyProperty2 = 1.14,
                MyProperty3 = 3.14f,
                MyProperty4 = MESSAGE
            };

            TcpMessage<MessageHeader> message = new TcpMessage<MessageHeader>(messageBody, (uint)100);

            message.WriteBytes(messageStream);

            messageStream.Position = 0;

            MessageHeader recievedHeader = messageStream.ReadHeader<MessageHeader>();

            Assert.Equal((uint)100, recievedHeader.ID);

            TestMessageBody recievedBody = messageStream.ReadBody<TestMessageBody>();

            Assert.Equal((uint)14, recievedBody.MyProperty1);
            Assert.Equal(1.14, recievedBody.MyProperty2);
            Assert.Equal(3.14f, recievedBody.MyProperty3);
            Assert.Equal(MESSAGE, recievedBody.MyProperty4);

            server.StopServer();
            server.WaitDisconnection(1000);
            server.StopServer();
        }


    }
}
