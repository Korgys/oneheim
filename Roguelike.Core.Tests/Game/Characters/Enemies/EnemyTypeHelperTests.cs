using Roguelike.Core.Game.Characters.Enemies;

namespace Roguelike.Core.Tests.Game.Characters.Enemies;

[TestClass]
public class EnemyTypeHelperTests
{
    [TestMethod]
    public void GetRandomEnemyTypesBag_InvalidSize_Throws()
    {
        int enumCount = Enum.GetNames<EnemyType>().Length;

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            EnemyTypeHelper.GetRandomEnemyTypesBag(-1));

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            EnemyTypeHelper.GetRandomEnemyTypesBag(0));

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            EnemyTypeHelper.GetRandomEnemyTypesBag(enumCount));

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            EnemyTypeHelper.GetRandomEnemyTypesBag(enumCount + 1));
    }

    [TestMethod]
    public void GetRandomEnemyTypesBag_ValidSize_ReturnsThatManyDistinctValidTypes()
    {
        int enumCount = Enum.GetNames<EnemyType>().Length;
        // valid size is any 1..(enumCount-1)
        int size = Math.Max(1, enumCount - 2);

        var result = EnemyTypeHelper.GetRandomEnemyTypesBag(size);

        Assert.IsNotNull(result);
        Assert.AreEqual(size, result.Count, "Returned bag does not match requested size.");

        // With current implementation, previously-picked types are excluded,
        // so all items should be distinct.
        Assert.AreEqual(size, result.Distinct().Count(), "Bag should not contain duplicates with current exclusion logic.");

        // All must be known/valid types present in the weights dictionary (and not Unknown).
        var validTypes = EnemyTypeHelper.Weights.Keys.ToHashSet();
        foreach (var t in result)
        {
            Assert.AreNotEqual(EnemyType.Unknown, t, "Bag must not contain EnemyType.Unknown.");
            Assert.IsTrue(validTypes.Contains(t), $"Bag contains an invalid EnemyType: {t}");
        }
    }

    [TestMethod]
    public void GetRandomEnemyTypesBag_NoImmediateDuplicates_AndNoUnknown()
    {
        int enumCount = Enum.GetNames<EnemyType>().Length;
        int size = Math.Max(2, enumCount - 1); // take the max valid size to stress the path

        var bag = EnemyTypeHelper.GetRandomEnemyTypesBag(size);

        // No immediate duplicates
        for (int i = 1; i < bag.Count; i++)
        {
            Assert.AreNotEqual(bag[i - 1], bag[i], $"Immediate duplicate at indices {i - 1} and {i}");
        }

        // No Unknown
        Assert.IsFalse(bag.Contains(EnemyType.Unknown), "Bag must not contain EnemyType.Unknown.");
    }

    [TestMethod]
    public void GetEnemyIdsByType_ReturnsExpectedLists()
    {
        // Undead
        var undead = EnemyTypeHelper.GetEnemyIdsByType(EnemyType.Undead);
        CollectionAssert.AreEquivalent(
            new List<EnemyId>
            {
                EnemyId.LeglessZombie,
                EnemyId.Skeleton,
                EnemyId.Zombie,
                EnemyId.ZombieCorpses,
                EnemyId.ArmoredZombie,
                EnemyId.PlagueGhoul,
                EnemyId.Revenant
            },
            undead,
            "Undead enemy IDs mismatch."
        );

        // Wild
        var wild = EnemyTypeHelper.GetEnemyIdsByType(EnemyType.Wild);
        CollectionAssert.AreEquivalent(
            new List<EnemyId>
            {
                EnemyId.WildLittleBear,
                EnemyId.WildBear,
                EnemyId.Wolf,
                EnemyId.RatsNest,
                EnemyId.AlphaWolf,
                EnemyId.SpiderNest,
                EnemyId.Werewolf
            },
            wild,
            "Wild enemy IDs mismatch."
        );

        // Outlaws
        var outlaws = EnemyTypeHelper.GetEnemyIdsByType(EnemyType.Outlaws);
        CollectionAssert.AreEquivalent(
            new List<EnemyId>
            {
                EnemyId.Drunkard,
                EnemyId.Pickpocket,
                EnemyId.Brigand,
                EnemyId.OutlawSentinel,
                EnemyId.Mercenary,
                EnemyId.WatchtowerArcher,
                EnemyId.Assassin
            },
            outlaws,
            "Outlaws enemy IDs mismatch."
        );

        // Cultist
        var cultist = EnemyTypeHelper.GetEnemyIdsByType(EnemyType.Cultist);
        CollectionAssert.AreEquivalent(
            new List<EnemyId>
            {
                EnemyId.Novice,
                EnemyId.Acolyte,
                EnemyId.Cultist,
                EnemyId.YoungPriest,
                EnemyId.Zealot,
                EnemyId.Priest,
                EnemyId.Champion
            },
            cultist,
            "Cultist enemy IDs mismatch."
        );

        // Demon
        var demon = EnemyTypeHelper.GetEnemyIdsByType(EnemyType.Demon);
        CollectionAssert.AreEquivalent(
            new List<EnemyId>
            {
                EnemyId.Imp,
                EnemyId.DemonSlave,
                EnemyId.Hellhound,
                EnemyId.HellStele,
                EnemyId.Overseer,
                EnemyId.HellObelisk,
                EnemyId.DoomReaper
            },
            demon,
            "Demon enemy IDs mismatch."
        );

        // Default / unknown
        var none = EnemyTypeHelper.GetEnemyIdsByType((EnemyType)999);
        Assert.IsNotNull(none);
        Assert.AreEqual(0, none.Count, "Unknown enemy type should return an empty list.");
    }

    [TestMethod]
    public void Weights_Definition_IsConsistent()
    {
        // Ensure that all weighted types are valid enum values (not Unknown) and weights are positive.
        foreach (var kv in EnemyTypeHelper.Weights)
        {
            Assert.AreNotEqual(EnemyType.Unknown, kv.Key, "Weights should not include Unknown.");
            Assert.IsTrue(kv.Value > 0, $"Weight for {kv.Key} should be positive.");
        }
    }
}
