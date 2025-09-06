using Roguelike.Core.Game.Abstractions;
using Roguelike.Core.Game.Characters.NPCs;
using Roguelike.Core.Game.Characters.NPCs.Dialogues;
using Roguelike.Core.Game.Characters.Players;

namespace Roguelike.Core.Tests.Fakes;

public sealed class FakeDialogueRenderer : IDialogueRenderer
{
    public void ShowDialogue(Npc npc, Player player, DialogueNode root)
    {
    }
}
