using Serilog;

namespace MiniProject.Playwright.Tests.Utilities
{
    /// <summary>
    /// Centralized Logger using Serilog.
    /// Logs are written to both Console and TestResults/Logs.
    /// </summary>
    public static class Logger
    {
        static Logger()
        {
            TestPaths.CreateDirectories();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(
                    Path.Combine(TestPaths.Logs, "Execution.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    shared: true,
                    outputTemplate:
                        "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }

        public static void Info(string message)
        {
            Log.Information(message);
        }

        public static void Warning(string message)
        {
            Log.Warning(message);
        }

        public static void Error(string message)
        {
            Log.Error(message);
        }

        public static void Error(Exception ex, string message)
        {
            Log.Error(ex, message);
        }

        public static void Debug(string message)
        {
            Log.Debug(message);
        }

        public static void Fatal(string message)
        {
            Log.Fatal(message);
        }

        public static void Close()
        {
            Log.CloseAndFlush();
        }
    }
}