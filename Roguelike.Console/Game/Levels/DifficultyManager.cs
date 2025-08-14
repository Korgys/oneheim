using Roguelike.Console.Configuration;

namespace Roguelike.Console.Game.Levels;

public class DifficultyManager
{
    public DifficultyLevel DifficultyLevel { get; set; }

    public DifficultyManager (DifficultyLevel difficultyLevel)
    {
        DifficultyLevel = difficultyLevel;
    }

    public int GetTreasuresNumber()
    {
        switch (DifficultyLevel)
        {
            case DifficultyLevel.Normal:
                return 16;
            case DifficultyLevel.Hard:
                return 14;
            case DifficultyLevel.Hell:
                return 12;
            default:
                return 1;
        }
    }

    public int GetEnemiesNumber()
    {
        switch (DifficultyLevel)
        {
            case DifficultyLevel.Normal:
                return 16;
            case DifficultyLevel.Hard:
                return 20;
            case DifficultyLevel.Hell:
                return 24;
            default:
                return 100;
        }
    }
}
