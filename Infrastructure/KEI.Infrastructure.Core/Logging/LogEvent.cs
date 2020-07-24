using System;
using System.Xml.Serialization;

namespace KEI.Infrastructure.Logging
{
    public class LogEvent
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public LogLevel Level { get; set; }

        [XmlAttribute]
        public string Message { get; set; }

        [XmlElement(IsNullable = false)]
        public string Exception { get; set; }

        [XmlElement(IsNullable = false)]
        public string StackTrace { get; set; }

        [XmlAttribute]
        public DateTime Time { get; set; }

        [XmlAttribute]
        public int LineNumber { get; set; }

        [XmlAttribute]
        public string MachineName { get; set; }

        [XmlAttribute]
        public string MethodName { get; set; }

        [XmlAttribute]
        public string FileName { get; set; }
    }
}
