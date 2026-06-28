using Roguelike.Core.Configuration;
using Roguelike.Core.Game.Characters.NPCs;
using Roguelike.Core.Game.Characters.NPCs.Dialogues.Texts;
using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Game.Levels;
using Roguelike.Core.Game.Systems;
using Roguelike.Core.Game.Systems.Logics;

namespace Roguelike.Core.Tests.Game.Systems.Logics;

[TestClass]
public class NpcSpawnSystemTests
{
    [TestMethod]
    public void Update_SpawnsOracleAndEconomyNpcs_AtMilestones()
    {
        var settings = new GameSettings();
        var level = new LevelManager(settings);
        var ctx = new TurnContext(level, settings, new DifficultyManager(settings.Difficulty));
        var system = new NpcSpawnSystem();

        level.Player.Steps = 350;
        system.Update(ctx);
        Assert.IsTrue(level.Npcs.Any(n => n.Id == NpcId.Omana));

        level.Player.Steps = 450;
        system.Update(ctx);
        Assert.IsTrue(level.Npcs.Any(n => n.Id == NpcId.Urd));

        level.Player.Steps = 550;
        system.Update(ctx);
        Assert.IsTrue(level.Npcs.Any(n => n.Id == NpcId.Ylva));
    }

    [TestMethod]
    public void BuildDialogues_NewNpcs_CreateUsableRoots()
    {
        var level = new LevelManager(new GameSettings());
        level.Player.Gold = 500;
        level.Player.Inventory.Add(ItemFactory.CreateItem(ItemId.DaggerLifeSteal));

        var omana = NpcFactory.CreateNpc(NpcId.Omana, 1, 1);
        NpcDialogues.BuildForOmana(omana, level);
        Assert.IsNotNull(omana.Root);
        Assert.IsFalse(string.IsNullOrWhiteSpace(omana.Root.Text!()));

        var urd = NpcFactory.CreateNpc(NpcId.Urd, 1, 1);
        NpcDialogues.BuildForUrd(urd, level);
        Assert.IsNotNull(urd.Root);
        Assert.IsTrue(urd.Root.Options.Any(o => o.Action != null));

        var ylva = NpcFactory.CreateNpc(NpcId.Ylva, 1, 1);
        NpcDialogues.BuildForYlva(ylva, level);
        Assert.IsNotNull(ylva.Root);
        Assert.IsTrue(ylva.Root.Options.Any(o => o.Action != null));
    }
}
