using MiniProject.Playwright.Tests.Utilities;
using NUnit.Framework;

[OneTimeSetUp]
public void BeforeAllTests()
{
    TestPaths.CreateDirectories();

    Logger.Info("========== TEST EXECUTION STARTED ==========");

    _ = ExtentReportManager.Instance;
}

[OneTimeTearDown]
public void AfterAllTests()
{
    ExtentReportManager.Flush();

    Logger.Info("========== TEST EXECUTION COMPLETED ==========");

    Logger.Close();
}