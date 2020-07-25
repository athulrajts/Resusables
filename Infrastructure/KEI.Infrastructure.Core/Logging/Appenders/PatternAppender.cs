using KEI.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace KEI.Infrastructure.Logging
{

    public static class TokenConstants
    {
        public const string MESSAGE = "msg";
        public const string DATE_TIME = "dt";
        public const string FILE = "f";
        public const string METHOD = "m";
        public const string LINE = "ln";
        public const string LEVEL = "l";
        public const string MACHINE = "mn";
        public const string LOGGER_NAME = "n";
    }

    public class PatternAppender : RollingFileAppender
    {
        public static readonly string DEFAULT_PATTERN = $@"${TokenConstants.LEVEL} ${TokenConstants.DATE_TIME} " +
                                                        $@"${TokenConstants.FILE} ${TokenConstants.METHOD} " + 
                                                        $@"${TokenConstants.LINE} ${TokenConstants.MESSAGE}";
        public const char DELIMITER = '\u180e';
        public const char LINE_BREAK = '\u200b';
        public const string DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss,ffff";
        public const string LINE_NUMBER_PREFIX = "LN-";

        public static readonly Regex TokenRegex = new Regex(@"\$(\w+)");
        private readonly List<LogToken> _logStructure = new List<LogToken>();

        public PatternAppender()
        {
            LoadStructure();
        }

        private string layoutPattern = DEFAULT_PATTERN;
        public string LayoutPattern
        {
            get => layoutPattern;
            set
            {
                if (layoutPattern == value)
                {
                    return;
                }

                layoutPattern = value;

                LoadStructure();
            }
        }

        protected override void WriteToFile(LogEvent msg)
        {
            try
            {
                using (var writer = fileInfo.AppendText())
                {
                    writer.WriteLine($"{GetLogString(msg)}{LINE_BREAK}");
                }
            }
            catch (Exception) { }
        }

        protected override void WriteMetaDataInternal(StreamWriter writer)
        {
            writer.WriteLine($"{DELIMITER}{LayoutPattern}{LINE_BREAK}");
        }

        private string GetLogString(LogEvent logEvent)
        {
            var logString = new List<string>();

            foreach (var token in _logStructure)
            {
                switch (token)
                {
                    case LogToken.Message:
                        logString.Add(logEvent.Message.Trim());
                        break;
                    case LogToken.DateTime:
                        logString.Add($"{logEvent.Time.ToString(DATE_TIME_FORMAT)} ");
                        break;
                    case LogToken.LineNumber:
                        logString.Add($"{LINE_NUMBER_PREFIX}{logEvent.LineNumber}");
                        break;
                    case LogToken.MethodName:
                        logString.Add(logEvent.MethodName.ToString().PadRight(15));
                        break;
                    case LogToken.Level:
                        logString.Add($"[ {logEvent.Level.GetEnumDescription()} ]");
                        break;
                    case LogToken.FileName:
                        logString.Add(logEvent.FileName.PadRight(25));
                        break;
                }
            }

            return string.Join($"\t{DELIMITER}", logString);
        }

        public static LogToken GetLogToken(string token)
        {
            return token switch
            {
                TokenConstants.MESSAGE => LogToken.Message,
                TokenConstants.METHOD => LogToken.MethodName,
                TokenConstants.LINE => LogToken.LineNumber,
                TokenConstants.DATE_TIME => LogToken.DateTime,
                TokenConstants.LEVEL => LogToken.Level,
                TokenConstants.MACHINE => LogToken.MachineName,
                TokenConstants.FILE => LogToken.FileName,
                TokenConstants.LOGGER_NAME => LogToken.LoggerName,
                _ => LogToken.Invalid
            };
        }

        private void LoadStructure()
        {
            _logStructure.Clear();
            if (TokenRegex.Matches(LayoutPattern) is MatchCollection m)
            {
                for (int i = 0; i < m.Count; i++)
                {
                    _logStructure.Add(GetLogToken(m[i].Groups[1].Value));
                }
            }
        }
    }
}
