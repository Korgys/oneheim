using Roguelike.Core.Configuration;
using Roguelike.Core.Game.Characters.Enemies.Bosses;
using Roguelike.Core.Game.Levels;
using Roguelike.Core.Game.Systems;
using Roguelike.Core.Game.Systems.Logics;
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Tests.Game.Systems.Logics;

[TestClass]
public class WaveAndFogSystemTests
{
    private static (WaveAndFogSystem sys, TurnContext ctx, LevelManager level) CreateContext(
        Difficulty difficulty = Difficulty.Normal,
        int playerHp = 10,
        int playerSteps = 0)
    {
        // Game settings (adjust as needed)
        var settings = new GameSettings
        {
            Difficulty = difficulty
        };

        // Real level + difficulty manager (as used by GameEngine)
        var level = new LevelManager(settings);
        level.Player.LifePoint = playerHp;
        level.Player.Steps = playerSteps;

        var diff = new DifficultyManager(settings.Difficulty);

        var ctx = new TurnContext(level, settings, diff);
        var sys = new WaveAndFogSystem();

        return (sys, ctx, level);
    }

    [TestMethod]
    public void Update_DoesNothing_WhenPlayerDead()
    {
        var (sys, ctx, level) = CreateContext(playerHp: 0, playerSteps: 8);

        sys.Update(ctx);

        Assert.IsNull(sys.LastMessage, "No message should be set when player is already dead.");
    }

    [TestMethod]
    public void Update_SpawnsWaveAndWarnsPlayer_AtStep8()
    {
        var (sys, ctx, level) = CreateContext(playerSteps: 8);

        sys.Update(ctx);

        Assert.AreEqual(Messages.BeCarefullYouAreNotSafeHere, sys.LastMessage);
    }

    [TestMethod]
    public void Update_FogIntensifies_AtStep100()
    {
        var (sys, ctx, level) = CreateContext(playerSteps: 100);

        sys.Update(ctx);

        Assert.AreEqual(Messages.TheFogIntensifies, sys.LastMessage);
    }

    [TestMethod]
    public void Update_PlacesBossAndFog_AtStep515()
    {
        var (sys, ctx, level) = CreateContext(playerSteps: 515);

        sys.Update(ctx);

        Assert.AreEqual(Messages.ABossArrives, sys.LastMessage);
    }

    [TestMethod]
    public void Update_PlacesBossAndFog_AtStep1015()
    {
        var (sys, ctx, level) = CreateContext(playerSteps: 1015);

        sys.Update(ctx);

        Assert.AreEqual(Messages.ABossArrives, sys.LastMessage);
    }

    [TestMethod]
    public void Update_PlacesBossAndFog_AtStep1515()
    {
        var (sys, ctx, level) = CreateContext(playerSteps: 1515);

        sys.Update(ctx);

        Assert.AreEqual(Messages.ABossArrives, sys.LastMessage);
    }

    [TestMethod]
    public void Update_NoBossRemaining_AnnouncesEndgame_AfterStep1516()
    {
        var (sys, ctx, level) = CreateContext(playerSteps: 1516);

        // Ensure there is no boss alive on the map.
        // By default LevelManager starts with no enemies; this assert protects intent.
        Assert.IsFalse(level.Enemies.Exists(e => e is Boss),
            "Precondition failed: level should have no Boss before endgame check.");

        TestHelper.SetPrivateField(sys, "_firstBossPlaced", true);
        TestHelper.SetPrivateField(sys, "_secondBossPlaced", true);
        TestHelper.SetPrivateField(sys, "_thirdBossPlaced", true);
        level.Enemies.Clear(); // Ensure no boss remains
        sys.Update(ctx);

        Assert.AreEqual(Messages.YouDefeatedAllBossesThanksForPlaying, sys.LastMessage);
    }

    [TestMethod]
    public void Update_BossRemaining_NoEndgame_AfterStep1516()
    {
        var (sys, ctx, level) = CreateContext(playerSteps: 1516);

        // Simulate a remaining boss on the map
        level.PlaceBoss();
        TestHelper.SetPrivateField(sys, "_firstBossPlaced", true);
        TestHelper.SetPrivateField(sys, "_secondBossPlaced", true);
        TestHelper.SetPrivateField(sys, "_thirdBossPlaced", true);

        sys.Update(ctx);

        Assert.IsNull(sys.LastMessage, "When a boss remains after 1516 steps, endgame message must not trigger.");
    }
}
