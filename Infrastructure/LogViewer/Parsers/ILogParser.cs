using KEI.Infrastructure.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogViewer.Parsers
{
    public interface ILogParser
    {
        IEnumerable<LogEvent> Parse(string fileName);
    }
}
