using System;
using System.IO;
using System.Reflection;
using System.Xml;
using log4net;

namespace KEI.Infrastructure
{
    public class Log4NetConfigurator : ILogConfigurator
    {
        #region ILogConfigurator Members

        public int NumOfLogFiles { get; set; }
        public int LogFileSize { get; set; }
        public bool IncludeDebugOutput { get; set; }
        private string m_sLogFilename = ".\\Logs\\log.xml";
        private bool m_bLogFilenameSet = false;
        public string LogFilename
        {
            get
            {
                return m_sLogFilename;
            }
            set
            {
                if (value != m_sLogFilename)
                {
                    m_sLogFilename = value;
                    m_bLogFilenameSet = true;
                    StoreConfiguration();
                    log4net.Config.XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetEntryAssembly()), new FileInfo(GetConfigPath()));
                }
            }
        }

        private string GetConfigPath()
        {
            return @"Configs\log4net.config";
        }

        public void ReadConfiguration()
        {
            string strConfigLocation = GetConfigPath();

            if (!File.Exists(strConfigLocation))
            {
                System.Diagnostics.Trace.WriteLine($"Log config {strConfigLocation} does not exist");
            }

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(strConfigLocation);

                var fileSizeNode = xmlDoc.DocumentElement.SelectSingleNode("appender/maximumFileSize");

                if (fileSizeNode != null)
                {
                    var valueAttribute = fileSizeNode.Attributes["value"];
                    if (valueAttribute != null)
                    {
                        string value = valueAttribute.Value;
                        string strFileSize = value.Substring(0, value.Length - 2).ToUpper();
                        string strFileUnits = value.Substring(value.Length - 2, 2);

                        int multiplier = 1;

                        if (strFileUnits == "MB")
                        {
                            multiplier = 1000;
                        }
                        else if (strFileUnits == "GB")
                        {
                            multiplier = 1000000;
                        }

                        int iKilobyteSize;
                        if (int.TryParse(strFileSize, out iKilobyteSize))
                        {
                            LogFileSize = iKilobyteSize * multiplier;
                        }
                    }
                }

                var numFilesNode = xmlDoc.DocumentElement.SelectSingleNode("appender/maxSizeRollBackups");

                if (numFilesNode != null)
                {
                    var valueAttribute = numFilesNode.Attributes["value"];
                    if (valueAttribute != null)
                    {
                        int backups;
                        if (int.TryParse(valueAttribute.Value, out backups))
                        {
                            NumOfLogFiles = backups + 1;
                        }
                    }
                }

                var outputLevelNode = xmlDoc.DocumentElement.SelectSingleNode("root/level");

                if (outputLevelNode != null)
                {
                    var valueAttribute = outputLevelNode.Attributes["value"];
                    if (valueAttribute != null)
                    {
                        string strValue = valueAttribute.Value.ToLower();
                        IncludeDebugOutput = strValue == "all" || strValue == "debug";
                    }
                }

                var filenameNode = xmlDoc.DocumentElement.SelectSingleNode("appender/file");

                if (filenameNode != null)
                {
                    var valueFile = outputLevelNode.Attributes["value"];
                    if (valueFile != null)
                    {
                        // set member variable here to not trigger a write
                        m_sLogFilename = valueFile.Value;
                    }
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine($"Error reading logging configuration file ({strConfigLocation}) Exception : {e}");
            }
        }

        public void StoreConfiguration()
        {
            string strConfigLocation = GetConfigPath();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(strConfigLocation);

            var fileSizeNode = xmlDoc.DocumentElement.SelectSingleNode("appender/maximumFileSize");

            if (fileSizeNode != null)
            {
                var valueAttribute = fileSizeNode.Attributes["value"];
                if (valueAttribute != null)
                {
                    valueAttribute.Value = LogFileSize + "KB";
                }
            }

            var numFilesNode = xmlDoc.DocumentElement.SelectSingleNode("appender/maxSizeRollBackups");

            if (numFilesNode != null)
            {
                var valueAttribute = numFilesNode.Attributes["value"];
                if (valueAttribute != null)
                {
                    valueAttribute.Value = "" + (NumOfLogFiles - 1);
                }
            }

            var outputLevelNode = xmlDoc.DocumentElement.SelectSingleNode("root/level");

            if (outputLevelNode != null)
            {
                var valueAttribute = outputLevelNode.Attributes["value"];
                if (valueAttribute != null)
                {
                    valueAttribute.Value = IncludeDebugOutput ? "ALL" : "INFO";
                }
            }

            if (m_bLogFilenameSet)
            {
                var FileNode = xmlDoc.DocumentElement.SelectSingleNode("appender/file");

                if (FileNode != null)
                {
                    var valueFile = FileNode.Attributes["value"];
                    if (valueFile != null)
                    {
                        valueFile.Value = m_sLogFilename;
                    }
                }
            }
            xmlDoc.Save(strConfigLocation);
        }
    }

    #endregion
}

