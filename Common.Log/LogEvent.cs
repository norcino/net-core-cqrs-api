using Microsoft.Extensions.Logging;

namespace Common.Log
{
    public static class LogEvent
    {
        static LogEvent()
        {
            QueryHandlying = new EventId(1, "QueryHandlying");
            CommandHandlying = new EventId(2, "QueryHandlying");
        }

        public static EventId QueryHandlying { get; }
        public static EventId CommandHandlying { get; }
    }
}
