using Roguelike.Core.Game.Characters.NPCs;

namespace Roguelike.Core.Tests.Game.Characters.NPCs;

[TestClass]
public class NpcTests
{
    [TestMethod]
    public void TakeDamage_ShouldTakeDamage()
    {
        var npc = new Npc
        {
            LifePoint = 100,
            Armor = 5
        };
        npc.TakeDamage(10);

        Assert.AreEqual(95, npc.LifePoint); // 10 - 5 = 5 damage
    }
}
