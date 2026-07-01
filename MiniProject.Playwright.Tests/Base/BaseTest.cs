using AventStack.ExtentReports;
using Microsoft.Playwright;
using MiniProject.Playwright.Tests.Config;
using MiniProject.Playwright.Tests.Utilities;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using PlaywrightFactory = Microsoft.Playwright.Playwright;

namespace MiniProject.Playwright.Tests.Base;

/// <summary>
/// Base class for all UI test fixtures.
///
/// ═══════════════════════════════════════════════════════════════════════════════
/// ROOT CAUSE FIX
/// ═══════════════════════════════════════════════════════════════════════════════
///
/// Previously, _playwright and _browser were declared as static fields,
/// meaning every test fixture (ExampleTests, TodoTests, etc.) shared the
/// same Playwright process.
///
/// NUnit executes [OneTimeSetUp] once per fixture class. When the second
/// fixture started, it replaced the shared static Playwright instance,
/// causing the first fixture's connection to be disposed.
///
/// This resulted in:
///
/// Microsoft.Playwright.TargetClosedException : Connection disposed
///
/// FIX:
/// - _playwright and _browser are now INSTANCE fields.
/// - Every fixture class owns its own browser.
/// - [OneTimeSetUp]/[OneTimeTearDown] manage that fixture's lifecycle.
/// - No cross-fixture interference.
///
/// Settings are loaded only once using Lazy<AppSettings>, making them
/// thread-safe while remaining shared across fixtures.
///
/// ExtentReports lifecycle:
///     SetUp     -> Create test node
///     TearDown  -> Log Pass/Fail/Skip
///     Assembly  -> Flush report
/// </summary>

[Parallelizable(ParallelScope.Self)]
public abstract class BaseTest
{
    #region Playwright Fields

    // One browser per fixture
    private IPlaywright _playwright = null!;
    private IBrowser _browser = null!;

    protected IBrowserContext Context { get; private set; } = null!;
    protected IPage Page { get; private set; } = null!;

    private string? _videoDirectory;

    #endregion

    #region Configuration

    private static readonly Lazy<AppSettings> _lazySettings =
        new(
            ConfigReader.Load,
            LazyThreadSafetyMode.ExecutionAndPublication);

    protected static AppSettings Settings => _lazySettings.Value;

    #endregion

    #region One Time Setup

