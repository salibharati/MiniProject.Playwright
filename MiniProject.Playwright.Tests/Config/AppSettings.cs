namespace MiniProject.Playwright.Tests.Config
{
    /// <summary>
    /// Root configuration class mapped from appsettings.json
    /// </summary>
    public class AppSettings
    {
        public BrowserSettings BrowserSettings { get; set; } = new();

        public TestSettings TestSettings { get; set; } = new();

        public EnvironmentSettings EnvironmentSettings { get; set; } = new();

        public ReportSettings ReportSettings { get; set; } = new();
    }

    /// <summary>
    /// Browser Configuration
    /// </summary>
    public class BrowserSettings
    {
        /// <summary>
        /// chromium / firefox / webkit
        /// </summary>
        public string BrowserName { get; set; } = "chromium";

        /// <summary>
        /// true = Headless
        /// false = Browser visible
        /// </summary>
        public bool Headless { get; set; } = false;

        /// <summary>
        /// SlowMo in milliseconds
        /// </summary>
        public int SlowMo { get; set; } = 0;

        /// <summary>
        /// Browser timeout in milliseconds
        /// </summary>
        public int Timeout { get; set; } = 30000;
    }

    /// <summary>
    /// Test execution settings
    /// </summary>
    public class TestSettings
    {
        /// <summary>
        /// Base URL of AUT
        /// </summary>
        public string BaseUrl { get; set; } = string.Empty;

        /// <summary>
        /// Capture Screenshot on Failure
        /// </summary>
        public bool ScreenshotOnFailure { get; set; } = true;

        /// <summary>
        /// Record Video
        /// </summary>
        public bool VideoOnFailure { get; set; } = true;

        /// <summary>
        /// Save Playwright Trace
        /// </summary>
        public bool TraceOnFirstRetry { get; set; } = true;

        /// <summary>
        /// Maximum Retry Count
        /// </summary>
        public int RetryCount { get; set; } = 0;
    }

    /// <summary>
    /// Environment Configuration
    /// </summary>
    public class EnvironmentSettings
    {
        public string EnvironmentName { get; set; } = "QA";

        public string ApplicationName { get; set; } = "MiniProject";

        public string Version { get; set; } = "1.0";
    }

    /// <summary>
    /// Reporting Configuration
    /// </summary>
    public class ReportSettings
    {
        public string ReportName { get; set; } = "Playwright Automation Report";

        public string ReportTitle { get; set; } = "Execution Report";

        public string Theme { get; set; } = "Dark";

        public bool CaptureSystemInfo { get; set; } = true;
    }
}