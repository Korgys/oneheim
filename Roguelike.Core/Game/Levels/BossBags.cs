using Roguelike.Core.Game.Characters.Enemies;

namespace Roguelike.Core.Game.Levels;

public class BossBags
{
    public static Dictionary<EnemyId, int> GetByLevel(int level)
    {
        return level switch
        {
            5 => Level5,
            10 => Level10,
            _ => throw new ArgumentOutOfRangeException(nameof(level), "Invalid level")
        };
    }

    public static Dictionary<EnemyId, int> Level5 = new Dictionary<EnemyId, int>
    {
        { EnemyId.Lich, 1 },
        { EnemyId.Troll, 1 },
        { EnemyId.Wyvern, 1 },
        { EnemyId.HighPriest, 1 },
        { EnemyId.ChiefBrigand, 1 },
        { EnemyId.YannTheSilent, 1 },
    };

    public static Dictionary<EnemyId, int> Level10 = new Dictionary<EnemyId, int>
    {
        { EnemyId.Lich, 1 },
        { EnemyId.Troll, 1 },
        { EnemyId.Wyvern, 1 },
        { EnemyId.HighPriest, 1 },
        { EnemyId.ChiefBrigand, 1 },
        { EnemyId.YannTheSilent, 1 },
    };
}
