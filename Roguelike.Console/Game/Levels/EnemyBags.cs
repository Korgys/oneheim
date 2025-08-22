using Roguelike.Console.Game.Characters.Enemies;

namespace Roguelike.Console.Game.Levels;

public static class EnemyBags
{
    public static Dictionary<EnemyId, int> GetByLevel(int level)
    {
        return level switch
        {
            1 => Level1,
            2 => Level2,
            3 => Level3,
            4 => Level4,
            5 => Level5,
            6 => Level6,
            7 => Level7,
            8 => Level8,
            9 => Level9,
            10 => Level10,
            _ => throw new ArgumentOutOfRangeException(nameof(level), "Invalid level")
        };
    }

    public static Dictionary<EnemyId, int>  Level1 = new Dictionary<EnemyId, int>
    {
        { EnemyId.LeglessZombie,200 },
        { EnemyId.WildBear,     160 },
        { EnemyId.Zombie,        20 },
        { EnemyId.Wolf,          10 },
        { EnemyId.ArmoredZombie,  2 },
        { EnemyId.GiantSpider,    5 },
        { EnemyId.PlagueGhoul,    5 },
        { EnemyId.AlphaWolf,      1 },
        { EnemyId.Werewolf,       1 },
        { EnemyId.Revenant,       1 }
    };

    public static Dictionary<EnemyId, int> Level2 = new Dictionary<EnemyId, int>
    {
        { EnemyId.LeglessZombie, 90 },
        { EnemyId.WildBear,      80 },
        { EnemyId.Zombie,        20 },
        { EnemyId.Wolf,          10 },
        { EnemyId.ArmoredZombie,  2 },
        { EnemyId.GiantSpider,    5 },
        { EnemyId.PlagueGhoul,    5 },
        { EnemyId.AlphaWolf,      1 },
        { EnemyId.Werewolf,       1 },
        { EnemyId.Revenant,       1 }
    };

    public static Dictionary<EnemyId, int> Level3 = new Dictionary<EnemyId, int>
    {
        { EnemyId.LeglessZombie, 10 },
        { EnemyId.WildBear,       5 },
        { EnemyId.Zombie,        90 },
        { EnemyId.Wolf,          10 },
        { EnemyId.ArmoredZombie, 40 },
        { EnemyId.GiantSpider,    5 },
        { EnemyId.PlagueGhoul,    5 },
        { EnemyId.AlphaWolf,      2 },
        { EnemyId.Werewolf,       1 },
        { EnemyId.Revenant,       1 }
    };

    public static Dictionary<EnemyId, int> Level4 = new Dictionary<EnemyId, int>
    {
        { EnemyId.LeglessZombie, 10 },
        { EnemyId.WildBear,      10 },
        { EnemyId.Zombie,        25 },
        { EnemyId.Wolf,          95 },
        { EnemyId.ArmoredZombie, 25 },
        { EnemyId.GiantSpider,   10 },
        { EnemyId.PlagueGhoul,   10 },
        { EnemyId.AlphaWolf,     15 },
        { EnemyId.Werewolf,      10 },
        { EnemyId.Revenant,       2 }
    };

    public static Dictionary<EnemyId, int> Level5 = new Dictionary<EnemyId, int>
    {
        { EnemyId.LeglessZombie,  5 },
        { EnemyId.WildBear,       5 },
        { EnemyId.Zombie,        20 },
        { EnemyId.Wolf,          10 },
        { EnemyId.ArmoredZombie, 50 },
        { EnemyId.GiantSpider,   30 },
        { EnemyId.PlagueGhoul,   30 },
        { EnemyId.AlphaWolf,     10 },
        { EnemyId.Werewolf,      10 },
        { EnemyId.Revenant,      10 }
    };

    public static Dictionary<EnemyId, int> Level6 = new Dictionary<EnemyId, int>
    {
        { EnemyId.LeglessZombie,  2 },
        { EnemyId.WildBear,       2 },
        { EnemyId.Zombie,        10 },
        { EnemyId.Wolf,           5 },
        { EnemyId.ArmoredZombie, 30 },
        { EnemyId.GiantSpider,   10 },
        { EnemyId.PlagueGhoul,   10 },
        { EnemyId.AlphaWolf,     15 },
        { EnemyId.Werewolf,      20 },
        { EnemyId.Revenant,      20 }
    };

    public static Dictionary<EnemyId, int> Level7 = new Dictionary<EnemyId, int>
    {
        { EnemyId.LeglessZombie,  1 },
        { EnemyId.WildBear,       1 },
        { EnemyId.Zombie,         5 },
        { EnemyId.Wolf,           5 },
        { EnemyId.ArmoredZombie, 10 },
        { EnemyId.GiantSpider,    5 },
        { EnemyId.PlagueGhoul,    5 },
        { EnemyId.AlphaWolf,     15 },
        { EnemyId.Werewolf,      25 },
        { EnemyId.Revenant,      25 }
    };

    public static Dictionary<EnemyId, int> Level8 = new Dictionary<EnemyId, int>
    {
        { EnemyId.LeglessZombie,  1 },
        { EnemyId.WildBear,       1 },
        { EnemyId.Zombie,         2 },
        { EnemyId.Wolf,           2 },
        { EnemyId.ArmoredZombie,  5 },
        { EnemyId.GiantSpider,   12 },
        { EnemyId.PlagueGhoul,   12 },
        { EnemyId.AlphaWolf,     10 },
        { EnemyId.Werewolf,      50 },
        { EnemyId.Revenant,      50 }
    };

    public static Dictionary<EnemyId, int> Level9 = new Dictionary<EnemyId, int>
    {
        { EnemyId.LeglessZombie,  1 },
        { EnemyId.WildBear,       1 },
        { EnemyId.Zombie,         2 },
        { EnemyId.Wolf,           2 },
        { EnemyId.ArmoredZombie,  5 },
        { EnemyId.GiantSpider,   15 },
        { EnemyId.PlagueGhoul,   15 },
        { EnemyId.AlphaWolf,     10 },
        { EnemyId.Werewolf,      50 },
        { EnemyId.Revenant,     100 }
    };

    public static Dictionary<EnemyId, int> Level10 = new Dictionary<EnemyId, int>
    {
        { EnemyId.LeglessZombie,  1 },
        { EnemyId.WildBear,       1 },
        { EnemyId.Zombie,         2 },
        { EnemyId.Wolf,           2 },
        { EnemyId.ArmoredZombie,  5 },
        { EnemyId.GiantSpider,   20 },
        { EnemyId.PlagueGhoul,   20 },
        { EnemyId.AlphaWolf,     10 },
        { EnemyId.Werewolf,      50 },
        { EnemyId.Revenant,     200 }
    };
}
