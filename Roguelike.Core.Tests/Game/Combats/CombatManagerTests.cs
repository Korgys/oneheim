using Roguelike.Core.Configuration;
using Roguelike.Core.Game.Characters.Enemies;
using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Game.Combat;
using Roguelike.Core.Game.Levels;
using Roguelike.Core.Tests.Fakes;

namespace Roguelike.Core.Tests.Game.Combats;

[TestClass]
public class CombatManagerTests
{
    [TestMethod]
    public void StartCombat_TalismanSavesPlayer_CancelsCombatAndTeleportsToCamp()
    {
        var level = new LevelManager(new GameSettings());
        var player = level.Player;
        player.LifePoint = 1;
        player.MaxLifePoint = 20;
        player.Speed = 1;
        player.Inventory.Add(ItemFactory.CreateItem(ItemId.TalismanOfTheLastBreath));
        player.X = 50;
        player.Y = 18;

        var enemy = EnemyFactory.Create(EnemyId.Drunkard, player.X, player.Y, 1);
        enemy.Strength = 100;
        enemy.Speed = 100;
        level.Enemies.Add(enemy);

        var report = new CombatManager(level, new FakeCombatRenderer()).StartCombat(enemy);

        Assert.IsFalse(report.PlayerDied);
        Assert.IsFalse(report.EnemyDied);
        Assert.IsTrue(report.PlayerTeleportedToCamp);
        Assert.AreEqual(0, report.Gold);
        Assert.AreEqual(0, report.Xp);
        Assert.IsTrue(level.Structures.First(s => s.Name == Roguelike.Core.Properties.i18n.Messages.BaseCamp)
            .IsInterior(player.X, player.Y));
        Assert.IsTrue(level.Enemies.Contains(enemy), "The combat should stop without killing the enemy.");
    }
}
