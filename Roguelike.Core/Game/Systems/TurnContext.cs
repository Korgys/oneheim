namespace Roguelike.Core.Game.Systems;

using Roguelike.Core.Configuration;
using Roguelike.Core.Game.Levels;

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