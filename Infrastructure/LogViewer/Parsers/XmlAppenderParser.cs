using KEI.Infrastructure.Logging;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using KEI.Infrastructure;

namespace LogViewer.Parsers
{
    public class XmlAppenderParser : ILogParser
    {
        public IEnumerable<LogEvent> Parse(string fileName)
        {
            var result = new List<LogEvent>();

            if(File.Exists(fileName) == false)
            {
                return result;
            }

            using var reader = XmlReader.Create(fileName, new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment });

            while(reader.ReadToFollowing(nameof(LogEvent)))
            {
                result.Add(reader.ReadObjectXML<LogEvent>());
            }

            return result;
        }
    }
}
