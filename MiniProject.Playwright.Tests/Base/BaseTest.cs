using Microsoft.Playwright;
using MiniProject.Playwright.Tests.Config;
using MiniProject.Playwright.Tests.Utilities;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using PlaywrightFactory = Microsoft.Playwright.Playwright;

namespace MiniProject.Playwright.Tests.Base;

/// <summary>
/// Base class for all UI test fixtures. Owns the Playwright/Browser lifecycle
/// (created once per fixture) and the BrowserContext/Page lifecycle (created
/// fresh per test for isolation). Re-implements, in C#, the behavior that was
/// previously configured declaratively in playwright.config.js:
///   - browserName / headless / slowMo   -> BrowserSettings
///   - screenshot: 'only-on-failure'     -> ScreenshotHelper, called from TearDown
///   - video: 'retain-on-failure'        -> recorded always, deleted on pass
///   - trace: 'on-first-retry'           -> recorded always, persisted only on failure
/// </summary>
[Parallelizable(ParallelScope.Self)]
public abstract class BaseTest
{
    private static IPlaywright _playwright = null!;
    private static IBrowser _browser = null!;

    protected static AppSettings Settings { get; private set; } = null!;
    protected IBrowserContext Context { get; private set; } = null!;
    protected IPage Page { get; private set; } = null!;

    private string? _videoDirectory;

    [OneTimeSetUp]
    public async Task GlobalSetUpAsync()
    {
        Settings = ConfigReader.Load();
        _playwright = await PlaywrightFactory.CreateAsync();
        _browser = await LaunchBrowserAsync();
    }

    private static async Task<IBrowser> LaunchBrowserAsync()
    {
        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = Settings.BrowserSettings.Headless,
            SlowMo = Settings.BrowserSettings.SlowMo
        };

        return Settings.BrowserSettings.BrowserName.Trim().ToLowerInvariant() switch
        {
            "firefox" => await _playwright.Firefox.LaunchAsync(launchOptions),
            "webkit" => await _playwright.Webkit.LaunchAsync(launchOptions),
            _ => await _playwright.Chromium.LaunchAsync(launchOptions),
        };
    }

    [SetUp]
    public async Task SetUpAsync()
    {
        var contextOptions = new BrowserNewContextOptions();

        if (Settings.TestSettings.VideoOnFailure)
        {
            // Playwright can only record to a directory, not a specific file path.
            // We record every test into its own directory and decide whether to
            // keep or discard it once the test outcome is known, in TearDown.
            _videoDirectory = Path.Combine("TestResults", "Videos", $"{Sanitize(TestContext.CurrentContext.Test.Name)}_{Guid.NewGuid():N}");
            Directory.CreateDirectory(_videoDirectory);
            contextOptions.RecordVideoDir = _videoDirectory;
        }

        Context = await _browser.NewContextAsync(contextOptions);

        if (Settings.TestSettings.TraceOnFirstRetry)
        {
            await Context.Tracing.StartAsync(new TracingStartOptions
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
        }

        Page = await Context.NewPageAsync();
    }

    [TearDown]
    public async Task TearDownAsync()
    {
        var testFailed = TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed;
        var testName = TestContext.CurrentContext.Test.Name;

        if (testFailed && Settings.TestSettings.ScreenshotOnFailure)
        {
            await ScreenshotHelper.CaptureAsync(Page, testName);
        }

        if (Settings.TestSettings.TraceOnFirstRetry)
        {
            string? tracePath = null;
            if (testFailed)
            {
                var traceDir = Path.Combine("TestResults", "Traces");
                Directory.CreateDirectory(traceDir);
                tracePath = Path.Combine(traceDir, $"{Sanitize(testName)}.zip");
            }

            // Passing a null Path discards the trace instead of writing it to disk.
            await Context.Tracing.StopAsync(new TracingStopOptions { Path = tracePath });
        }

        // Closing the context finalizes any in-progress video recording to disk.
        await Context.CloseAsync();

        if (Settings.TestSettings.VideoOnFailure && !string.IsNullOrEmpty(_videoDirectory))
        {
            if (!testFailed && Directory.Exists(_videoDirectory))
            {
                Directory.Delete(_videoDirectory, recursive: true);
            }
        }
    }

    [OneTimeTearDown]
    public async Task GlobalTearDownAsync()
    {
        await _browser.CloseAsync();
        _playwright.Dispose();
    }

    private static string Sanitize(string name)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return name;
    }
}
