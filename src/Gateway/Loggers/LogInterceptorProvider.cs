namespace Gateway.Loggers;

public class LogInterceptorProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new LogInterceptorLogger(categoryName);
    }

    public void Dispose()
    {
    }

    private class LogInterceptorLogger : ILogger
    {
        private readonly string _categoryName;

        public LogInterceptorLogger(string categoryName)
        {
            _categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public async void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = formatter(state, exception);

            GatewayApp.LoggerQueue.Enqueue($"[{DateTime.Now}] [{_categoryName}] [{logLevel}] {message}");
        }
    }
}
