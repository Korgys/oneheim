using Roguelike.Core.Game.Characters.Enemies;
using Roguelike.Core.Game.Levels;

namespace Roguelike.Core.Tests.Game.Levels;

[TestClass]
public class EnemyBagsTests
{
    [DataTestMethod]
    [DataRow(0)]
    [DataRow(16)]
    [DataRow(int.MinValue)]
    [DataRow(int.MaxValue)]
    public void GetByLevelAndType_InvalidLevel_Throws(int level)
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            EnemyBags.GetByLevelAndType(level, new List<EnemyType>()));
    }

    [TestMethod]
    public void GetByLevelAndType_EmptyTypes_ReturnsEmpty()
    {
        var result = EnemyBags.GetByLevelAndType(1, new List<EnemyType>());
        Assert.AreEqual(0, result.Count);
    }

    [DataTestMethod]
    [DynamicData(nameof(AllLevelIndices), DynamicDataSourceType.Method)]
    public void GetByLevelAndType_AllTypes_ReturnsExactLevelBag(int level)
    {
        var allTypes = Enum.GetValues(typeof(EnemyType)).Cast<EnemyType>().ToList();

        var expected = EnemyBags.Levels[level - 1];
        var actual = EnemyBags.GetByLevelAndType(level, allTypes);

        AssertDictionariesEqual(expected, actual, $"Level {level} bag mismatch.");
    }

    // Uses a few individual types (and a combo) to ensure filtering works and counts match.
    [DataTestMethod]
    [DynamicData(nameof(SomeTypeSelections), DynamicDataSourceType.Method)]
    public void GetByLevelAndType_FiltersByType_IntersectionOnly(int level, List<EnemyType> selectedTypes)
    {
        // Build expected via the same helper (keeps tests resilient to future mapping changes).
        var allowedIds = selectedTypes
            .SelectMany(EnemyTypeHelper.GetEnemyIdsByType)
            .ToHashSet();

        var levelBag = EnemyBags.Levels[level - 1];

        var expected = levelBag
            .Where(kv => allowedIds.Contains(kv.Key))
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        var actual = EnemyBags.GetByLevelAndType(level, selectedTypes);

        AssertDictionariesEqual(expected, actual, $"Level {level} filtering mismatch.");
    }

    private static IEnumerable<object[]> AllLevelIndices()
    {
        // levels are 1..15
        for (int i = 1; i <= 15; i++)
            yield return new object[] { i };
    }

    private static IEnumerable<object[]> SomeTypeSelections()
    {
        // Pick up to 3 concrete EnemyType values if the enum has that many.
        var all = Enum.GetValues(typeof(EnemyType)).Cast<EnemyType>().ToList();

        if (all.Count == 0)
        {
            // If the enum is empty (unlikely), still run a no-op case to keep coverage stable.
            yield return new object[] { 1, new List<EnemyType>() };
            yield break;
        }

        // Single type (first)
        yield return new object[] { 1, new List<EnemyType> { all[0] } };

        // If available: second single type at a different level
        if (all.Count >= 2)
            yield return new object[] { 8, new List<EnemyType> { all[1] } };

        // If available: third single type at another level
        if (all.Count >= 3)
            yield return new object[] { 12, new List<EnemyType> { all[2] } };

        // A combo of the first few types to test union behavior
        var take = Math.Min(3, all.Count);
        yield return new object[] { 9, all.Take(take).ToList() };
    }

    private static void AssertDictionariesEqual(
        IReadOnlyDictionary<EnemyId, int> expected,
        IReadOnlyDictionary<EnemyId, int> actual,
        string? message = null)
    {
        Assert.AreEqual(expected.Count, actual.Count, $"Count differs. {message}");

        foreach (var (id, count) in expected)
        {
            Assert.IsTrue(actual.ContainsKey(id), $"Missing id: {id}. {message}");
            Assert.AreEqual(count, actual[id], $"Count mismatch for {id}. {message}");
        }

        // Ensure no extra keys slipped in
        foreach (var id in actual.Keys)
        {
            Assert.IsTrue(expected.ContainsKey(id), $"Unexpected id: {id}. {message}");
        }
    }
}
