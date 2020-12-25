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

            messageBody = DataContainerBuilder.CreateObject("body", body);

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
                var value = prop.GetValue();
                if(value is int || value is float)
                {
                    messageHeader.MessageLength += 4;
                }
                else if(value is double)
                {
                    messageHeader.MessageLength += 8;
                }
                else if(value is bool || value is byte)
                {
                    messageHeader.MessageLength += 1;
                }
                else if(value is string)
                {
                    if(prop.GetValue() is string s)
                    {
                        messageHeader.MessageLength += (uint)s.Length;
                    }
                }
            }
        }
    }
}
