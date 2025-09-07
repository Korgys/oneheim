using Roguelike.Core.Game.Characters.Enemies;

namespace Roguelike.Core.Game.Levels;

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
            11 => Level11,
            12 => Level12,
            13 => Level13,
            14 => Level14,
            15 => Level15,
            _ => throw new ArgumentOutOfRangeException(nameof(level), "Invalid level")
        };
    }

    public static Dictionary<EnemyId, int>  Level1 = new Dictionary<EnemyId, int>
    {
        // weak ennemies
        { EnemyId.LeglessZombie,    85 },
        { EnemyId.WildBear,         99 },
        { EnemyId.Cultist,          99 },
        { EnemyId.Drunkard,         99 },
        // normal ennemies
        { EnemyId.Zombie,           15 },        
        { EnemyId.Wolf,             20 },
        { EnemyId.Acolyte,          20 },
        { EnemyId.Brigand,          20 },
        // strong ennemies
        { EnemyId.ArmoredZombie,     1 },
        { EnemyId.AlphaWolf,         1 },
        { EnemyId.Zealot,            1 },
        { EnemyId.Mercenary,         1 },
        // static ennemies (strong)
        { EnemyId.PlagueGhoul,       5 },
        { EnemyId.GiantSpider,       5 },
        { EnemyId.Priest,            5 },
        { EnemyId.WatchtowerArcher,  5 },
    };

    public static Dictionary<EnemyId, int> Level2 = new Dictionary<EnemyId, int>
    {
        // weak ennemies
        { EnemyId.LeglessZombie,    35 },
        { EnemyId.WildBear,         40 },
        { EnemyId.Cultist,          40 },
        { EnemyId.Drunkard,         40 },
        // normal ennemies
        { EnemyId.Zombie,           18 },
        { EnemyId.Wolf,             20 },
        { EnemyId.Acolyte,          20 },
        { EnemyId.Brigand,          20 },
        // strong ennemies
        { EnemyId.ArmoredZombie,     1 },
        { EnemyId.AlphaWolf,         1 },
        { EnemyId.Zealot,            1 },
        { EnemyId.Mercenary,         1 },
        // static ennemies (strong)
        { EnemyId.PlagueGhoul,       4 },
        { EnemyId.GiantSpider,       4 },
        { EnemyId.Priest,            4 },
        { EnemyId.WatchtowerArcher,  4 },
    };

    public static Dictionary<EnemyId, int> Level3 = new Dictionary<EnemyId, int>
    {
        // weak ennemies
        { EnemyId.LeglessZombie,    18 },
        { EnemyId.WildBear,         18 },
        { EnemyId.Cultist,          18 },
        { EnemyId.Drunkard,         18 },
        // normal ennemies
        { EnemyId.Zombie,           14 },
        { EnemyId.Wolf,             14 },
        { EnemyId.Acolyte,          14 },
        { EnemyId.Brigand,          14 },
        // strong ennemies
        { EnemyId.ArmoredZombie,     1 },
        { EnemyId.AlphaWolf,         1 },
        { EnemyId.Zealot,            1 },
        { EnemyId.Mercenary,         1 },
        // static ennemies (strong)
        { EnemyId.PlagueGhoul,       3 },
        { EnemyId.GiantSpider,       3 },
        { EnemyId.Priest,            3 },
        { EnemyId.WatchtowerArcher,  3 },
    };

    public static Dictionary<EnemyId, int> Level4 = new Dictionary<EnemyId, int>
    {
        // weak ennemies
        { EnemyId.LeglessZombie,     2 },
        { EnemyId.WildBear,          2 },
        { EnemyId.Cultist,           2 },
        { EnemyId.Drunkard,          2 },
        // normal ennemies
        { EnemyId.Zombie,            7 },
        { EnemyId.Wolf,              7 },
        { EnemyId.Acolyte,           7 },
        { EnemyId.Brigand,           7 },
        // strong ennemies
        { EnemyId.ArmoredZombie,     1 },
        { EnemyId.AlphaWolf,         1 },
        { EnemyId.Zealot,            1 },
        { EnemyId.Mercenary,         1 },
        // static ennemies (strong)
        { EnemyId.PlagueGhoul,       2 },
        { EnemyId.GiantSpider,       2 },
        { EnemyId.Priest,            2 },
        { EnemyId.WatchtowerArcher,  2 },
    };

    public static Dictionary<EnemyId, int> Level5 = new Dictionary<EnemyId, int>
    {
        // weak ennemies
        { EnemyId.LeglessZombie,     2 },
        { EnemyId.WildBear,          2 },
        { EnemyId.Cultist,           2 },
        { EnemyId.Drunkard,          2 },
        // normal ennemies
        { EnemyId.Zombie,           10 },
        { EnemyId.Wolf,             10 },
        { EnemyId.Acolyte,          10 },
        { EnemyId.Brigand,          10 },
        // strong ennemies
        { EnemyId.ArmoredZombie,    14 },
        { EnemyId.AlphaWolf,        14 },
        { EnemyId.Zealot,           14 },
        { EnemyId.Mercenary,        14 },
        // static ennemies (strong)
        { EnemyId.PlagueGhoul,       3 },
        { EnemyId.GiantSpider,       3 },
        { EnemyId.Priest,            3 },
        { EnemyId.WatchtowerArcher,  3 },
        // really strong ennemies
        { EnemyId.Revenant,          1 },
        { EnemyId.Werewolf,          1 },
        { EnemyId.Champion,          1 },
        { EnemyId.Assassin,          1 },
    };

    public static Dictionary<EnemyId, int> Level6 = new Dictionary<EnemyId, int>
    {
        // weak ennemies
        { EnemyId.LeglessZombie,     1 },
        { EnemyId.WildBear,          1 },
        { EnemyId.Cultist,           1 },
        { EnemyId.Drunkard,          1 },
        // normal ennemies
        { EnemyId.Zombie,            8 },
        { EnemyId.Wolf,              8 },
        { EnemyId.Acolyte,           8 },
        { EnemyId.Brigand,           8 },
        // strong ennemies
        { EnemyId.ArmoredZombie,    14 },
        { EnemyId.AlphaWolf,        14 },
        { EnemyId.Zealot,           14 },
        { EnemyId.Mercenary,        14 },
        // static ennemies (strong)
        { EnemyId.PlagueGhoul,       4 },
        { EnemyId.GiantSpider,       4 },
        { EnemyId.Priest,            4 },
        { EnemyId.WatchtowerArcher,  4 },
        // really strong ennemies
        { EnemyId.Revenant,          2 },
        { EnemyId.Werewolf,          2 },
        { EnemyId.Champion,          2 },
        { EnemyId.Assassin,          2 },
    };

    public static Dictionary<EnemyId, int> Level7 = new Dictionary<EnemyId, int>
    {
        // weak ennemies
        { EnemyId.LeglessZombie,     1 },
        { EnemyId.WildBear,          1 },
        { EnemyId.Cultist,           1 },
        { EnemyId.Drunkard,          1 },
        // normal ennemies
        { EnemyId.Zombie,            5 },
        { EnemyId.Wolf,              5 },
        { EnemyId.Acolyte,           5 },
        { EnemyId.Brigand,           5 },
        // strong ennemies
        { EnemyId.ArmoredZombie,    17 },
        { EnemyId.AlphaWolf,        17 },
        { EnemyId.Zealot,           17 },
        { EnemyId.Mercenary,        17 },
        // static ennemies (strong)
        { EnemyId.PlagueGhoul,       4 },
        { EnemyId.GiantSpider,       4 },
        { EnemyId.Priest,            4 },
        { EnemyId.WatchtowerArcher,  4 },
        // really strong ennemies
        { EnemyId.Revenant,          3 },
        { EnemyId.Werewolf,          3 },
        { EnemyId.Champion,          3 },
        { EnemyId.Assassin,          3 },
    };

    public static Dictionary<EnemyId, int> Level8 = new Dictionary<EnemyId, int>
    {
        // normal ennemies
        { EnemyId.Zombie,            1 },
        { EnemyId.Wolf,              1 },
        { EnemyId.Acolyte,           1 },
        { EnemyId.Brigand,           1 },
        // strong ennemies
        { EnemyId.ArmoredZombie,    15 },
        { EnemyId.AlphaWolf,        15 },
        { EnemyId.Zealot,           15 },
        { EnemyId.Mercenary,        15 },
        // static ennemies (strong)
        { EnemyId.PlagueGhoul,       5 },
        { EnemyId.GiantSpider,       5 },
        { EnemyId.Priest,            5 },
        { EnemyId.WatchtowerArcher,  5 },
        // really strong ennemies
        { EnemyId.Revenant,         10 },
        { EnemyId.Werewolf,         10 },
        { EnemyId.Champion,         10 },
        { EnemyId.Assassin,         10 },
    };

    public static Dictionary<EnemyId, int> Level9 = new Dictionary<EnemyId, int>
    {
        // strong ennemies
        { EnemyId.ArmoredZombie,    15 },
        { EnemyId.AlphaWolf,        15 },
        { EnemyId.Zealot,           15 },
        { EnemyId.Mercenary,        15 },
        // static ennemies (strong)
        { EnemyId.PlagueGhoul,       5 },
        { EnemyId.GiantSpider,       5 },
        { EnemyId.Priest,            5 },
        { EnemyId.WatchtowerArcher,  5 },
        // really strong ennemies
        { EnemyId.Revenant,         15 },
        { EnemyId.Werewolf,         15 },
        { EnemyId.Champion,         15 },
        { EnemyId.Assassin,         15 },
    };

    public static Dictionary<EnemyId, int> Level10 = new Dictionary<EnemyId, int>
    {
        // static ennemies (strong)
        { EnemyId.PlagueGhoul,       1 },
        { EnemyId.GiantSpider,       1 },
        { EnemyId.Priest,            1 },
        { EnemyId.WatchtowerArcher,  1 },
        // really strong ennemies
        { EnemyId.Revenant,          4 },
        { EnemyId.Werewolf,          4 },
        { EnemyId.Champion,          4 },
        { EnemyId.Assassin,          4 },
    };

    public static Dictionary<EnemyId, int> Level11 = new Dictionary<EnemyId, int>
    {
        // static ennemies (strong)
        { EnemyId.PlagueGhoul,       1 },
        { EnemyId.GiantSpider,       1 },
        { EnemyId.Priest,            1 },
        { EnemyId.WatchtowerArcher,  1 },
        // really strong ennemies
        { EnemyId.Revenant,          4 },
        { EnemyId.Werewolf,          4 },
        { EnemyId.Champion,          4 },
        { EnemyId.Assassin,          4 },
    };

    public static Dictionary<EnemyId, int> Level12 = new Dictionary<EnemyId, int>
    {
        // static ennemies (strong)
        { EnemyId.PlagueGhoul,       1 },
        { EnemyId.GiantSpider,       1 },
        { EnemyId.Priest,            1 },
        { EnemyId.WatchtowerArcher,  1 },
        // really strong ennemies
        { EnemyId.Revenant,          4 },
        { EnemyId.Werewolf,          4 },
        { EnemyId.Champion,          4 },
        { EnemyId.Assassin,          4 },
    };

    public static Dictionary<EnemyId, int> Level13 = new Dictionary<EnemyId, int>
    {
        // static ennemies (strong)
        { EnemyId.PlagueGhoul,       1 },
        { EnemyId.GiantSpider,       1 },
        { EnemyId.Priest,            1 },
        { EnemyId.WatchtowerArcher,  1 },
        // really strong ennemies
        { EnemyId.Revenant,          4 },
        { EnemyId.Werewolf,          4 },
        { EnemyId.Champion,          4 },
        { EnemyId.Assassin,          4 },
    };

    public static Dictionary<EnemyId, int> Level14 = new Dictionary<EnemyId, int>
    {
        // static ennemies (strong)
        { EnemyId.PlagueGhoul,       1 },
        { EnemyId.GiantSpider,       1 },
        { EnemyId.Priest,            1 },
        { EnemyId.WatchtowerArcher,  1 },
        // really strong ennemies
        { EnemyId.Revenant,          4 },
        { EnemyId.Werewolf,          4 },
        { EnemyId.Champion,          4 },
        { EnemyId.Assassin,          4 },
    };

    public static Dictionary<EnemyId, int> Level15 = new Dictionary<EnemyId, int>
    {
        // static ennemies (strong)
        { EnemyId.PlagueGhoul,       1 },
        { EnemyId.GiantSpider,       1 },
        { EnemyId.Priest,            1 },
        { EnemyId.WatchtowerArcher,  1 },
        // really strong ennemies
        { EnemyId.Revenant,          4 },
        { EnemyId.Werewolf,          4 },
        { EnemyId.Champion,          4 },
        { EnemyId.Assassin,          4 },
    };
}
