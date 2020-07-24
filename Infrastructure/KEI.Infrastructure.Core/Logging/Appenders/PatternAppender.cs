using KEI.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace KEI.Infrastructure.Logging
{
    public class PatternAppender : RollingFileAppender
    {
        public const string DEFAULT_PATTERN = @"$(l) $(dt) $(f) $(m) $(ln) $(msg)";
        public const char DELIMITER = '\u180e';
        public const char LINE_BREAK = '\u200b';
        public const string DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss,ffff";
        public const string LINE_NUMBER_PREFIX = "LN-";

        public static readonly Regex TokenRegex = new Regex(@"\$\((\w+)\)");
        private readonly List<LogToken> _logStructure = new List<LogToken>();

        public PatternAppender()
        {
            LoadLayoutTokens();
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

                LoadLayoutTokens();
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
                "msg" => LogToken.Message,
                "m" => LogToken.MethodName,
                "ln" => LogToken.LineNumber,
                "dt" => LogToken.DateTime,
                "l" => LogToken.Level,
                "sn" => LogToken.MachineName,
                "f" => LogToken.FileName,
                _ => LogToken.Invalid
            };
        }

        private void LoadLayoutTokens()
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
