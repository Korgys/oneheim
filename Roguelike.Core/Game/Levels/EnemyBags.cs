using Roguelike.Core.Game.Characters.Enemies;

namespace Roguelike.Core.Game.Levels;

public static class EnemyBags
{
    /// <summary>
    /// Retrieves a dictionary of enemy identifiers and their associated counts for the specified level and enemy types.
    /// </summary>
    /// <param name="level">The level for which to retrieve enemy data. Must be between 1 and 15, inclusive.</param>
    /// <param name="enemyTypes">A list of enemy types to filter the results.</param>
    /// <returns>A dictionary where the keys are enemy identifiers and the values are their respective counts for the specified
    /// level.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="level"/> is less than 1 or greater than 15.</exception>
    public static Dictionary<EnemyId, int> GetByLevelAndType(int level, List<EnemyType> enemyTypes)
    {
        if (level < 1 || level > 15)
            throw new ArgumentOutOfRangeException(nameof(level), "Level must be between 1 and 15.");

        var enemyIds = enemyTypes.SelectMany(EnemyTypeHelper.GetEnemyIdsByType).ToHashSet();

        return Levels[level - 1].Where(e => enemyIds.Contains(e.Key)).ToDictionary();
    }

    public static Dictionary<EnemyId, int>[] Levels =
    [
        new Dictionary<EnemyId, int> // Level 1
        {
            // Weak
            { EnemyId.LeglessZombie,     3 },
            { EnemyId.WildLittleBear,    3 },
            { EnemyId.Drunkard,          3 },
            { EnemyId.Novice,            3 },
            { EnemyId.Imp,               3 },
            // Mid-weak
            { EnemyId.Skeleton,          1 },
            { EnemyId.WildBear,          1 },
            { EnemyId.Pickpocket,        1 },
            { EnemyId.Acolyte,           1 },
            { EnemyId.DemonSlave,        1 },
        },
        new Dictionary<EnemyId, int> // Level 2
        {
            // Weak
            { EnemyId.LeglessZombie,    10 },
            { EnemyId.WildLittleBear,   10 },
            { EnemyId.Drunkard,         10 },
            { EnemyId.Novice,           10 },
            { EnemyId.Imp,              10 },
            // Mid-weak
            { EnemyId.Skeleton,          5 },
            { EnemyId.WildBear,          5 },
            { EnemyId.Pickpocket,        5 },
            { EnemyId.Acolyte,           5 },
            { EnemyId.DemonSlave,        5 },
            // Normal 
            { EnemyId.Zombie,            1 },
            { EnemyId.Wolf,              1 },
            { EnemyId.Brigand,           1 },
            { EnemyId.Cultist,           1 },
            { EnemyId.Hellhound,         1 },
        },
        new Dictionary<EnemyId, int> // Level 3
        {
            // Weak
            { EnemyId.LeglessZombie,     3 },
            { EnemyId.WildLittleBear,    3 },
            { EnemyId.Drunkard,          3 },
            { EnemyId.Novice,            3 },
            { EnemyId.Imp,               3 },
            // Mid-weak
            { EnemyId.Skeleton,          3 },
            { EnemyId.WildBear,          3 },
            { EnemyId.Pickpocket,        3 },
            { EnemyId.Acolyte,           3 },
            { EnemyId.DemonSlave,        3 },
            // Normal 
            { EnemyId.Zombie,            1 },
            { EnemyId.Wolf,              1 },
            { EnemyId.Brigand,           1 },
            { EnemyId.Cultist,           1 },
            { EnemyId.Hellhound,         1 },
        },
        new Dictionary<EnemyId, int> // Level 4
        {
            // Weak
            { EnemyId.LeglessZombie,     2 },
            { EnemyId.WildLittleBear,    2 },
            { EnemyId.Drunkard,          2 },
            { EnemyId.Novice,            2 },
            { EnemyId.Imp,               2 },
            // Mid-weak
            { EnemyId.Skeleton,          3 },
            { EnemyId.WildBear,          3 },
            { EnemyId.Pickpocket,        3 },
            { EnemyId.Acolyte,           3 },
            { EnemyId.DemonSlave,        3 },
            // Normal 
            { EnemyId.Zombie,            5 },
            { EnemyId.Wolf,              5 },
            { EnemyId.Brigand,           5 },
            { EnemyId.Cultist,           5 },
            { EnemyId.Hellhound,         5 },
            // Static (Strong)
            { EnemyId.PlagueGhoul,       1 },
            { EnemyId.GiantSpider,       1 },
            { EnemyId.Priest,            1 },
            { EnemyId.WatchtowerArcher,  1 },
            { EnemyId.HellObelisk,       1 },
        },
        new Dictionary<EnemyId, int> // Level 5
        {
            // Mid-weak
            { EnemyId.Skeleton,          2 },
            { EnemyId.WildBear,          2 },
            { EnemyId.Pickpocket,        2 },
            { EnemyId.Acolyte,           2 },
            { EnemyId.DemonSlave,        2 },
            // Normal 
            { EnemyId.Zombie,            5 },
            { EnemyId.Wolf,              5 },
            { EnemyId.Brigand,           5 },
            { EnemyId.Cultist,           5 },
            { EnemyId.Hellhound,         5 },
            // Static (Strong)
            { EnemyId.PlagueGhoul,       1 },
            { EnemyId.GiantSpider,       1 },
            { EnemyId.Priest,            1 },
            { EnemyId.WatchtowerArcher,  1 },
            { EnemyId.HellObelisk,       1 },
        },
        new Dictionary<EnemyId, int> // Level 6
        {
            // Mid-weak
            { EnemyId.Skeleton,          2 },
            { EnemyId.WildBear,          2 },
            { EnemyId.Pickpocket,        2 },
            { EnemyId.Acolyte,           2 },
            { EnemyId.DemonSlave,        2 },
            // Normal 
            { EnemyId.Zombie,            6 },
            { EnemyId.Wolf,              6 },
            { EnemyId.Brigand,           6 },
            { EnemyId.Cultist,           6 },
            { EnemyId.Hellhound,         6 },            
            // Mid strong
            { EnemyId.ArmoredZombie,     1 },
            { EnemyId.AlphaWolf,         1 },
            { EnemyId.Mercenary,         1 },
            { EnemyId.Zealot,            1 },
            { EnemyId.Overseer,          1 },
            // Static (Strong)
            { EnemyId.PlagueGhoul,       1 },
            { EnemyId.GiantSpider,       1 },
            { EnemyId.Priest,            1 },
            { EnemyId.WatchtowerArcher,  1 },
            { EnemyId.HellObelisk,       1 },
        },
        new Dictionary<EnemyId, int> // Level 7
        {
            // Normal 
            { EnemyId.Zombie,            6 },
            { EnemyId.Wolf,              6 },
            { EnemyId.Brigand,           6 },
            { EnemyId.Cultist,           6 },
            { EnemyId.Hellhound,         6 },            
            // Mid strong
            { EnemyId.ArmoredZombie,     3 },
            { EnemyId.AlphaWolf,         3 },
            { EnemyId.Mercenary,         3 },
            { EnemyId.Zealot,            3 },
            { EnemyId.Overseer,          3 },
            // Static (Strong)
            { EnemyId.PlagueGhoul,       1 },
            { EnemyId.GiantSpider,       1 },
            { EnemyId.Priest,            1 },
            { EnemyId.WatchtowerArcher,  1 },
            { EnemyId.HellObelisk,       1 },
        },
        new Dictionary<EnemyId, int> // Level 8
        {
            // Static (Strong)
            { EnemyId.PlagueGhoul,       1 },
            { EnemyId.GiantSpider,       1 },
            { EnemyId.Priest,            1 },
            { EnemyId.WatchtowerArcher,  1 },
            { EnemyId.HellObelisk,       1 },
        },
        new Dictionary<EnemyId, int> // Level 9
        {
            // Normal 
            { EnemyId.Zombie,            1 },
            { EnemyId.Wolf,              1 },
            { EnemyId.Brigand,           1 },
            { EnemyId.Cultist,           1 },
            { EnemyId.Hellhound,         1 },            
            // Mid strong
            { EnemyId.ArmoredZombie,     3 },
            { EnemyId.AlphaWolf,         3 },
            { EnemyId.Mercenary,         3 },
            { EnemyId.Zealot,            3 },
            { EnemyId.Overseer,          3 },
            // Static (Strong)
            { EnemyId.PlagueGhoul,       1 },
            { EnemyId.GiantSpider,       1 },
            { EnemyId.Priest,            1 },
            { EnemyId.WatchtowerArcher,  1 },
            { EnemyId.HellObelisk,       1 },
            // Strong
            { EnemyId.Revenant,          1 },
            { EnemyId.Werewolf,          1 },
            { EnemyId.Champion,          1 },
            { EnemyId.Assassin,          1 },
            { EnemyId.DoomReaper,        1 },
        },
        new Dictionary<EnemyId, int> // Level 10
        {
            // Normal 
            { EnemyId.Zombie,            1 },
            { EnemyId.Wolf,              1 },
            { EnemyId.Brigand,           1 },
            { EnemyId.Cultist,           1 },
            { EnemyId.Hellhound,         1 },            
            // Mid strong
            { EnemyId.ArmoredZombie,     1 },
            { EnemyId.AlphaWolf,         1 },
            { EnemyId.Mercenary,         1 },
            { EnemyId.Zealot,            1 },
            { EnemyId.Overseer,          1 },
            // Static (Strong)
            { EnemyId.PlagueGhoul,       1 },
            { EnemyId.GiantSpider,       1 },
            { EnemyId.Priest,            1 },
            { EnemyId.WatchtowerArcher,  1 },
            { EnemyId.HellObelisk,       1 },
            // Strong
            { EnemyId.Revenant,          1 },
            { EnemyId.Werewolf,          1 },
            { EnemyId.Champion,          1 },
            { EnemyId.Assassin,          1 },
            { EnemyId.DoomReaper,        1 },
        },
        new Dictionary<EnemyId, int> // Level 11
        {       
            // Mid strong
            { EnemyId.ArmoredZombie,     2 },
            { EnemyId.AlphaWolf,         2 },
            { EnemyId.Mercenary,         2 },
            { EnemyId.Zealot,            2 },
            { EnemyId.Overseer,          2 },
            // Static (Strong)
            { EnemyId.PlagueGhoul,       1 },
            { EnemyId.GiantSpider,       1 },
            { EnemyId.Priest,            1 },
            { EnemyId.WatchtowerArcher,  1 },
            { EnemyId.HellObelisk,       1 },
            // Strong
            { EnemyId.Revenant,          1 },
            { EnemyId.Werewolf,          1 },
            { EnemyId.Champion,          1 },
            { EnemyId.Assassin,          1 },
            { EnemyId.DoomReaper,        1 },
        },
        new Dictionary<EnemyId, int> // Level 12
        {
            // Mid strong
            { EnemyId.ArmoredZombie,     1 },
            { EnemyId.AlphaWolf,         1 },
            { EnemyId.Mercenary,         1 },
            { EnemyId.Zealot,            1 },
            { EnemyId.Overseer,          1 },
            // Static (Strong)
            { EnemyId.PlagueGhoul,       1 },
            { EnemyId.GiantSpider,       1 },
            { EnemyId.Priest,            1 },
            { EnemyId.WatchtowerArcher,  1 },
            { EnemyId.HellObelisk,       1 },
            // Strong
            { EnemyId.Revenant,          1 },
            { EnemyId.Werewolf,          1 },
            { EnemyId.Champion,          1 },
            { EnemyId.Assassin,          1 },
            { EnemyId.DoomReaper,        1 },
        },
        new Dictionary<EnemyId, int> // Level 13
        {
            // Static (Strong)
            { EnemyId.PlagueGhoul,       1 },
            { EnemyId.GiantSpider,       1 },
            { EnemyId.Priest,            1 },
            { EnemyId.WatchtowerArcher,  1 },
            { EnemyId.HellObelisk,       1 },
            // Strong
            { EnemyId.Revenant,          1 },
            { EnemyId.Werewolf,          1 },
            { EnemyId.Champion,          1 },
            { EnemyId.Assassin,          1 },
            { EnemyId.DoomReaper,        1 },
        },
        new Dictionary<EnemyId, int> // Level 14
        {
            // Strong
            { EnemyId.Revenant,          1 },
            { EnemyId.Werewolf,          1 },
            { EnemyId.Champion,          1 },
            { EnemyId.Assassin,          1 },
            { EnemyId.DoomReaper,        1 },
        },
        new Dictionary<EnemyId, int> // Level 15
        {
            // Strong
            { EnemyId.Revenant,          1 },
            { EnemyId.Werewolf,          1 },
            { EnemyId.Champion,          1 },
            { EnemyId.Assassin,          1 },
            { EnemyId.DoomReaper,        1 },
        }
    ];
}
