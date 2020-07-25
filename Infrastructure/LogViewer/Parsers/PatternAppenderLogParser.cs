using KEI.Infrastructure.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using KEI.Infrastructure.Helpers;
using System.Linq;
using System.Globalization;

namespace LogViewer.Parsers
{
    public class PatternAppenderLogParser : ILogParser
    {
        public PatternAppenderLogParser()
        {
            Pattern = PatternAppender.DEFAULT_PATTERN;
        }

        private string pattern;
        public string Pattern
        {
            get { return pattern; }
            set
            {
                if (pattern == value)
                    return;

                pattern = value;

                LoadStructure();
            }
        }

        private List<LogToken> _logStructure = new List<LogToken>();

        private void LoadStructure()
        {
            _logStructure.Clear();
            if (PatternAppender.TokenRegex.Matches(Pattern) is MatchCollection m)
            {
                for (int i = 0; i < m.Count; i++)
                {
                    _logStructure.Add(PatternAppender.GetLogToken(m[i].Groups[1].Value));
                }
            }
        }


        public IEnumerable<LogEvent> Parse(string fileName)
        {
            var result = new List<LogEvent>();

            if (File.Exists(fileName) == false)
            {
                return result;
            }

            var logs = File.ReadAllText(fileName).Split(PatternAppender.LINE_BREAK);

            if (logs.Count() < 2)
            {
                return result;
            }

            if (logs[0].StartsWith(PatternAppender.DELIMITER))
            {
                Pattern = logs[0].Replace(PatternAppender.DELIMITER.ToString(), " ").Trim();
                logs = logs.Skip(1).ToArray();
            }


            foreach (var log in logs)
            {
                var tokens = log.Trim().Split(PatternAppender.DELIMITER);

                if (tokens.Length != _logStructure.Count)
                    continue;

                var logEvt = new LogEvent();


                for (int i = 0; i < tokens.Length; i++)
                {
                    switch (_logStructure[i])
                    {
                        case LogToken.Message:
                            var components = tokens[i].Split(PatternAppender.EXECEPTION_SEPARATOR);
                            logEvt.Message = components[0].Trim();
                            if (components.Length == 3)
                            {
                                logEvt.Exception = components[1].Replace(PatternAppender.EXCEPTION_STRING, string.Empty).Trim();
                                logEvt.StackTrace = components[2].Replace(PatternAppender.STACKTRACE_STRING, string.Empty).Trim();
                            }
                            break;
                        case LogToken.Level:
                            logEvt.Level = GetLogLevelFromString(tokens[i].Trim());
                            break;
                        case LogToken.DateTime:
                            logEvt.Time = DateTime.ParseExact(tokens[i].Trim(), PatternAppender.DATE_TIME_FORMAT, CultureInfo.InvariantCulture);
                            break;
                        case LogToken.LineNumber:
                            logEvt.LineNumber = int.Parse(tokens[i].Trim().Replace(PatternAppender.LINE_NUMBER_PREFIX, string.Empty).Trim());
                            break;
                        case LogToken.FileName:
                            logEvt.FileName = tokens[i].Trim();
                            break;
                        case LogToken.MethodName:
                            logEvt.MethodName = tokens[i].Trim();
                            break;
                    }
                }

                result.Add(logEvt);
            }

            return result;
        }

        public static LogLevel GetLogLevelFromString(string token)
        {
            var values = new List<int>((int[])Enum.GetValues(typeof(LogLevel)));

            return (LogLevel)values.FirstOrDefault(x => token.Contains(((LogLevel)x).GetEnumDescription()));
        }
    }
}
