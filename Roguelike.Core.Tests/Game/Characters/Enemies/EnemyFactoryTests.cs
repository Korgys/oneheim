using Roguelike.Core.Game.Characters.Enemies;
using Roguelike.Core.Game.Characters.Enemies.Bosses;
using Roguelike.Core.Game.Characters.Enemies.Mobs.Cultists;
using Roguelike.Core.Game.Characters.Enemies.Mobs.Demons;
using Roguelike.Core.Game.Characters.Enemies.Mobs.Humans;
using Roguelike.Core.Game.Characters.Enemies.Mobs.Outlaws;
using Roguelike.Core.Game.Characters.Enemies.Mobs.Undeads;
using Roguelike.Core.Game.Characters.Enemies.Mobs.Wilds;

namespace Roguelike.Core.Tests.Game.Characters.Enemies;

[TestClass]
public class EnemyFactoryTests
{
    private const int X = 3;
    private const int Y = 7;
    private const int Level = 5;

    // ---- Create: mapping complet EnemyId -> Type ----

    [DataTestMethod]
    // Undead
    [DataRow(EnemyId.LeglessZombie, typeof(LeglessZombie))]
    [DataRow(EnemyId.Skeleton, typeof(Skeleton))]
    [DataRow(EnemyId.Zombie, typeof(Zombie))]
    [DataRow(EnemyId.ArmoredZombie, typeof(ArmoredZombie))]
    [DataRow(EnemyId.PlagueGhoul, typeof(PlagueGhoul))]
    [DataRow(EnemyId.Revenant, typeof(Revenant))]
    [DataRow(EnemyId.Lich, typeof(Lich))]

    // Wild beasts
    [DataRow(EnemyId.WildLittleBear, typeof(WildLittleBear))]
    [DataRow(EnemyId.WildBear, typeof(WildBear))]
    [DataRow(EnemyId.Wolf, typeof(Wolf))]
    [DataRow(EnemyId.AlphaWolf, typeof(AlphaWolf))]
    [DataRow(EnemyId.GiantSpider, typeof(SpiderNest))]
    [DataRow(EnemyId.Werewolf, typeof(Werewolf))]
    [DataRow(EnemyId.Wyvern, typeof(Wyvern))]
    [DataRow(EnemyId.Troll, typeof(Troll))]

    // Outlaws
    [DataRow(EnemyId.Drunkard, typeof(Drunkard))]
    [DataRow(EnemyId.Pickpocket, typeof(Pickpocket))]
    [DataRow(EnemyId.Brigand, typeof(Brigand))]
    [DataRow(EnemyId.Mercenary, typeof(GreedyMercenary))]
    [DataRow(EnemyId.Assassin, typeof(Assassin))]
    [DataRow(EnemyId.WatchtowerArcher, typeof(WatchtowerArcher))]
    [DataRow(EnemyId.ChiefBrigand, typeof(ChiefBrigand))]
    [DataRow(EnemyId.YannTheSilent, typeof(YannTheSilent))]

    // Cultists
    [DataRow(EnemyId.Novice, typeof(Novice))]
    [DataRow(EnemyId.Acolyte, typeof(Acolyte))]
    [DataRow(EnemyId.Cultist, typeof(Cultist))]
    [DataRow(EnemyId.Zealot, typeof(Zealot))]
    [DataRow(EnemyId.Priest, typeof(Priest))]
    [DataRow(EnemyId.Champion, typeof(Champion))]
    [DataRow(EnemyId.HighPriest, typeof(HighPriest))]

    // Demons
    [DataRow(EnemyId.Imp, typeof(Imp))]
    [DataRow(EnemyId.DemonSlave, typeof(DemonSlave))]
    [DataRow(EnemyId.Hellhound, typeof(Hellhound))]
    [DataRow(EnemyId.Overseer, typeof(Overseer))]
    [DataRow(EnemyId.HellObelisk, typeof(HellObelisk))]
    [DataRow(EnemyId.DoomReaper, typeof(DoomReaper))]
    [DataRow(EnemyId.DartTheSoulbound, typeof(DartTheSoulbound))]
    [DataRow(EnemyId.AzrakelTheForsaken, typeof(AzrakelTheForsaken))]
    public void Create_ReturnsExpectedType(EnemyId id, Type expectedType)
    {
        var enemy = EnemyFactory.Create(id, X, Y, Level);
        Assert.IsNotNull(enemy);
        Assert.IsInstanceOfType(enemy, expectedType);
    }

    [TestMethod]
    public void Create_UnknownEnemyId_ThrowsArgumentOutOfRange()
    {
        var invalidId = (EnemyId)999999;

        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => EnemyFactory.Create(invalidId, X, Y, Level));
    }
    
    [TestMethod]
    public void CreateFromBag_SingleEntryBag_AlwaysReturnsThatType()
    {
        var bag = new Dictionary<EnemyId, int>
        {
            { EnemyId.Wolf, 1 }
        };

        var enemy = EnemyFactory.CreateFromBag(bag, X, Y, Level);

        Assert.IsNotNull(enemy);
        Assert.IsInstanceOfType<Wolf>(enemy);
    }

    [TestMethod]
    public void CreateFromBag_MultiEntryBag_ReturnsOneOfBagTypes()
    {
        // Arrange
        var allowed = new[]
        {
            EnemyId.Wolf, EnemyId.Skeleton, EnemyId.Novice
        };
        var bag = new Dictionary<EnemyId, int>
        {
            { EnemyId.Wolf, 1 },
            { EnemyId.Skeleton, 2 },
            { EnemyId.Novice, 3 }
        };

        // Act – on répète quelques fois à cause de l’aléatoire
        var results = new HashSet<Type>();
        for (int i = 0; i < 20; i++)
        {
            var enemy = EnemyFactory.CreateFromBag(bag, X, Y, Level);
            results.Add(enemy.GetType());
        }

        // Assert – tous les types observés font partie des clés autorisées
        var allowedTypes = allowed.Select(id => EnemyFactory.Create(id, X, Y, Level).GetType()).ToHashSet();
        foreach (var t in results)
        {
            Assert.IsTrue(allowedTypes.Contains(t), $"Type inattendu dans le résultat: {t.FullName}");
        }
    }

    [TestMethod]
    public void CreateFromBag_EmptyBag_ThrowsException()
    {
        var bag = new Dictionary<EnemyId, int>();

        // total == 0 fait échouer Random.Next(1, total+1) => ArgumentOutOfRangeException attendu
        Assert.ThrowsException<InvalidOperationException>(
            () => EnemyFactory.CreateFromBag(bag, X, Y, Level));
    }
}

