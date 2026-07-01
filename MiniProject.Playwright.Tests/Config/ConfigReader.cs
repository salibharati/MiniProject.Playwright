using System.Text.Json;

namespace MiniProject.Playwright.Tests.Config;

public static class ConfigReader
{
    private static readonly Lazy<AppSettings> _settings =
        new(LoadConfiguration);

    public static AppSettings Load()
    {
        return _settings.Value;
    }

    private static AppSettings LoadConfiguration()
    {
        string configFile = Path.Combine(
            AppContext.BaseDirectory,
            "appsettings.json");

        if (!File.Exists(configFile))
        {
            throw new FileNotFoundException(
                $"Configuration file not found: {configFile}");
        }

        var json = File.ReadAllText(configFile);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var settings = JsonSerializer.Deserialize<AppSettings>(
            json,
            options);

        if (settings == null)
        {
            throw new InvalidOperationException(
                "Unable to deserialize appsettings.json.");
        }

        Validate(settings);

        return settings;
    }

    private static void Validate(AppSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.BrowserSettings.BrowserName))
            throw new InvalidOperationException(
                "BrowserSettings.BrowserName is required.");

        if (settings.BrowserSettings.SlowMo < 0)
            throw new InvalidOperationException(
                "BrowserSettings.SlowMo cannot be negative.");

        if (string.IsNullOrWhiteSpace(settings.Urls.PlaywrightDevUrl))
            throw new InvalidOperationException(
                "Urls.PlaywrightDevUrl is required.");

        if (string.IsNullOrWhiteSpace(settings.Urls.TodoMvcUrl))
            throw new InvalidOperationException(
                "Urls.TodoMvcUrl is required.");
    }
}