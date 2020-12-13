using System;
using KEI.Infrastructure.Helpers;

namespace KEI.Infrastructure.Logging
{
    //TODO : Add pattern logging
    public class ColorConsoleAppender : BaseAppender
    {
        protected override void ProcessLogInternal(LogEvent msg)
        {
            Console.ForegroundColor = msg.Level switch
            {
                LogLevel.Debug => ConsoleColor.White,
                LogLevel.Information => ConsoleColor.Green,
                LogLevel.Warning => ConsoleColor.DarkYellow,
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Critical => ConsoleColor.DarkRed,
                _ => ConsoleColor.Gray
            };

            string log = $"[ {msg.Level.GetEnumDescription()} ]\t{msg.Time:yyyy-MM-dd HH:mm:ss,ffff}\tLN-{msg.LineNumber}\t{msg.FileName}\t{msg.Message}{msg.Exception}{msg.StackTrace}";

            Console.WriteLine(log);

            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
