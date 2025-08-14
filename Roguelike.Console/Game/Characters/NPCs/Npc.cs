using Roguelike.Console.Game.Characters.NPCs.Dialogues;

namespace Roguelike.Console.Game.Characters.NPCs;

public class Npc : Character
{
    public NpcId Id { get; set; }
    public char Character { get; set; } // NPC asset
    public string Name { get; set; }

    #region Dialogue
    public bool HasMet { get; set; }
    public int TimesTalked { get; set; }
    public DialogueNode? Root { get; set; } // Entry point (will be chosen at runtime)
    #endregion
}
