using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Config;

namespace MiniProject.Playwright.Tests.Utilities
{
    public static class ExtentReportManager
    {
        private static readonly Lazy<ExtentReports> _extent = new(CreateReport);

        public static ExtentReports Instance => _extent.Value;

        private static ExtentReports CreateReport()
        {
            // Azure DevOps Artifact folder
            string? reportRoot = Environment.GetEnvironmentVariable("BUILD_ARTIFACTSTAGINGDIRECTORY");

            // Local execution (Visual Studio)
            if (string.IsNullOrWhiteSpace(reportRoot))
            {
                reportRoot = Path.Combine(AppContext.BaseDirectory, "TestResults");
            }

            // Final Report Folder
            string reportDirectory = Path.Combine(reportRoot, "Reports");

            Directory.CreateDirectory(reportDirectory);

            string reportFile = Path.Combine(reportDirectory, "ExtentReport.html");

            var sparkReporter = new ExtentSparkReporter(reportFile);

            sparkReporter.Config.Theme = Theme.Dark;
            sparkReporter.Config.DocumentTitle = "Playwright .NET Automation Report";
            sparkReporter.Config.ReportName = "MiniProject - Playwright Execution Report";
            sparkReporter.Config.Encoding = "utf-8";

            var extent = new ExtentReports();

            extent.AttachReporter(sparkReporter);

            // System Information
            extent.AddSystemInfo("Framework", ".NET 8");
            extent.AddSystemInfo("Automation Tool", "Playwright");
            extent.AddSystemInfo("Report Type", "Extent Report");
            extent.AddSystemInfo("Environment", "QA");
            extent.AddSystemInfo("Operating System", Environment.OSVersion.ToString());
            extent.AddSystemInfo(".NET Version", Environment.Version.ToString());
            extent.AddSystemInfo("Machine Name", Environment.MachineName);

            return extent;
        }

        public static ExtentTest CreateTest(string testName)
        {
            return Instance.CreateTest(testName);
        }

        public static void Flush()
        {
            Instance.Flush();
        }
    }
}