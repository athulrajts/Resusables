using System;
using System.ComponentModel;

namespace KEI.Infrastructure.Logging
{
    public enum LogToken
    {
        Invalid,
        Message,
        MethodName,
        LineNumber,
        DateTime,
        Level,
        MachineName,
        FileName,
        LoggerName
    }

    [Flags]
    public enum LogLevel
    {
        [Description("NON")]
        None = 0,

        [Description("TRC")]
        Trace = 1,

        [Description("DBG")]
        Debug = 2,

        [Description("INF")]
        Information = 4,

        [Description("WRN")]
        Warning = 8,

        [Description("ERR")]
        Error = 16,

        [Description("CTL")]
        Critical = 32
    }
}
