using KEI.Infrastructure.Configuration;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace KEI.Infrastructure.Server
{
    public class TcpMessage<THeader>
        where THeader : IMessageHeader
    {
        private IMessageHeader messageHeader;
        private IDataContainer messageBody;

        public TcpMessage(IMessageBody body, params object[] headerArgs)
        {
            messageHeader = (IMessageHeader)Activator.CreateInstance(typeof(THeader), headerArgs);
            
            messageBody = body.ToDataContainer();

            if (messageBody != null)
            {
                messageBody.PropertyChanged += Inpc_PropertyChanged; 
            }

            RecalculateLength();
        }

        public TcpMessage(IDataContainer body, params object[] headerArgs)
        {
            messageHeader = (IMessageHeader)Activator.CreateInstance(typeof(THeader), headerArgs);

            messageBody = body;

            if (messageBody != null)
            {
                messageBody.PropertyChanged += Inpc_PropertyChanged;
            }

            RecalculateLength();
        }

        ~TcpMessage()
        {
            if (messageBody != null)
            {
                messageBody.PropertyChanged -= Inpc_PropertyChanged; 
            }
        }

        public void WriteBytes(Stream stream)
        {
            messageHeader.WriteBytes(stream);
            messageBody.WriteBytes(stream);
        }

        private void Inpc_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(messageBody.GetType().GetProperty(e.PropertyName) is PropertyInfo pi
                && pi.PropertyType == typeof(string))
            {
                RecalculateLength();
            }
        }

        private void RecalculateLength()
        {
            if (messageBody == null)
            {
                return;
            }

            messageHeader.MessageLength = 0;

            foreach (var prop in messageBody)
            {
                if (prop.Type == typeof(string))
                {
                    string value = (string)prop.Value as string;

                    messageHeader.MessageLength += (uint)value.Length;
                }
                else if (prop.Type == typeof(int) ||
                    prop.Type == typeof(uint) ||
                    prop.Type == typeof(float))
                {
                    messageHeader.MessageLength += 4;
                }
                else if (prop.Type == typeof(double))
                {
                    messageHeader.MessageLength += 8;
                }
                else if (prop.Type == typeof(bool) ||
                    prop.Type == typeof(byte))
                {
                    messageHeader.MessageLength += 1;
                }
            }
        }
    }
}
