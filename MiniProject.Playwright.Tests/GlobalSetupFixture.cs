using MiniProject.Playwright.Tests.Utilities;
using NUnit.Framework;

namespace MiniProject.Playwright.Tests
{
    [SetUpFixture]
    public class GlobalSetupFixture
    {
        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            TestPaths.CreateDirectories();

            Logger.Info("========== TEST EXECUTION STARTED ==========");

            // Initialize Extent Report
            _ = ExtentReportManager.Instance;
        }

        [OneTimeTearDown]
        public void AfterAllTests()
        {
            ExtentReportManager.Flush();

            Logger.Info("========== TEST EXECUTION COMPLETED ==========");

            Logger.Close();
        }
    }
}