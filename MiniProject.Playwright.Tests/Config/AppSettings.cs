namespace MiniProject.Playwright.Tests.Config;

public class AppSettings
{
    public BrowserSettings BrowserSettings { get; set; } = new();

    public TestSettings TestSettings { get; set; } = new();

    public UrlSettings Urls { get; set; } = new();
}

public class BrowserSettings
{
    public string BrowserName { get; set; } = "chromium";

    public bool Headless { get; set; } = true;

    public int SlowMo { get; set; } = 0;
}

public class TestSettings
{
    public bool ScreenshotOnFailure { get; set; } = true;

    public bool VideoOnFailure { get; set; } = true;

    public bool TraceOnFirstRetry { get; set; } = true;
}

public class UrlSettings
{
    public string PlaywrightDevUrl { get; set; } = "";

    public string TodoMvcUrl { get; set; } = "";
}