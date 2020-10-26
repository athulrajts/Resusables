using System.IO;

namespace KEI.Infrastructure.Server
{
    public static class MessageContractExtensions
    {
        public static T ReadHeader<T>(this Stream s)
            where T : IMessageHeader, new()
        {
            var header = new T();
            header.ReadBytes(s);
            return header;
        }

        public static T ReadBody<T>(this Stream s)
            where T : IMessageBody, new()
        {
            var message = new T();
            message.ReadBytes(s);
            return message;
        }
    }
}
