using KEI.Infrastructure.Helpers;

namespace KEI.Infrastructure.Logging
{
    public class TraceAppender : BaseAppender
    {
        protected override void ProcessLogInternal(LogEvent logEvent)
        {
            string log = $"[ {logEvent.Level.GetEnumDescription()} ] LN-{logEvent.LineNumber} | {logEvent.FileName} | {logEvent.MethodName} | {logEvent.Message} | {logEvent.Exception} {logEvent.StackTrace}";

            switch(logEvent.Level)
            {
                case LogLevel.Debug:
                case LogLevel.Trace:
                    System.Diagnostics.Trace.WriteLine(log);
                    break;
                case LogLevel.Information:
                    System.Diagnostics.Trace.TraceInformation(log);
                    break;
                case LogLevel.Warning:
                    System.Diagnostics.Trace.TraceWarning(log);
                    break;
                case LogLevel.Error:
                case LogLevel.Critical:
                    System.Diagnostics.Trace.TraceError(log);
                    break;
                default:
                    break;
            }

        }
    }
}