    [OneTimeSetUp]
    public async Task GlobalSetUpAsync()
    {
        _playwright = await PlaywrightFactory.CreateAsync();
        _browser = await LaunchBrowserAsync();
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
        var testName = TestContext.CurrentContext.Test.Name;
        var className = TestContext.CurrentContext.Test.ClassName ?? string.Empty;

        var extentTest =
    ExtentReportManager.CreateTest(testName);

        ExtentTestContext.Current = extentTest;
        foreach (var category in
                 TestContext.CurrentContext.Test.Properties["Category"].Cast<string>())
        {
            extentTest.AssignCategory(category);
        }

        extentTest.AssignDevice(Settings.BrowserSettings.BrowserName);

        extentTest.Log(
            Status.Info,
            $"<b>Test Started:</b> {testName}");

        ExtentTestContext.Current = extentTest;

        var contextOptions = new BrowserNewContextOptions();

        if (Settings.TestSettings.VideoOnFailure)
        {
            _videoDirectory = Path.Combine(
                "TestResults",
                "Videos",
                $"{Sanitize(testName)}_{Guid.NewGuid():N}");

            Directory.CreateDirectory(_videoDirectory);

            contextOptions.RecordVideoDir = _videoDirectory;
        }

        Context = await _browser.NewContextAsync(contextOptions);

        if (Settings.TestSettings.TraceOnFirstRetry)
        {
            await Context.Tracing.StartAsync(
                new TracingStartOptions
                {
                    Screenshots = true,
                    Snapshots = true,
                    Sources = true
                });
        }

        Page = await Context.NewPageAsync();
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

        if (testFailed && Settings.TestSettings.ScreenshotOnFailure)
        {
            try
            {
                await ScreenshotHelper.CaptureAsync(Page, testName);
                // ScreenshotHelper.CaptureAsync returns Task, not Task<string>.
                // If you need the path, update ScreenshotHelper.CaptureAsync to return it.
                // For now, set screenshotPath to null or to a known path if possible.
            }
            catch (Exception ex)
            {
                extentTest.Log(
                    Status.Warning,
                    $"Screenshot failed: {ex.Message}");
            }
        }

        var video =
            Settings.TestSettings.VideoOnFailure
                ? Page?.Video
                : null;

        if (Settings.TestSettings.TraceOnFirstRetry)
        {
            try
            {
                string? tracePath = null;

                if (testFailed)
                {
                    var traceDir = Path.Combine(
                        "TestResults",
                        "Traces");

                    Directory.CreateDirectory(traceDir);

                    tracePath = Path.Combine(
                        traceDir,
                        $"{Sanitize(testName)}.zip");
                }

                await Context.Tracing.StopAsync(
                    new TracingStopOptions
                    {
                        Path = tracePath
                    });

                if (tracePath != null)
                {
                    extentTest.Log(
                        Status.Info,
                        $"Trace: <code>{tracePath}</code>");
                }
            }
            catch (Exception ex)
            {
                extentTest.Log(
                    Status.Warning,
                    $"Trace failed: {ex.Message}");
            }
        }

        try
        {
            await Context.CloseAsync();
        }
        catch (Exception ex)
        {
            extentTest.Log(
                Status.Warning,
                $"Context close failed: {ex.Message}");
        }

        var errorMessage =
            TestContext.CurrentContext.Result.Message ?? string.Empty;

        switch (outcome)
        {
            case TestStatus.Failed:

                extentTest.Log(
                    Status.Fail,
                    $"<pre>{System.Net.WebUtility.HtmlEncode(errorMessage)}</pre>");

                if (screenshotPath != null)
                {
                    extentTest.AddScreenCaptureFromPath(
                        screenshotPath,
                        "Screenshot");
                }

                break;

            case TestStatus.Inconclusive:

                extentTest.Log(
                    Status.Warning,
                    string.IsNullOrWhiteSpace(errorMessage)
                        ? "Test was inconclusive."
                        : errorMessage);

                break;

            case TestStatus.Skipped:

                extentTest.Log(
                    Status.Skip,
                    string.IsNullOrWhiteSpace(errorMessage)
                        ? "Test was skipped."
                        : errorMessage);

                break;

            default:

                extentTest.Log(
                    Status.Pass,
                    "Test Passed ✔");

                break;
        }

        if (video != null)
        {
            try
            {
                if (testFailed)
                {
                    var videoPath = await video.PathAsync();

                    extentTest.Log(
                        Status.Info,
                        $"Video retained: <code>{videoPath}</code>");
                }
                else if (!string.IsNullOrEmpty(_videoDirectory) &&
                         Directory.Exists(_videoDirectory))
                {
                    Directory.Delete(_videoDirectory, true);
                }
            }
            catch (Exception ex)
            {
                extentTest.Log(
                    Status.Warning,
                    $"Video handling failed: {ex.Message}");
            }
        }
    }

    #endregion

    #region One Time TearDown

    [OneTimeTearDown]
    public async Task GlobalTearDownAsync()
    {
        try
        {
            await _browser.CloseAsync();
        }
        catch
        {
            // Best effort
        }

        try
        {
            _playwright.Dispose();
        }
        catch
        {
            // Best effort
        }
    }

    #endregion

    #region Helper Methods

    private static string Sanitize(string name)
    {
        foreach (var invalidChar in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(invalidChar, '_');
        }

        return name;
    }

    #endregion
}

// Add this class to provide the missing ExtentTestContext used in BaseTest.
// This assumes ExtentTestContext is a simple static context holder for the current ExtentTest.
// Place this class in the appropriate namespace (MiniProject.Playwright.Tests.Utilities or similar)
// if it is referenced elsewhere, or adjust the namespace as needed.

public static class ExtentTestContext
{
    [ThreadStatic]
    private static ExtentTest? _current;

    public static ExtentTest? Current
    {
        get => _current;
        set => _current = value;
    }
}