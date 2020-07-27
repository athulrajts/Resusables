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
        public const char EXECEPTION_SEPARATOR = '\u200c';
        public const string DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss,ffff";
        public const string LINE_NUMBER_PREFIX = "LN-";
        public const string EXCEPTION_STRING = "Exception : ";
        public const string STACKTRACE_STRING = "StackTrace : ";
        public static readonly Regex TokenRegex = new Regex(@"\$(\w+)");
  
        private readonly List<LogToken> _logStructure = new List<LogToken>();

        public PatternAppender()
        {
            LoadStructure();
        }

        /// <summary>
        /// Decides the format in which log is written in to a file
        /// The pattern is only used to decided the order of things and
        /// not the spacing or indentation between item.
        /// </summary>
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

        /// <summary>
        /// Append to file as plain text
        /// </summary>
        /// <param name="msg"></param>
        protected override void WriteToFile(LogEvent msg)
        {
            try
            {
                using (var writer = fileInfo.AppendText())
                {
                    /// We're adding an Invisible characters <see cref="LINE_BREAK"/>
                    /// so that a parser can know end of a single log.
                    writer.WriteLine($"{GetLogString(msg)}{LINE_BREAK}");
                }
            }
            catch (Exception) { }
        }


        /// <summary>
        /// Write data to start of each file to identify ourself.
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteMetaDataInternal(StreamWriter writer)
        {
            writer.WriteLine($"{DELIMITER}{LayoutPattern}{LINE_BREAK}");
        }

        /// <summary>
        /// Convert <see cref="LogEvent"/> object to the formate we want.
        /// </summary>
        /// <param name="logEvent"></param>
        /// <returns></returns>
        private string GetLogString(LogEvent logEvent)
        {
            var logString = new List<string>();

            foreach (var token in _logStructure)
            {
                switch (token)
                {
                    case LogToken.Message:
                        string msg = logEvent.Message.Trim();
                        if(string.IsNullOrEmpty(logEvent.Exception) == false)
                        {
                            msg = $"{msg}\t{EXECEPTION_SEPARATOR}{EXCEPTION_STRING}{logEvent.Exception}" +
                                  $"{EXECEPTION_SEPARATOR}{STACKTRACE_STRING}{logEvent.StackTrace}";
                        }
                        logString.Add(msg);
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

            /// We're also adding an invisible character <see cref="DELIMITER"/>
            /// to separate each component in log so that a parser can correctly
            /// recreate <see cref="LogEvent"/> object given <see cref="LayoutPattern"/>
            return string.Join($"\t{DELIMITER}", logString);
        }

        /// <summary>
        /// Converted string patter to a enum one so it's easier to use
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
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
