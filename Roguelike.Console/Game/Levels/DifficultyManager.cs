using Roguelike.Console.Configuration;

namespace Roguelike.Console.Game.Levels;

public class DifficultyManager
{
    public Difficulty DifficultyLevel { get; set; }

    public DifficultyManager (Difficulty difficultyLevel)
    {
        DifficultyLevel = difficultyLevel;
    }

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
