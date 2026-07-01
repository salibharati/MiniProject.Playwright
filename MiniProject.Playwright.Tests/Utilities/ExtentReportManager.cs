using AventStack.ExtentReports;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Config;

namespace MiniProject.Playwright.Tests.Utilities;

public static class ExtentReportManager
{
    private static readonly Lazy<ExtentReports> _extent = new(CreateReport);

    public static ExtentReports Instance => _extent.Value;

    private static ExtentReports CreateReport()
    {
        var reportDir = Environment.GetEnvironmentVariable("BUILD_ARTIFACTSTAGINGDIRECTORY");

        if (string.IsNullOrWhiteSpace(reportDir))
        {
            reportDir = Path.Combine(AppContext.BaseDirectory, "TestResults");
        }

        reportDir = Path.Combine(reportDir, "Reports");
        Directory.CreateDirectory(reportDir);

        var reportFile = Path.Combine(reportDir, "ExtentReport.html");

        var spark = new ExtentSparkReporter(reportFile);

        spark.Config.Theme = Theme.Dark;
        spark.Config.DocumentTitle = "Playwright Report";
        spark.Config.ReportName = "Automation Execution Report";

        var extent = new ExtentReports();

        extent.AttachReporter(spark);

        extent.AddSystemInfo("Framework", ".NET 8");
        extent.AddSystemInfo("Automation", "Playwright");

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