namespace Roguelike.Console.Game.Characters.NPCs.Dialogues;

using Microsoft.VisualBasic.FileIO;
using Roguelike.Console.Configuration;
using Roguelike.Console.Game.Collectables.Items;
using Roguelike.Console.Game.Levels;
using System;

public static class NpcDialogManager
{
    public static void StartDialogue(Npc npc, LevelManager level, GameSettings settings)
    {
        // Build (or refresh) the tree for this NPC
        if (npc.Id == NpcId.Armin)
            NpcDialogues.BuildForArmin(npc, level, settings);
        else if (npc.Id == NpcId.Ichem)
            NpcDialogues.BuildForIchem(npc, level, settings);

        var node = npc.Root;
        if (node == null) return;

        bool end = false;
        while (!end && node != null)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{npc.Name}:");
            Console.ResetColor();
            Console.WriteLine(node.Text?.Invoke() ?? "");
            Console.WriteLine();

            if (node.Options.Count == 0)
            {
                Console.WriteLine("Press any key to return...");
                Console.ReadKey(true);
                break;
            }

            // Render options with Choice 1/2/3 mapping
            var map = new[]
            {
                settings.Controls.Choice1,
                settings.Controls.Choice2,
                settings.Controls.Choice3
            };
            for (int i = 0; i < node.Options.Count && i < map.Length; i++) // Be careful with the length, only 3 choices are supported
            {
                var label = node.Options[i].LabelFactory?.Invoke() ?? node.Options[i].Label;
                Console.WriteLine($"{map[i]}. {label}");
            }

            // Read input
            int choice = -1;
            while (choice == -1)
            {
                var key = Console.ReadKey(true).Key.ToString().ToUpperInvariant();
                for (int i = 0; i < node.Options.Count && i < map.Length; i++)
                {
                    if (key == map[i].ToUpperInvariant()) { choice = i; break; }
                }
            }

            var opt = node.Options[choice];

            // Execute action if any
            string? feedback = opt.Action?.Invoke();
            if (!string.IsNullOrWhiteSpace(feedback))
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(feedback);
                Console.ResetColor();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }

            // Display player HUD
            RenderPlayerHud(level);

            // Next node (null = end)
            node = opt.Next;
            if (node == null) end = true;
        }

        // Update NPC memory
        npc.HasMet = true;
        npc.TimesTalked++;
    }

    private static void RenderPlayerHud(LevelManager level)
    {
        var p = level.Player;

        Console.WriteLine();
        Console.WriteLine(new string('-', 50));
        Console.WriteLine($"Gold: {p.Gold} | XP: {p.XP}/{p.GetNextLevelXP()} | Lv: {p.Level}");
        Console.WriteLine($"HP: {p.LifePoint}/{p.MaxLifePoint} | STR: {p.Strength} | ARM: {p.Armor} | SPD: {p.Speed} | VIS: {p.Vision}");

        if (p.Inventory.Any())
        {
            Console.WriteLine("Inventory:");
            foreach (var it in p.Inventory)
            {
                ItemManager.WriteColored($"- {it.Name} ({it.EffectDescription})", it.Rarity);
                Console.WriteLine();
            }
        }
    }
}

