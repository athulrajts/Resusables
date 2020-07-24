﻿using KEI.Infrastructure.Helpers;
using System;
using System.IO;

namespace KEI.Infrastructure.Logging
{
    public class ConsoleAppender : BaseAppender
    {
        protected override void ProcessLogInternal(LogEvent msg)
        {
            Console.ForegroundColor = msg.Level switch
            {
                LogLevel.Debug => ConsoleColor.White,
                LogLevel.Info => ConsoleColor.Green,
                LogLevel.Warn => ConsoleColor.DarkYellow,
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Fatal => ConsoleColor.DarkRed,
                _ => ConsoleColor.Gray
            };

            string log = $"[ {msg.Level.GetEnumDescription()} ] {msg.Time:yyyy-MM-dd HH:mm:ss,ffff} {msg.Name} {Path.GetFileName(msg.FileName)} {msg.Message}";

            Console.WriteLine(log);
        }
    }
}
