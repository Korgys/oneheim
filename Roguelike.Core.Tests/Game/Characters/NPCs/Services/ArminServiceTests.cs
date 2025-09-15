using Roguelike.Core.Configuration;
using Roguelike.Core.Game.Characters.NPCs.Services;
using Roguelike.Core.Game.Characters.Players;
using Roguelike.Core.Game.Levels;
using Roguelike.Core.Game.Structures;

namespace Roguelike.Core.Tests.Game.Characters.NPCs.Services;

[TestClass]
public class ArminServiceTests
{
    [TestInitialize]
    public void ResetArminFlags()
    {
        // Ensure the static interaction flag doesn't leak between tests
        ArminInteractions.HasExplainedWhereWeAre = false;
    }


    [TestMethod]
    public void CanHealNow_False_WhenFullHpOrZeroGold()
    {
        var lvl = NewLevel(new Player { LifePoint = 100, MaxLifePoint = 100, Gold = 50, Steps = 100 });
        var svc = new ArminService(lvl);
        Assert.IsFalse(svc.CanHealNow, "Full HP should not allow healing.");

        lvl.Player.LifePoint = 50;
        lvl.Player.Gold = 0;
        Assert.AreEqual(0, svc.HealAmount);
        Assert.IsFalse(svc.CanHealNow, "No gold → cannot heal.");
    }

    [TestMethod]
    public void HealAction_HealsAndCosts_GoldScaledBySteps()
    {
        // missing = 50, gold = 30 → healAmount = 30
        // steps = 100 → cost = 30
        var lvl = NewLevel(new Player { LifePoint = 50, MaxLifePoint = 100, Gold = 30, Steps = 100 });
        var svc = new ArminService(lvl);

        Assert.IsTrue(svc.CanHealNow);
        Assert.AreEqual(30, svc.HealAmount);
        Assert.AreEqual(30, svc.HealCost);

        var msg = svc.HealAction(); // don’t assert full i18n, just effects
        Assert.AreEqual(80, lvl.Player.LifePoint); // +30
        Assert.AreEqual(0, lvl.Player.Gold);       // -30
        StringAssert.Contains(msg, "30");
    }

    [TestMethod]
    public void HealAction_AlreadyFull_ReturnsEarlyMessage()
    {
        var lvl = NewLevel(new Player { LifePoint = 100, MaxLifePoint = 100, Gold = 999, Steps = 500 });
        var svc = new ArminService(lvl);
        var msg = svc.HealAction();
        Assert.AreEqual(100, lvl.Player.LifePoint);
        Assert.AreEqual(999, lvl.Player.Gold);
        Assert.IsFalse(string.IsNullOrWhiteSpace(msg));
    }

    [TestMethod]
    public void HealPitchText_ShowsCostAndAmount_WhenNotFull()
    {
        var lvl = NewLevel(new Player { LifePoint = 50, MaxLifePoint = 100, Gold = 20, Steps = 150 });
        var svc = new ArminService(lvl);
        // amount = min(20, 50) = 20; cost = 20*150/100 = 30
        var txt = svc.HealPitchText();
        StringAssert.Contains(txt, "20");
    }

    [TestMethod]
    public void CanRepairNow_False_WhenNoCampOrFullOrNoGold()
    {
        var lvl = NewLevel(); // no camp
        var svc = new ArminService(lvl);
        Assert.IsFalse(svc.CanRepairNow);

        lvl = NewLevel(p: new Player { Gold = 5 }, camp: NewCamp(10, 10)); // full camp
        svc = new ArminService(lvl);
        Assert.IsFalse(svc.CanRepairNow);

        lvl = NewLevel(p: new Player { Gold = 0 }, camp: NewCamp(5, 10));
        svc = new ArminService(lvl);
        Assert.IsFalse(svc.CanRepairNow);
    }

    [TestMethod]
    public void RepairPitchText_NoCampOrFull_ReturnsInformativeText()
    {
        var lvl = NewLevel();
        var svc = new ArminService(lvl);
        var txt = svc.RepairPitchText();
        Assert.IsFalse(string.IsNullOrWhiteSpace(txt));

        lvl = NewLevel(p: new Player { Gold = 10 }, camp: NewCamp(10, 10));
        svc = new ArminService(lvl);
        txt = svc.RepairPitchText();
        Assert.IsFalse(string.IsNullOrWhiteSpace(txt));
    }

    [TestMethod]
    public void PickOther_TogglesByInteractionFlag()
    {
        var lvl = NewLevel();
        var svc = new ArminService(lvl);

        ArminInteractions.HasExplainedWhereWeAre = false;
        var first = svc.PickOther();
        Assert.IsFalse(string.IsNullOrWhiteSpace(first));

        ArminInteractions.HasExplainedWhereWeAre = true;
        var second = svc.PickOther();
        Assert.IsFalse(first.Equals(second, StringComparison.Ordinal), "Should differ based on the flag.");
    }

    [TestMethod]
    public void BuildServiceNodes_WiresStandardOptions_AndConditionalActions()
    {
        var lvl = NewLevel(
            p: new Player { LifePoint = 75, MaxLifePoint = 100, Gold = 10, Steps = 100 }, // not full → heal action present
            camp: NewCamp(hp: 5, max: 10)); // not full → repair action present
        var svc = new ArminService(lvl);

        var (hub, heal, repair, other) = svc.BuildServiceNodes(svc.PickOther());

        // Hub standard options: 4 entries (heal / repair / other / goodbye)
        Assert.AreEqual(4, hub.Options.Count);

        // Heal node: Pay&Heal present when not full; MaybeLater always present
        Assert.IsTrue(heal.Options.Count >= 1);
        Assert.IsTrue(heal.Options.Any(o => o.Action != null),
            "Heal node should include an actionable option when player not full.");

        // Other node returns some text and has an Ok option
        Assert.IsFalse(string.IsNullOrWhiteSpace(other.Text()));
        Assert.IsTrue(other.Options.Any(o => o.Label != null));
    }

    [TestMethod]
    public void BuildServiceNodes_OtherWhereAreWe_SetsExplainedFlagAndChangesTextNextTime()
    {
        var lvl = NewLevel();
        var svc = new ArminService(lvl);
        ArminInteractions.HasExplainedWhereWeAre = false;

        var nodes1 = svc.BuildServiceNodes(otherText: Properties.i18n.Messages.WhereAreWe);
        var first = nodes1.Other.Text();
        Assert.IsTrue(ArminInteractions.HasExplainedWhereWeAre);
        Assert.IsFalse(string.IsNullOrWhiteSpace(first));

        // Calling again with same input should now answer differently
        var nodes2 = svc.BuildServiceNodes(otherText: Properties.i18n.Messages.WhereAreWe);
        var second = nodes2.Other.Text();
        Assert.IsFalse(string.IsNullOrWhiteSpace(second));
        Assert.AreNotEqual(first, second);
    }

    private static LevelManager NewLevel(
        Player? p = null,
        Structure? camp = null)
    {
        var lvl = new LevelManager(new GameSettings())
        {
            Player = p ?? new Player
            {
                LifePoint = 50,
                MaxLifePoint = 100,
                Gold = 0,
                Steps = 0
            }
        };
        if (camp != null)
            lvl.Structures.Add(camp);
        return lvl;
    }

    private static Structure NewCamp(int hp, int max)
    {
        var structure = new Structure("", 0, 0, [ "X" ], new HashSet<(int x, int y)>(), max);
        structure.Hp = hp;
        return structure;
    }
}
