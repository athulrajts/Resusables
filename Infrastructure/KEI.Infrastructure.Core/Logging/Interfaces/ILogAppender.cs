namespace KEI.Infrastructure.Logging
{
    public interface ILogAppender
    {
        void Append(LogEvent msg);
    }
}
