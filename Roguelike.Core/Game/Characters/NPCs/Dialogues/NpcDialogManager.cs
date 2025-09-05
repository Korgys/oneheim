using Roguelike.Core.Configuration;
using Roguelike.Core.Game.Abstractions;
using Roguelike.Core.Game.Characters.NPCs;
using Roguelike.Core.Game.Characters.NPCs.Dialogues;
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
        // Build tree depending on NPC
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
        }

        if (npc.Root == null) return;

        // Pass tree to renderer (Console, UI, etc.)
        renderer.ShowDialogue(npc, level.Player, npc.Root);

        // Update memory
        npc.HasMet = true;
        npc.TimesTalked++;
    }
}
