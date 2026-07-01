using AventStack.ExtentReports;
using Microsoft.Playwright;
using MiniProject.Playwright.Tests.Config;
using MiniProject.Playwright.Tests.Utilities;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using PlaywrightFactory = Microsoft.Playwright.Playwright;

namespace MiniProject.Playwright.Tests.Base
{
    /// <summary>
    /// Base class for all Playwright UI tests.
    /// Creates one browser per fixture and one browser context per test.
    /// Integrates Playwright, NUnit and Extent Reports.
    /// </summary>
    [Parallelizable(ParallelScope.Self)]
    public abstract class BaseTest
    {
        #region Playwright Fields

        private IPlaywright _playwright = null!;
        private IBrowser _browser = null!;

        protected IBrowserContext Context { get; private set; } = null!;
        protected IPage Page { get; private set; } = null!;

        private string? _videoDirectory;

        #endregion

        #region Configuration

        private static readonly Lazy<AppSettings> _settings =
            new(
                ConfigReader.Load,
                LazyThreadSafetyMode.ExecutionAndPublication);

        protected static AppSettings Settings => _settings.Value;

        #endregion

        #region One Time Setup

        [OneTimeSetUp]
        public async Task GlobalSetUpAsync()
        {
            _playwright = await PlaywrightFactory.CreateAsync();

            _browser = await LaunchBrowserAsync();
            Logger.Info("Launching Playwright.");
            Logger.Info($"Browser launched : {Settings.BrowserSettings.BrowserName}");
        }

        private async Task<IBrowser> LaunchBrowserAsync()
        {
            var launchOptions = new BrowserTypeLaunchOptions
            {
                Headless = Settings.BrowserSettings.Headless,
                SlowMo = Settings.BrowserSettings.SlowMo
            };

            return Settings.BrowserSettings.BrowserName
                .Trim()
                .ToLowerInvariant() switch
            {
                "firefox" => await _playwright.Firefox.LaunchAsync(launchOptions),

                "webkit" => await _playwright.Webkit.LaunchAsync(launchOptions),

                _ => await _playwright.Chromium.LaunchAsync(launchOptions)
            };
        }

        #endregion

        #region Test Setup

        [SetUp]
        public async Task SetUpAsync()
        {
            string testName = TestContext.CurrentContext.Test.Name;

            string className =
                TestContext.CurrentContext.Test.ClassName ?? string.Empty;

            var extentTest = ExtentReportManager.CreateTest(testName);

            ExtentTestContext.Current = extentTest;

            // Categories

            foreach (string category in
                     TestContext.CurrentContext.Test.Properties["Category"].Cast<string>())
            {
                extentTest.AssignCategory(category);
            }

            // Browser

            extentTest.AssignDevice(Settings.BrowserSettings.BrowserName);

            // Author (Optional)

            extentTest.AssignAuthor(Environment.UserName);

            // Initial Logs

            extentTest.Info($"Test Started : {testName}");

            extentTest.Info($"Class : {className}");

            extentTest.Info($"Browser : {Settings.BrowserSettings.BrowserName}");

            extentTest.Info($"Headless : {Settings.BrowserSettings.Headless}");

            // Browser Context

            var contextOptions = new BrowserNewContextOptions();

            if (Settings.TestSettings.VideoOnFailure)
            {
                _videoDirectory = Path.Combine(
                    TestPaths.Videos,
                    $"{Sanitize(testName)}_{Guid.NewGuid():N}");

                Directory.CreateDirectory(_videoDirectory);

                contextOptions.RecordVideoDir = _videoDirectory;
            }

            Context = await _browser.NewContextAsync(contextOptions);

            extentTest.Info("Browser Context Created");
            Logger.Info($"Starting Test : {testName}");
            Logger.Info("Creating Browser Context.");

            // Start Trace

            if (Settings.TestSettings.TraceOnFirstRetry)
            {
                await Context.Tracing.StartAsync(
                    new TracingStartOptions
                    {
                        Screenshots = true,
                        Snapshots = true,
                        Sources = true
                    });

                extentTest.Info("Tracing Started");
            }

            Page = await Context.NewPageAsync();

            extentTest.Info("Browser Page Created");
            Logger.Info("Browser Page Created.");
        }

        #endregion
        #region Test TearDown

