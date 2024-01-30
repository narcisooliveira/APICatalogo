namespace APICatalogo.Logging
{
    public class CustomerLogger : ILogger
    {
        private readonly string _loggerName;
        private readonly CustomLoggerProviderConfiguration _loggerConfig;

        public CustomerLogger(string loggerName, CustomLoggerProviderConfiguration loggerConfig)
        {
            _loggerName = loggerName;
            _loggerConfig = loggerConfig;
        }

        public IDisposable? BeginScope<TState>(TState state)
        {
            return default;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == _loggerConfig.LogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
                       Func<TState, Exception, string> formatter)
            => WriteTextMessageToFile($"{logLevel} - {eventId.Id} - {_loggerName} - {formatter(state, exception)}");
            
        private static void WriteTextMessageToFile(string message)
        {
            string path = @"C:\Users\narci\Downloads\log.txt";
            using StreamWriter streamWriter = new(path, true);
            try
            {
                streamWriter.WriteLine(message);
                streamWriter.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
