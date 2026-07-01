using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Config;

namespace MiniProject.Playwright.Tests.Utilities
{
    /// <summary>
    /// Manages the lifecycle of the Extent Report.
    /// Supports both Local execution and Azure DevOps Pipelines.
    /// </summary>
    public static class ExtentReportManager
    {
        private static readonly Lazy<ExtentReports> _extent =
            new(CreateReport);

        /// <summary>
        /// Singleton instance of ExtentReports.
        /// </summary>
        public static ExtentReports Instance => _extent.Value;

        /// <summary>
        /// Creates and configures the Extent Report.
        /// </summary>
        private static ExtentReports CreateReport()
        {
            // Ensure folders exist
            TestPaths.CreateDirectories();

            string reportFile = Path.Combine(
                TestPaths.Reports,
                "ExtentReport.html");

            var sparkReporter = new ExtentSparkReporter(reportFile);

            sparkReporter.Config.Theme = Theme.Dark;
            sparkReporter.Config.DocumentTitle = "Playwright .NET Automation Report";
            sparkReporter.Config.ReportName = "MiniProject - Playwright Automation Execution Report";
            sparkReporter.Config.Encoding = "UTF-8";
            sparkReporter.Config.TimeStampFormat = "dd-MMM-yyyy HH:mm:ss";

            var extent = new ExtentReports();

            extent.AttachReporter(sparkReporter);

            // ===============================
            // Framework Information
            // ===============================

            extent.AddSystemInfo("Framework", ".NET 8");

            extent.AddSystemInfo("Automation Tool", "Microsoft Playwright");

            extent.AddSystemInfo("Test Framework", "NUnit");

            extent.AddSystemInfo("Report", "Extent Reports");

            extent.AddSystemInfo("Operating System",
                Environment.OSVersion.ToString());

            extent.AddSystemInfo(".NET Version",
                Environment.Version.ToString());

            extent.AddSystemInfo("Machine",
                Environment.MachineName);

            extent.AddSystemInfo("User",
                Environment.UserName);

            extent.AddSystemInfo("Execution Time",
                DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss"));

            // Azure DevOps Information (if available)

            AddIfExists(extent,
                "Azure Build Number",
                "BUILD_BUILDNUMBER");

            AddIfExists(extent,
                "Azure Build Id",
                "BUILD_BUILDID");

            AddIfExists(extent,
                "Build Definition",
                "BUILD_DEFINITIONNAME");

            AddIfExists(extent,
                "Source Branch",
                "BUILD_SOURCEBRANCHNAME");

            AddIfExists(extent,
                "Repository",
                "BUILD_REPOSITORY_NAME");

            AddIfExists(extent,
                "Requested By",
                "BUILD_REQUESTEDFOR");

            return extent;
        }

        /// <summary>
        /// Creates a new Extent Test.
        /// </summary>
        public static ExtentTest CreateTest(string testName)
        {
            return Instance.CreateTest(testName);
        }

        /// <summary>
        /// Flushes the report to disk.
        /// </summary>
        public static void Flush()
        {
            Instance.Flush();
        }

        /// <summary>
        /// Adds Azure DevOps system information if available.
        /// </summary>
        private static void AddIfExists(
            ExtentReports extent,
            string displayName,
            string environmentVariable)
        {
            string? value =
                Environment.GetEnvironmentVariable(environmentVariable);

            if (!string.IsNullOrWhiteSpace(value))
            {
                extent.AddSystemInfo(displayName, value);
            }
        }
    }
}