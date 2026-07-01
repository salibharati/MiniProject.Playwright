using System.Text.Json;

namespace MiniProject.Playwright.Tests.Config
{
    /// <summary>
    /// Reads application configuration from appsettings.json.
    /// </summary>
    public static class ConfigReader
    {
        private static readonly Lazy<AppSettings> _settings =
            new(LoadConfiguration);

        /// <summary>
        /// Returns the loaded configuration.
        /// </summary>
        public static AppSettings Load()
        {
            return _settings.Value;
        }

        /// <summary>
        /// Loads appsettings.json from the application directory.
        /// </summary>
        private static AppSettings LoadConfiguration()
        {
            string configFile = Path.Combine(
                AppContext.BaseDirectory,
                "appsettings.json");

            if (!File.Exists(configFile))
            {
                throw new FileNotFoundException(
                    $"Configuration file not found.\nLocation: {configFile}");
            }

            string json = File.ReadAllText(configFile);

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

        /// <summary>
        /// Validates mandatory configuration values.
        /// </summary>
        private static void Validate(AppSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.BrowserSettings.BrowserName))
            {
                throw new InvalidOperationException(
                    "BrowserSettings:BrowserName is missing.");
            }

            if (settings.BrowserSettings.SlowMo < 0)
            {
                throw new InvalidOperationException(
                    "BrowserSettings:SlowMo cannot be negative.");
            }

            if (settings.BrowserSettings.Timeout <= 0)
            {
                throw new InvalidOperationException(
                    "BrowserSettings:Timeout must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(settings.TestSettings.BaseUrl))
            {
                throw new InvalidOperationException(
                    "TestSettings:BaseUrl is missing.");
            }

            if (string.IsNullOrWhiteSpace(settings.EnvironmentSettings.EnvironmentName))
            {
                throw new InvalidOperationException(
                    "EnvironmentSettings:EnvironmentName is missing.");
            }

            if (string.IsNullOrWhiteSpace(settings.ReportSettings.ReportName))
            {
                throw new InvalidOperationException(
                    "ReportSettings:ReportName is missing.");
            }
        }
    }
}