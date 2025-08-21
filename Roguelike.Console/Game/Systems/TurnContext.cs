namespace Roguelike.Console.Game.Systems;

using Roguelike.Console.Configuration;
using Roguelike.Console.Game.Levels;

public sealed class TurnContext
{
    public LevelManager Level { get; }
    public GameSettings Settings { get; }
    public DifficultyManager Difficulty { get; }

    public TurnContext(LevelManager level, GameSettings gameSettings, DifficultyManager difficultyManager)
    {
        Level = level;
        Settings = gameSettings;
        Difficulty = difficultyManager;
    }
}