using Microsoft.Playwright;
using NUnit.Framework;

namespace MiniProject.Playwright.Tests.Utilities;

/// <summary>
/// Captures and attaches a full-page screenshot when a test fails,
/// equivalent to the `screenshot: 'only-on-failure'` setting in playwright.config.js.
/// </summary>
public static class ScreenshotHelper
{
    private static readonly char[] InvalidChars = Path.GetInvalidFileNameChars();

    public static async Task CaptureAsync(IPage page, string testName)
    {
        var root = Environment.GetEnvironmentVariable("BUILD_ARTIFACTSTAGINGDIRECTORY");

        if (string.IsNullOrWhiteSpace(root))
        {
            root = Path.Combine(AppContext.BaseDirectory, "TestResults");
        }

        var directory = Path.Combine(root, "Screenshots");
        Directory.CreateDirectory(directory);
        var fileName = $"{Sanitize(testName)}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
        var filePath = Path.Combine(directory, fileName);

        await page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = filePath,
            FullPage = true
        });

        // Surfaces the screenshot in the NUnit/TRX test result output.
        TestContext.AddTestAttachment(filePath, $"Screenshot on failure: {testName}");
    }

    private static string Sanitize(string name)
    {
        foreach (var c in InvalidChars)
            name = name.Replace(c, '_');
        return name;
    }
}
