using System.Text.Json;
using MiniProject.Playwright.Tests.Config;

namespace MiniProject.Playwright.Tests.Utilities;

/// <summary>
/// Loads and deserializes appsettings.json into a strongly-typed AppSettings instance.
/// Reads from the test assembly's output directory so it works the same way
/// whether tests are run from Visual Studio, `dotnet test`, or CI.
/// </summary>
public static class ConfigReader
{
    private static AppSettings? _cached;

    public static AppSettings Load()
    {
        if (_cached is not null)
            return _cached;

        var configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException(
                $"Configuration file not found at '{configPath}'. " +
                "Ensure appsettings.json has 'Copy to Output Directory' set to 'Copy if newer'.");
        }

        var json = File.ReadAllText(configPath);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        _cached = JsonSerializer.Deserialize<AppSettings>(json, options)
            ?? throw new InvalidOperationException("Failed to deserialize appsettings.json - file is empty or malformed.");

        return _cached;
    }
}
