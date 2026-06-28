using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Roguelike.Core.Configuration;

public class ConfigurationReader
{
    private static GameSettings? _gameSettings = null;
    private static readonly string _settingsFilePath = "gameSettings.json";

    public static GameSettings LoadGameSettings()
    {
        // Add cache on game settings
        if (_gameSettings != null) return _gameSettings;

        // Load settings from gameSettings.json
        if (File.Exists(_settingsFilePath))
        {
            string json = File.ReadAllText(_settingsFilePath);
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

    public static void SaveGameSettings(GameSettings settings)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        options.Converters.Add(new JsonStringEnumConverter());

        string json = JsonSerializer.Serialize(settings, options);
        File.WriteAllText(_settingsFilePath, json);

        _gameSettings = settings;
    }

    public static GameSettings ReloadGameSettings()
    {
        _gameSettings = null;
        return LoadGameSettings();
    }
}
