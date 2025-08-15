namespace Roguelike.Console.Game.Characters.NPCs.Dialogues;

using Roguelike.Console.Configuration;
using Roguelike.Console.Game.Levels;
using System;

public static class NpcDialogManager
{
    public static void StartDialogue(Npc npc, LevelManager level, GameSettings settings)
    {
        // Build (or refresh) the tree for this NPC
        if (npc.Name.Equals(NpcId.Armin.ToString(), StringComparison.OrdinalIgnoreCase))
            NpcDialogues.BuildForArmin(npc, level, settings);

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

            // Render options with Choice1/2/3 mapping
            var map = new[]
            {
                settings.Controls.Choice1,
                settings.Controls.Choice2,
                settings.Controls.Choice3
            };

            for (int i = 0; i < node.Options.Count && i < map.Length; i++)
                Console.WriteLine($"{map[i]}: {node.Options[i].Label}");
            if (node.Options.Count > map.Length)
                Console.WriteLine("(More options exist but only 3 can be mapped here)");

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

            // Next node (null = end)
            node = opt.Next;
            if (node == null) end = true;
        }

        // Update NPC memory
        npc.HasMet = true;
        npc.TimesTalked++;
    }
}

