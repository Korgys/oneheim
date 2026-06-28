using Roguelike.Core.Configuration;
using Roguelike.Core.Game.Abstractions;
using Roguelike.Core.Game.Characters.NPCs;
using Roguelike.Core.Game.Characters.NPCs.Dialogues.Texts;
using Roguelike.Core.Game.Levels;

public static class NpcDialogManager
{
    public static void StartDialogue(
        Npc npc,
        LevelManager level,
        GameSettings settings,
        IDialogueRenderer renderer,
        ITreasurePicker treasurePicker,
        IInventoryUI inventoryUI)
    {
        switch (npc.Id)
        {
            case NpcId.Armin:
                NpcDialogues.BuildForArmin(npc, level);
                break;
            case NpcId.Ichem:
                NpcDialogues.BuildForIchem(npc, level, settings, treasurePicker, inventoryUI);
                break;
            case NpcId.Eber:
                NpcDialogues.BuildForEber(npc, level, settings);
                break;
            case NpcId.Omana:
                NpcDialogues.BuildForOmana(npc, level);
                break;
            case NpcId.Urd:
                NpcDialogues.BuildForUrd(npc, level);
                break;
            case NpcId.Ylva:
                NpcDialogues.BuildForYlva(npc, level);
                break;
        }

        if (npc.Root == null) return;

        renderer.ShowDialogue(npc, level.Player, npc.Root);

        npc.HasMet = true;
        npc.TimesTalked++;
    }
}
