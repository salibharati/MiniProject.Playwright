using Microsoft.Playwright;
using NUnit.Framework;

namespace MiniProject.Playwright.Tests.Utilities
{
    /// <summary>
    /// Captures a full-page screenshot for failed tests.
    /// Supports both Local execution and Azure DevOps pipelines.
    /// </summary>
    public static class ScreenshotHelper
    {
        private static readonly char[] InvalidChars = Path.GetInvalidFileNameChars();

        /// <summary>
        /// Captures a screenshot and returns its file path.
        /// </summary>
        public static async Task<string> CaptureAsync(IPage page, string testName)
        {
            // Ensure screenshot folder exists
            Directory.CreateDirectory(TestPaths.Screenshots);

            string fileName =
                $"{Sanitize(testName)}_{DateTime.Now:yyyyMMdd_HHmmss}.png";

            string filePath =
                Path.Combine(TestPaths.Screenshots, fileName);

            await page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = filePath,
                FullPage = true
            });

            // Attach screenshot to NUnit Test Results
            TestContext.AddTestAttachment(
                filePath,
                $"Failure Screenshot - {testName}");

            return filePath;
        }

        /// <summary>
        /// Removes invalid filename characters.
        /// </summary>
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