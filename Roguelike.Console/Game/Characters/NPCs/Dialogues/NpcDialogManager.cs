namespace Roguelike.Console.Game.Characters.NPCs.Dialogues;

using Roguelike.Console.Configuration;
using Roguelike.Console.Game.Levels;
using Roguelike.Console.Game.Rendering;
using Roguelike.Console.Rendering;
using System;

public static class NpcDialogManager
{
    public static void StartDialogue(Npc npc, LevelManager level, GameSettings settings)
    {
        // Build (or refresh) the tree for this NPC
        switch (npc.Id)
        {
            case NpcId.Armin: 
                NpcDialogues.BuildForArmin(npc, level);
                break;
            case NpcId.Ichem:
                NpcDialogues.BuildForIchem(npc, level, settings);
                break;
            case NpcId.Eber:
                NpcDialogues.BuildForEber(npc, level, settings);
                break;
            default:
                break;
        }

        var node = npc.Root;
        if (node == null) return;

        bool end = false;
        while (!end && node != null)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{npc.Name}:");
            Console.ResetColor();

            // Typewriter for the narrative line
            DialogueUI.RenderTypewriter(node.Text?.Invoke() ?? "", new TypewriterOptions
            {
                BaseWordDelayMs = 35,
                CommaExtraDelayMs = 120,
                PeriodExtraDelayMs = 240,
                EnableColorMarkup = true
            });

            // Display player HUD
            PlayerRenderer.RenderPlayerInfoInDialogues(level.Player);

            if (node.Options.Count == 0)
            {
                Console.WriteLine("Press any key to return...");
                Console.ReadKey(true);
                break;
            }

            Console.WriteLine();

            // Render options with Choice 1/2/3/escape mapping
            var map = new[]
            {
                settings.Controls.Choice1,
                settings.Controls.Choice2,
                settings.Controls.Choice3,
                settings.Controls.Exit
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
            PlayerRenderer.RenderPlayerInfoInDialogues(level.Player);

            // Next node (null = end)
            node = opt.Next;
            if (node == null) end = true;
        }

        // Update NPC memory
        npc.HasMet = true;
        npc.TimesTalked++;
    }
}

