using Roguelike.Core.Configuration;

namespace Roguelike.Core.Game.Levels;

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
                return 20;
            case Difficulty.Hard:
                return 18;
            case Difficulty.Hell:
                return 16;
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
