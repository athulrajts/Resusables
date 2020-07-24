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
            LoadLayoutTokens();
        }

        private string pattern = PatternAppender.DEFAULT_PATTERN;
        public string Pattern
        {
            get { return pattern; }
            set 
            {
                if (pattern == value)
                    return;

                pattern = value;

                LoadLayoutTokens();
            }
        }

        private List<LogToken> _logStructure = new List<LogToken>();

        private void LoadLayoutTokens()
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

            if(File.Exists(fileName) == false)
            {
                return result;
            }

            var logs = File.ReadAllText(fileName).Split(PatternAppender.LINE_BREAK);

            foreach (var log in logs)
            {
                var tokens = log.Trim().Split(PatternAppender.DELIMITER);

                if (tokens.Length != _logStructure.Count)
                    continue;

                var logEvt = new LogEvent();


                for (int i = 0; i < tokens.Length; i++)
                {
                    switch(_logStructure[i])
                    {
                        case LogToken.Message:
                            logEvt.Message = tokens[i].Trim();
                            break;
                        case LogToken.Level:
                            logEvt.Level = GetLogLevelFromString(tokens[i].Trim());
                            break;
                        case LogToken.DateTime:
                            logEvt.Time = DateTime.ParseExact(tokens[i].Trim(), PatternAppender.DATE_TIME_FORMAT, CultureInfo.InvariantCulture);
                            break;
                        case LogToken.LineNumber:
                            logEvt.LineNumber = int.Parse(tokens[i].Trim().Replace(PatternAppender.LINE_NUMBER_PREFIX, "").Trim());
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
            var values = new List<int>((int [])Enum.GetValues(typeof(LogLevel)));

            return (LogLevel)values.FirstOrDefault(x => token.Contains(((LogLevel)x).GetEnumDescription()));
        }
    }
}
