using Roguelike.Console.Configuration;

namespace Roguelike.Console.Game.Levels;

public class DifficultyManager
{
    public Difficulty DifficultyLevel { get; set; }

    public DifficultyManager (Difficulty difficultyLevel)
    {
        DifficultyLevel = difficultyLevel;
    }

    /// <summary>
    /// Get the number of treasures at each wave.
    /// </summary>
    /// <returns></returns>
    public int GetTreasuresNumber()
    {
        switch (DifficultyLevel)
        {
            case Difficulty.Normal:
                return 16;
            case Difficulty.Hard:
                return 14;
            case Difficulty.Hell:
                return 12;
            default:
                return 1;
        }
    }

    /// <summary>
    /// Get the number of enemies at each wave.
    /// </summary>
    /// <returns></returns>
    public int GetEnemiesNumber()
    {
        switch (DifficultyLevel)
        {
            case Difficulty.Normal:
                return 16;
            case Difficulty.Hard:
                return 20;
            case Difficulty.Hell:
                return 24;
            default:
                return 100;
        }
    }
}
