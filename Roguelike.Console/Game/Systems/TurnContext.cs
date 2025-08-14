namespace Roguelike.Console.Game.Systems;

using Roguelike.Console.Configuration;
using Roguelike.Console.Game.Levels;

public sealed class TurnContext
{
    public LevelManager Level { get; }
    public GameSettings Settings { get; }
    public DifficultyManager Difficulty { get; }
    public Random Rng { get; }
    public bool PlayerMovedThisTurn { get; }
    public int TurnNumber { get; }
    public Action<string> PushMessage { get; }   // collect messages (UI)

    public TurnContext(
        LevelManager level,
        GameSettings settings,
        DifficultyManager difficulty,
        Random rng,
        bool playerMovedThisTurn,
        int turnNumber,
        Action<string> pushMessage)
    {
        Level = level;
        Settings = settings;
        Difficulty = difficulty;
        Rng = rng;
        PlayerMovedThisTurn = playerMovedThisTurn;
        TurnNumber = turnNumber;
        PushMessage = pushMessage;
    }
}