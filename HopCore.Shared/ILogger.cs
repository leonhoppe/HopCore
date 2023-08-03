namespace HopCore.Shared {
    public interface ILogger {
        void Debug(object message);
        void Info(object message);
        void Warning(object message);
        void Error(object message);
        void Fatal(object message);
        void Database(object message, bool newLine = true);
    }
}