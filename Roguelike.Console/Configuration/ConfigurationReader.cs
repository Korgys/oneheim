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
            try
            {
                _gameSettings = JsonSerializer.Deserialize<GameSettings>(json);
            }
            catch (Exception)
            {
                // Ignore
            }
        }

        // Ensure that game settings are not null
        if (_gameSettings?.ControlsSettings?.ExitGame == null)
        {
            // Default game settings
            _gameSettings = new GameSettings
            {
                DifficultySettings = new DifficultySettings
                {
                    Difficulty = DifficultyLevel.Normal,
                },
                ControlsSettings = new ControlsSettings
                {
                    ExitGame = "ESCAPE",
                    MoveUp = "Z",
                    MoveDown = "S",
                    MoveLeft = "Q",
                    MoveRight = "D",
                    Choice1 = "W",
                    Choice2 = "X",
                    Choice3 = "C",
                }
            };
        }
            
        return _gameSettings;
    }
}
