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
        FileName
    }

    [Flags]
    public enum LogLevel
    {
        [Description("DBG")]
        Debug = 1,

        [Description("INF")]
        Info = 2,

        [Description("WRN")]
        Warn = 4,

        [Description("ERR")]
        Error = 8,

        [Description("FTL")]
        Fatal = 16
    }
}
