using Roguelike.Core.Game.Characters.NPCs.Dialogues;

namespace Roguelike.Core.Game.Characters.NPCs;

public class Npc : Character
{
    public NpcId Id { get; set; }
    public char Character { get; set; } // NPC asset
    public override string Name { get; set; }

    #region Dialogue
    public bool HasMet { get; set; }
    public int TimesTalked { get; set; }
    public DialogueNode? Root { get; set; } // Entry point (will be chosen at runtime)

    /// <summary>
    /// Reduces the life points of the entity based on the specified attack strength and its armor value.
    /// </summary>
    /// <remarks>The damage dealt is calculated as the attack strength minus the entity's armor value, with a
    /// minimum damage of 2.  The entity's life points are reduced by the calculated damage, but will not drop below
    /// 0.</remarks>
    /// <param name="strength">The strength of the attack. Must be a non-negative value.</param>
    public void TakeDamage(int strength)
    {
        var damage = Math.Max(2, strength - Armor);
        LifePoint = Math.Max(LifePoint - damage, 0);
    }
    #endregion
}
