namespace MiniProject.Playwright.Tests.Config;

/// <summary>
/// Root configuration model deserialized from appsettings.json.
/// Mirrors the settings previously expressed in playwright.config.js.
/// </summary>
public class AppSettings
{
    public BrowserSettings BrowserSettings { get; set; } = new();
    public TestSettings TestSettings { get; set; } = new();
    public UrlSettings Urls { get; set; } = new();
}

/// <summary>
/// Maps to the original `use` block in playwright.config.js
/// (browserName, launchOptions.headless, launchOptions.slowMo).
/// </summary>
public class BrowserSettings
{
    public string BrowserName { get; set; } = "chromium";
    public bool Headless { get; set; } = false;
    public float SlowMo { get; set; } = 0;
}

/// <summary>
/// Maps to the original screenshot / video / trace / retries settings
/// from playwright.config.js.
/// </summary>
public class TestSettings
{
    public bool ScreenshotOnFailure { get; set; } = true;
    public bool VideoOnFailure { get; set; } = true;
    public bool TraceOnFirstRetry { get; set; } = true;
    public int Retries { get; set; } = 0;
}

public class UrlSettings
{
    public string PlaywrightDevUrl { get; set; } = string.Empty;
    public string TodoMvcUrl { get; set; } = string.Empty;
}
