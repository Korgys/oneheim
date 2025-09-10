using Roguelike.Core.Game.Characters.Enemies.Bosses;
using Roguelike.Core.Game.Characters.Enemies.Mobs.Cultists;
using Roguelike.Core.Game.Characters.Enemies.Mobs.Humans;
using Roguelike.Core.Game.Characters.Enemies.Mobs.Outlaws;
using Roguelike.Core.Game.Characters.Enemies.Mobs.Undeads;
using Roguelike.Core.Game.Characters.Enemies.Mobs.Wilds;

namespace Roguelike.Core.Game.Characters.Enemies;

public class EnemyFactory
{
    private static readonly Random _random = new Random();

    public static Enemy CreateFromBag(IDictionary<EnemyId, int> bag, int x, int y, int danger)
    {
        // Roulette wheel selection
        int total = bag.Values.Sum();
        int roll = _random.Next(1, total + 1);
        int cumulative = 0;

        foreach (var kv in bag)
        {
            cumulative += kv.Value;
            if (roll <= cumulative)
            {
                return Create(kv.Key, x, y, danger);
            }
        }

        // Fallback (shouldn't happen)
        return Create(bag.First().Key, x, y, danger);
    }

    public static Enemy Create(EnemyId enemyId, int x, int y, int level)
    {
        return enemyId switch
        {
            // Undead
            EnemyId.LeglessZombie => new LeglessZombie(x, y, level),
            EnemyId.Skeleton => new Skeleton(x, y, level),
            EnemyId.Zombie => new Zombie(x, y, level),
            EnemyId.ArmoredZombie => new ArmoredZombie(x, y, level),
            EnemyId.PlagueGhoul => new PlagueGhoul(x, y, level),
            EnemyId.Revenant => new Revenant(x, y, level),

            EnemyId.Lich => new Lich(x, y, level),

            // Wild beasts
            EnemyId.WildLittleBear => new WildLittleBear(x, y, level),
            EnemyId.WildBear => new WildBear(x, y, level),
            EnemyId.Wolf => new Wolf(x, y, level),
            EnemyId.AlphaWolf => new AlphaWolf(x, y, level),
            EnemyId.GiantSpider => new SpiderNest(x, y, level),
            EnemyId.Werewolf => new Werewolf(x, y, level),
            
            EnemyId.Wyvern => new Wyvern(x, y, level),
            EnemyId.Troll => new Troll(x, y, level),

            // Outlaws
            EnemyId.Drunkard => new Drunkard(x, y, level),
            EnemyId.Pickpocket => new Pickpocket(x, y, level),
            EnemyId.Brigand => new Brigand(x, y, level),
            EnemyId.Mercenary => new GreedyMercenary(x, y, level),
            EnemyId.Assassin => new Assassin(x, y, level),
            EnemyId.WatchtowerArcher => new WatchtowerArcher(x, y, level),

            EnemyId.ChiefBrigand => new ChiefBrigand(x, y, level),
            EnemyId.YannTheSilent => new YannTheSilent(x, y, level),

            // Cultists
            EnemyId.Novice => new Novice(x, y, level),
            EnemyId.Acolyte => new Acolyte(x, y, level),
            EnemyId.Cultist => new Cultist(x, y, level),
            EnemyId.Zealot => new Zealot(x, y, level),
            EnemyId.Priest => new Priest(x, y, level),
            EnemyId.Champion => new Champion(x, y, level),

            EnemyId.HighPriest => new HighPriest(x, y, level),

            _ => throw new ArgumentOutOfRangeException(nameof(EnemyId), enemyId, null)
        };
    }
}
