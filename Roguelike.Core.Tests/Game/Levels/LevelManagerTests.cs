using Roguelike.Core.Configuration;
using Roguelike.Core.Game.Characters.Enemies;
using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Game.Levels;

namespace Roguelike.Core.Tests.Game.Levels;

[TestClass]
public class LevelManagerTests
{
    [TestMethod]
    public void PlaceEnemies_TalismanOfPeace_ReducesEnemyCount()
    {
        var originalEnemyTypes = LevelManager.EnemyTypes.ToList();
        try
        {
            LevelManager.EnemyTypes.Clear();
            LevelManager.EnemyTypes.Add(EnemyType.Wild);

            var level = CreateEmptyLevel();
            level.Player.Inventory.Add(ItemFactory.CreateItem(ItemId.TalismanOfPeace));

            level.PlaceEnemies(5);

            Assert.AreEqual(1, level.Enemies.Count);
        }
        finally
        {
            LevelManager.EnemyTypes.Clear();
            LevelManager.EnemyTypes.AddRange(originalEnemyTypes);
        }
    }

    [TestMethod]
    public void PlaceEnemies_TalismanOfPeace_DoesNotSpawnNegativeCount()
    {
        var originalEnemyTypes = LevelManager.EnemyTypes.ToList();
        try
        {
            LevelManager.EnemyTypes.Clear();
            LevelManager.EnemyTypes.Add(EnemyType.Wild);

            var level = CreateEmptyLevel();
            level.Player.Inventory.Add(ItemFactory.CreateItem(ItemId.TalismanOfPeace));

            level.PlaceEnemies(2);

            Assert.AreEqual(0, level.Enemies.Count);
        }
        finally
        {
            LevelManager.EnemyTypes.Clear();
            LevelManager.EnemyTypes.AddRange(originalEnemyTypes);
        }
    }

    private static LevelManager CreateEmptyLevel()
    {
        var level = new LevelManager(new GameSettings());
        level.Structures.Clear();
        level.Treasures.Clear();
        level.Npcs.Clear();
        level.Enemies.Clear();
        level.Mercenaries.Clear();
        return level;
    }
}
