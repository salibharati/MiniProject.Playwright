using Microsoft.Playwright;
using NUnit.Framework;

namespace MiniProject.Playwright.Tests.Utilities
{
    /// <summary>
    /// Captures a full-page screenshot when a test fails.
    /// Works for both Local execution and Azure DevOps.
    /// </summary>
    public static class ScreenshotHelper
    {
        private static readonly char[] InvalidChars = Path.GetInvalidFileNameChars();

        public static async Task<string> CaptureAsync(IPage page, string testName)
        {
            // Azure DevOps Artifact Folder
            string? rootFolder = Environment.GetEnvironmentVariable("BUILD_ARTIFACTSTAGINGDIRECTORY");

            // Local execution
            if (string.IsNullOrWhiteSpace(rootFolder))
            {
                rootFolder = Path.Combine(AppContext.BaseDirectory, "TestResults");
            }

            // Screenshots folder
            string screenshotFolder = Path.Combine(rootFolder, "Screenshots");

            Directory.CreateDirectory(screenshotFolder);

            string fileName = $"{Sanitize(testName)}_{DateTime.Now:yyyyMMdd_HHmmss}.png";

            string filePath = Path.Combine(screenshotFolder, fileName);

            await page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = filePath,
                FullPage = true
            });

            // Attach screenshot to NUnit Test Results
            TestContext.AddTestAttachment(filePath, $"Failure Screenshot - {testName}");

            return filePath;
        }

        private static string Sanitize(string name)
        {
            foreach (char c in InvalidChars)
            {
                name = name.Replace(c, '_');
            }

            return name;
        }
    }
}