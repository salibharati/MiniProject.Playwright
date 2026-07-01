using AventStack.ExtentReports;

namespace MiniProject.Playwright.Tests.Utilities;

public static class ExtentTestContext
{
    public static AsyncLocal<ExtentTest> CurrentTest = new();

    public static ExtentTest Current
    {
        get => CurrentTest.Value!;
        set => CurrentTest.Value = value;
    }
}
