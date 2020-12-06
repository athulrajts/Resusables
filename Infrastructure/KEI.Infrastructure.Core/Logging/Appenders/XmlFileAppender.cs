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
        /// <summary>
        /// Serialize <see cref="LogEvent"/> directly
        /// It'll be easier to parse and recreate the <see cref="LogEvent"/> and
        /// use it on logviewer, but one trade if that it's hard to read on a text editor
        /// </summary>
        /// <param name="msg"></param>
        protected override void WriteToFile(LogEvent msg)
        {
            try
            {
                using var sw = new StreamWriter(FilePath, append: true) { AutoFlush = true };
                sw.WriteLine(XmlHelper.SerializeToString(msg));
            }
            catch (Exception) { }
        }
    }
}
