using System.Globalization;
using System.Text.Json;

namespace Roguelike.Console.Configuration;

public class ConfigurationReader
{
    private static GameSettings? _gameSettings = null;

    public static GameSettings LoadGameSettings()
    {
        // Add cache on game settings
        if (_gameSettings != null) return _gameSettings;

        // Load settings from gameSettings.json
        string filePath = "gameSettings.json";
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            };

            // Deserialize JSON to GameSettings object
            try
            {
                _gameSettings = JsonSerializer.Deserialize<GameSettings>(json, options);
            }
            catch (Exception)
            {
                // Ignore
            }
        }

        // Ensure that game settings are not null
        if (_gameSettings?.Controls?.Exit == null)
            _gameSettings = new GameSettings();

        // Language settings
        if (_gameSettings.Language.ToUpper() == "FR")
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
        else
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

        return _gameSettings;
    }
}
