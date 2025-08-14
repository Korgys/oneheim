namespace Roguelike.Console.Configuration;

public class GameSettings
{
    public DifficultySettings DifficultySettings { get; set; } = new();
    public ControlsSettings ControlsSettings { get; set; } = new();
}
