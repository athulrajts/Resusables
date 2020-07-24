using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace KEI.Infrastructure.Logging
{
    public class XmlFileAppender : RollingFileAppender
    {

        protected override void WriteToFile(LogEvent msg)
        {
            try
            {
                using var sw = new StreamWriter(FilePath, append: true) { AutoFlush = true };
                sw.WriteLine(XmlHelper.Serialize(msg));
            }
            catch (Exception) { }
        }
    }
}