        [TearDown]
        public async Task TearDownAsync()
        {
            var outcome = TestContext.CurrentContext.Result.Outcome.Status;
            var testName = TestContext.CurrentContext.Test.Name;
            var testFailed = outcome == TestStatus.Failed;

            var extentTest = ExtentTestContext.Current;

            string? screenshotPath = null;

            // ==========================================================
            // Capture Screenshot (Only on Failure)
            // ==========================================================

            if (testFailed && Settings.TestSettings.ScreenshotOnFailure)
            {
                try
                {
                    screenshotPath = await ScreenshotHelper.CaptureAsync(
                        Page,
                        testName);

                    extentTest.Info("Screenshot captured.");
                    Logger.Error($"Test Failed : {testName}");
                    //Logger.Error(errorMessage);
                    if (!string.IsNullOrWhiteSpace(screenshotPath))
                    {
                        extentTest.AddScreenCaptureFromPath(
                            screenshotPath,
                            "Failure Screenshot");
                    }
                }
                catch (Exception ex)
                {
                    extentTest.Warning(
                        $"Unable to capture screenshot.<br>{ex.Message}");
                }
            }

            // ==========================================================
            // Save Video Reference
            // ==========================================================

            var video = Settings.TestSettings.VideoOnFailure
                ? Page?.Video
                : null;

            // ==========================================================
            // Stop Playwright Trace
            // ==========================================================

            if (Settings.TestSettings.TraceOnFirstRetry)
            {
                try
                {
                    string? tracePath = null;

                    if (testFailed)
                    {
                        Directory.CreateDirectory(TestPaths.Traces);

                        tracePath = Path.Combine(
                            TestPaths.Traces,
                            $"{Sanitize(testName)}.zip");
                    }

                    await Context.Tracing.StopAsync(
                        new TracingStopOptions
                        {
                            Path = tracePath
                        });

                    if (!string.IsNullOrWhiteSpace(tracePath))
                    {
                        extentTest.Info(
                            $"Trace saved : {tracePath}");
                    }
                }
                catch (Exception ex)
                {
                    extentTest.Warning(
                        $"Unable to save trace.<br>{ex.Message}");
                }
            }

            // ==========================================================
            // Close Browser Context
            // ==========================================================

            try
            {
                await Context.CloseAsync();

                extentTest.Info("Browser Context Closed.");
            }
            catch (Exception ex)
            {
                extentTest.Warning(
                    $"Context close failed.<br>{ex.Message}");
            }

            // ==========================================================
            // Test Result
            // ==========================================================

            string errorMessage =
                TestContext.CurrentContext.Result.Message ?? string.Empty;

            // ---------- Part 3B starts from here ----------


            switch (outcome)
            {
                case TestStatus.Failed:

                    extentTest.Fail(
                        $"<pre>{System.Net.WebUtility.HtmlEncode(errorMessage)}</pre>");

                    if (!string.IsNullOrWhiteSpace(screenshotPath))
                    {
                        extentTest.Info(
                            $"Screenshot Location : {screenshotPath}");
                    }

                    break;

                case TestStatus.Passed:

                    extentTest.Pass("Test Passed ✔");

                    break;

                case TestStatus.Skipped:

                    extentTest.Skip(
                        string.IsNullOrWhiteSpace(errorMessage)
                            ? "Test was skipped."
                            : errorMessage);

                    break;

                case TestStatus.Inconclusive:

                    extentTest.Warning(
                        string.IsNullOrWhiteSpace(errorMessage)
                            ? "Test was inconclusive."
                            : errorMessage);

                    break;

                default:

                    extentTest.Info("Test execution completed.");

                    break;
            }

            // ==========================================================
            // Video Handling
            // ==========================================================

            if (video != null)
            {
                try
                {
                    string videoPath = await video.PathAsync();

                    if (testFailed)
                    {
                        extentTest.Info(
                            $"Video Location : {videoPath}");
                    }
                    else
                    {
                        if (File.Exists(videoPath))
                        {
                            File.Delete(videoPath);
                        }

                        if (!string.IsNullOrWhiteSpace(_videoDirectory) &&
                            Directory.Exists(_videoDirectory) &&
                            !Directory.EnumerateFileSystemEntries(_videoDirectory).Any())
                        {
                            Directory.Delete(_videoDirectory, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    extentTest.Warning(
                        $"Video handling failed.<br>{ex.Message}");
                }
            }

            // ==========================================================
            // Final Execution Details
            // ==========================================================

            extentTest.Info(
                $"Execution Completed : {DateTime.Now:dd-MMM-yyyy HH:mm:ss}");

            extentTest.Info(
                $"Final Status : {outcome}");

            // ==========================================================
            // Cleanup
            // ==========================================================

            ExtentTestContext.Clear();
        }

        #endregion
        #region One Time TearDown

        /// <summary>
        /// Executes once after all tests in the fixture have completed.
        /// Closes the browser and disposes Playwright.
        /// </summary>
        [OneTimeTearDown]
        public async Task GlobalTearDownAsync()
        {
            try
            {
                if (_browser != null)
                {
                    await _browser.CloseAsync();
                }
            }
            catch (Exception)
            {
                // Best effort
            }

            try
            {
                _playwright?.Dispose();
            }
            catch (Exception)
            {
                // Best effort
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Removes invalid filename characters.
        /// </summary>
        private static string Sanitize(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }

            return name;
        }

        #endregion
    }
}