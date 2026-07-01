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

    [TestMethod]
    public void InitialDungeonEnemies_StayLevelOne_BeforeSecondWave()
    {
        var level = new LevelManager(new GameSettings());

        Assert.IsTrue(level.Enemies.Count > 0);
        Assert.IsTrue(level.Enemies.All(e => e.Level == 1));
    }

    [TestMethod]
    public void PlaceEnemies_UsesLevelTwo_FromSecondWaveOnly()
    {
        var originalEnemyTypes = LevelManager.EnemyTypes.ToList();
        try
        {
            LevelManager.EnemyTypes.Clear();
            LevelManager.EnemyTypes.Add(EnemyType.Wild);

            var level = CreateEmptyLevel();
            level.Player.Steps = 100;
            level.PlaceEnemies(3);
            Assert.IsTrue(level.Enemies.All(e => e.Level == 1));

            level.Enemies.Clear();
            level.Player.Steps = 200;
            level.PlaceEnemies(3);
            Assert.IsTrue(level.Enemies.All(e => e.Level == 2));
        }
        finally
        {
            LevelManager.EnemyTypes.Clear();
            LevelManager.EnemyTypes.AddRange(originalEnemyTypes);
        }
    }

    [TestMethod]
    public void ShouldOfferCampTeleportTreasure_WhenFarAndLowLife()
    {
        var level = new LevelManager(new GameSettings());
        level.Treasures.Clear();
        level.Npcs.Clear();
        level.Enemies.Clear();
        level.Mercenaries.Clear();
        level.Player.X = LevelManager.GridWidth - 2;
        level.Player.Y = LevelManager.GridHeight - 2;
        level.Player.LifePoint = 20;
        level.Player.MaxLifePoint = 100;

        Assert.IsTrue(level.ShouldOfferCampTeleportTreasure());
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
