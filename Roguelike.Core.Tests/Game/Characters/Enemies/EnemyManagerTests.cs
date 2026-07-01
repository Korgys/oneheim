using Roguelike.Core.Configuration;
using Roguelike.Core.Game.Characters.Enemies;
using Roguelike.Core.Game.Levels;
using Roguelike.Core.Game.Systems.Logics;
using Roguelike.Core.Tests.Fakes;

namespace Roguelike.Core.Tests.Game.Characters.Enemies;

[TestClass]
public class EnemyManagerTests
{
    [TestMethod]
    [DataRow(LevelManager.GridWidth - 1, 10, 3)]
    [DataRow(10, LevelManager.GridHeight - 1, 1)]
    public void MoveEnemies_RandomMovement_DoesNotLeaveGrid(int startX, int startY, int direction)
    {
        var level = new LevelManager(new GameSettings());
        level.Structures.Clear();
        level.Treasures.Clear();
        level.Npcs.Clear();
        level.Enemies.Clear();
        level.Mercenaries.Clear();
        level.Player.X = 1;
        level.Player.Y = 1;

        var enemy = new Enemy
        {
            Name = "Edge enemy",
            X = startX,
            Y = startY,
            Vision = 0,
            StepsPerTurn = 1
        };
        level.Enemies.Add(enemy);

        var manager = new EnemyManager(level, new StructureSiegeSystem(), new FakeCombatRenderer());
        TestHelper.SetPrivateField(manager, "_rng", new FixedRandom(direction));

        manager.MoveEnemies();

        Assert.AreEqual(startX, enemy.X);
        Assert.AreEqual(startY, enemy.Y);
    }

    private sealed class FixedRandom(int value) : Random
    {
        public override int Next(int maxValue) => value;
    }
}
