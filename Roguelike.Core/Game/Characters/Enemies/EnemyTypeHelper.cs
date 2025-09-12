namespace Roguelike.Core.Game.Characters.Enemies;

public static class EnemyTypeHelper
{
    private static readonly Random _random = new();

    // Pondération des catégories
    public static readonly Dictionary<EnemyType, int> Weights = new()
    {
        { EnemyType.Undead,     5 },
        { EnemyType.Wild,       5 },
        { EnemyType.Outlaws,    4 },
        { EnemyType.Cultist,    4 },
        { EnemyType.Demon,      3 } 
    };

    /// <summary>
    /// Generates a list of random enemy types based on predefined weights.
    /// </summary>
    /// <remarks>The method ensures that the selection of enemy types is influenced by predefined weights, 
    /// with higher-weighted types being more likely to appear. The method avoids immediate duplicates  by temporarily
    /// removing selected types from the pool, and the pool is replenished as needed.</remarks>
    /// <param name="size">The number of enemy types to include in the generated list. Must be greater than 0.</param>
    /// <returns>A list of <see cref="EnemyType"/> objects representing the randomly selected enemy types.  If <paramref
    /// name="size"/> is less than or equal to 0, an empty list is returned.</returns>
    public static List<EnemyType> GetRandomEnemyTypesBag(int size)
    {
        if (size <= 0) return new List<EnemyType>();

        // On construit une "liste pondérée" avec répétitions virtuelles
        var weightedList = new List<EnemyType>();
        foreach (var kv in Weights)
            for (int i = 0; i < kv.Value; i++)
                weightedList.Add(kv.Key);

        var bag = new List<EnemyType>();
        var pool = new List<EnemyType>(weightedList.Distinct()); // pas de doublons initiaux

        for (int i = 0; i < size; i++)
        {
            if (pool.Count == 0) pool = new List<EnemyType>(weightedList.Distinct()); // on recharge

            // Tirage pondéré
            var choice = GetWeightedRandom(weightedList, bag);
            bag.Add(choice);
            pool.Remove(choice); // évite le doublon immédiat
        }

        return bag;
    }

    /// <summary>
    /// Get the list of EnemyIds for a given EnemyType
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static List<EnemyId> GetEnemyIdsByType(EnemyType type)
    {
        return type switch
        {
            EnemyType.Undead => new List<EnemyId>
            {
                EnemyId.LeglessZombie,
                EnemyId.Skeleton,
                EnemyId.Zombie,
                EnemyId.ArmoredZombie,
                EnemyId.PlagueGhoul,
                EnemyId.Revenant
            },
            EnemyType.Wild => new List<EnemyId>
            {
                EnemyId.WildLittleBear,
                EnemyId.WildBear,
                EnemyId.Wolf,
                EnemyId.AlphaWolf,
                EnemyId.GiantSpider,
                EnemyId.Werewolf
            },
            EnemyType.Outlaws => new List<EnemyId>
            {
                EnemyId.Drunkard,
                EnemyId.Pickpocket,
                EnemyId.Brigand,
                EnemyId.Mercenary,
                EnemyId.WatchtowerArcher,
                EnemyId.Assassin
            },
            EnemyType.Cultist => new List<EnemyId>
            {
                EnemyId.Novice,
                EnemyId.Acolyte,
                EnemyId.Cultist,
                EnemyId.Zealot,
                EnemyId.Priest,
                EnemyId.Champion
            },
            EnemyType.Demon => new List<EnemyId>
            {
                EnemyId.Imp,
                EnemyId.DemonSlave,
                EnemyId.Hellhound,
                EnemyId.Overseer,
                EnemyId.HellObelisk,
                EnemyId.DoomReaper
            },
            _ => new List<EnemyId>()
        };
    }

    /// <summary>
    /// Sélectionne un type pondéré, en excluant ceux déjà tirés
    /// </summary>
    /// <param name="weightedList"></param>
    /// <param name="excluded"></param>
    /// <returns></returns>
    private static EnemyType GetWeightedRandom(List<EnemyType> weightedList, List<EnemyType> excluded)
    {
        var filtered = weightedList.Where(t => !excluded.Contains(t)).ToList();
        int index = _random.Next(filtered.Count);
        return filtered[index];
    }
}
