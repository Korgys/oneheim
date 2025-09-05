namespace Roguelike.Core.Game.Abstractions;

using Roguelike.Core.Game.Characters.NPCs;
using Roguelike.Core.Game.Characters.NPCs.Dialogues;
using Roguelike.Core.Game.Characters.Players;

public interface IDialogueRenderer
{
    // Called when starting dialogue with an NPC
    void ShowDialogue(Npc npc, Player player, DialogueNode root);
}