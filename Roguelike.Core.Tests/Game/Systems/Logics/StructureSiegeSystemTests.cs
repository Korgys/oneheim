using Roguelike.Core.Configuration;
using Roguelike.Core.Game.Characters.Enemies;
using Roguelike.Core.Game.Characters.Enemies.Bosses;
using Roguelike.Core.Game.Levels;
using Roguelike.Core.Game.Structures;
using Roguelike.Core.Game.Systems;
using Roguelike.Core.Game.Systems.Logics;
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Tests.Game.Systems.Logics;

[TestClass]
public class StructureSiegeSystemTests
{
    private static (StructureSiegeSystem sys, TurnContext ctx, LevelManager level) CreateContext(
        Difficulty difficulty = Difficulty.Normal,
        int playerSteps = 0,
        int playerX = 0,
        int playerY = 0)
    {
        var settings = new GameSettings { Difficulty = difficulty };
        var level = new LevelManager(settings);

        level.Player.Steps = playerSteps;
        level.Player.X = playerX;
        level.Player.Y = playerY;

        var diff = new DifficultyManager(settings.Difficulty);
        var ctx = new TurnContext(level, settings, diff);

        var sys = new StructureSiegeSystem();
        return (sys, ctx, level);
    }

    private static dynamic? GetBaseCamp(LevelManager level)
    {
        // We only need members used by the system: Name, Hp, MaxHp, TakeDamage(), WallTiles(), IsInterior(), IsSeverelyEndomaged().
        return level.Structures.FirstOrDefault(s => s.Name == Messages.BaseCamp);
    }

    /// <summary>
    /// Finds up to 'count' outside tiles adjacent (Manhattan distance 1) to the structure walls.
    /// Rejects any tile considered 'interior' by the structure.
    /// </summary>
    private static (int x, int y)[] FindExteriorAdjacentToWalls(dynamic structure, int count = 4)
    {
        var walls = ((IEnumerable<(int x, int y)>)structure.WallTiles()).ToArray();
        var spots = new List<(int x, int y)>();

        foreach (var w in walls)
        {
            var cands = new (int x, int y)[]
            {
                (w.x + 1, w.y),
                (w.x - 1, w.y),
                (w.x, w.y + 1),
                (w.x, w.y - 1),
            };

            foreach (var p in cands)
            {
                // ensure not interior (the siege requires enemies "outside", adjacent to walls)
                if (!structure.IsInterior(p.x, p.y))
                {
                    if (!spots.Contains(p)) spots.Add(p);
                    if (spots.Count >= count) return spots.ToArray();
                }
            }
        }

        return spots.ToArray();
    }

    private static Enemy MakeEnemyAt((int x, int y) pos, int strength = 4)
    {
        var e = new Enemy
        {
            X = pos.x,
            Y = pos.y,
            Strength = strength,
            LifePoint = 10
        };
        return e;
    }

    private static Boss MakeBossAt((int x, int y) pos, int strength = 10)
    {
        var b = new Boss(pos.x, pos.y, 1)
        {
            X = pos.x,
            Y = pos.y,
            Strength = strength,
            LifePoint = 50
        };
        return b;
    }

    [TestMethod]
    public void Update_NoBaseCamp_NoMessage_NoAttackers()
    {
        var (sys, ctx, level) = CreateContext(playerSteps: 200); // steps > 175 but no base camp

        // Remove any structure named BaseCamp if present
        var baseCamp = GetBaseCamp(level);
        if (baseCamp != null)
            level.Structures.Remove((Structure)baseCamp);

        sys.Update(ctx);

        Assert.IsNull(sys.LastMessage, "No base camp present => no siege, no message.");
        Assert.AreEqual(0, sys.LastAttackers.Count, "No base camp => no attackers tracked.");
    }

    [TestMethod]
    public void Update_StepsAtOrBelow175_DoesNothing()
    {
        var (sys, ctx, level) = CreateContext(playerSteps: 175);
        Assert.IsNotNull(GetBaseCamp(level), "Precondition: Base Camp should exist for this test.");

        sys.Update(ctx);

        Assert.IsNull(sys.LastMessage, "No siege before or at 175 steps.");
        Assert.AreEqual(0, sys.LastAttackers.Count);
    }

    [TestMethod]
    public void Update_At176_WithFourAdjacentEnemies_TriggersSiege_DamagesCamp()
    {
        var (sys, ctx, level) = CreateContext(playerSteps: 176);
        var baseCamp = GetBaseCamp(level);
        Assert.IsNotNull(baseCamp, "Precondition: Base Camp should exist.");

        int hpBefore = (int)baseCamp.Hp;

        // Place 4 enemies adjacent to walls (outside)
        var spots = FindExteriorAdjacentToWalls(baseCamp, 4);
        Assert.IsTrue(spots.Length >= 4, "Could not find enough exterior adjacent wall tiles for test.");

        foreach (var pos in spots)
            level.Enemies.Add(MakeEnemyAt(pos, strength: 4));

        sys.Update(ctx);

        Assert.IsTrue(sys.LastAttackers.Count >= 4, "Should have detected 4 attackers.");
        Assert.IsNotNull(sys.LastMessage, "Siege should produce a message.");
        Assert.IsTrue(baseCamp.Hp < hpBefore, "Structure HP should be reduced by the siege.");
    }

    [TestMethod]
    public void Update_At176_WithSingleBoss_TriggersSiegeEvenIfLessThanFour()
    {
        var (sys, ctx, level) = CreateContext(playerSteps: 176);
        var baseCamp = GetBaseCamp(level);
        Assert.IsNotNull(baseCamp, "Precondition: Base Camp should exist.");

        int hpBefore = (int)baseCamp.Hp;

        // Place one boss adjacent to a wall
        var spot = FindExteriorAdjacentToWalls(baseCamp, 1)[0];

        level.Enemies.Add(MakeBossAt(spot, strength: 10));

        sys.Update(ctx);

        Assert.IsTrue(sys.LastAttackers.Count >= 1, "Should have detected at least the boss as attacker.");
        Assert.IsNotNull(sys.LastMessage, "Siege should produce a message with a boss.");
        Assert.IsTrue(baseCamp.Hp < hpBefore, "Structure HP should be reduced by boss siege.");
    }

    [TestMethod]
    public void Update_Overkill_DestroyBaseCamp_YieldsDestroyMessage_AndClearsAttackers()
    {
        var (sys, ctx, level) = CreateContext(playerSteps: 176);
        var baseCamp = GetBaseCamp(level);
        Assert.IsNotNull(baseCamp, "Precondition: Base Camp should exist.");

        baseCamp.Hp = 2;

        // Place 4 enemies adjacent to walls (outside)
        var spots = FindExteriorAdjacentToWalls(baseCamp, 4);
        Assert.IsTrue(spots.Length >= 4, "Could not find enough exterior adjacent wall tiles for test.");

        foreach (var pos in spots)
            level.Enemies.Add(MakeEnemyAt(pos, strength: 4));

        sys.Update(ctx);

        Assert.IsNotNull(sys.LastMessage, "Should announce the destruction of the structure.");
        StringAssert.Contains(sys.LastMessage!, baseCamp.Name);
        Assert.AreEqual(0, sys.LastAttackers.Count, "Attackers should be cleared after destruction.");
        Assert.IsFalse(level.Structures.Any(s => s.Name == Messages.BaseCamp),
            "Base Camp should be removed from the level upon destruction.");
    }
}
