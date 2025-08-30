namespace Roguelike.Core.Game.Characters.NPCs.Dialogues;

using Roguelike.Core.Configuration;
using Roguelike.Core.Game.Levels;
using Roguelike.Core.Game.Characters.Players;
using System;
using System.Linq;
using System.Threading;

public static class NpcDialogManager
{
    public static void StartDialogue(Npc npc, LevelManager level, GameSettings settings)
    {
        // Rebuild the tree for this NPC (keeps context fresh)
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
                return;
        }

        var node = npc.Root;
        if (node == null) return;

        // Track the very first intro line so we don't overwrite it accidentally.
        // We'll use node builders that return a new DialogueNode for “back” screens.
        bool end = false;

        while (!end && node != null)
        {
            Console.Clear();

            // Speaker header
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{npc.Name}:");
            Console.ResetColor();

            // Typewriter narration (only text, not options)
            var line = node.Text?.Invoke() ?? string.Empty;
            TypeWrite(line, baseDelayMs: 35, commaExtra: 120, periodExtra: 240);

            // Player HUD (simple, non-UI-specific)
            RenderPlayerHud(level.Player);

            // No options → close on any key
            if (node.Options.Count == 0)
            {
                Console.WriteLine();
                Console.WriteLine("Press any key to return...");
                Console.ReadKey(true);
                break;
            }

            Console.WriteLine();

            // Keys mapping (ensure we have something to read even if labels change)
            var keyMap = new[]
            {
                settings.Controls.Choice1,
                settings.Controls.Choice2,
                settings.Controls.Choice3
            };
            var exitKey = settings.Controls.Exit;

            // Render up to 3 options
            for (int i = 0; i < node.Options.Count && i < keyMap.Length; i++)
            {
                var label = node.Options[i].LabelFactory?.Invoke() ?? node.Options[i].Label ?? "";
                Console.WriteLine($"{keyMap[i]}. {label}");
            }
            // Hint for exit (even if not a formal option)
            Console.WriteLine($"{exitKey}. Exit");

            // Read input (support exit at any time)
            int choice = -1;
            while (choice == -1)
            {
                var key = Console.ReadKey(true).Key.ToString().ToUpperInvariant();

                // Exit?
                if (key == exitKey.ToUpperInvariant())
                {
                    node = null; // end dialog
                    break;
                }

                // Choices
                for (int i = 0; i < node.Options.Count && i < keyMap.Length; i++)
                {
                    if (key == keyMap[i].ToUpperInvariant())
                    {
                        choice = i;
                        break;
                    }
                }
            }

            if (node == null) // Exit pressed
            {
                end = true;
                break;
            }

            var opt = node.Options[choice];

            // Execute option action (if any) and show feedback
            string? feedback = opt.Action?.Invoke();
            if (!string.IsNullOrWhiteSpace(feedback))
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(feedback);
                Console.ResetColor();

                // Re-render HUD after effects (heals, gold changes, repairs…)
                RenderPlayerHud(level.Player);

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }

            // Move to the next node (null → end)
            node = opt.Next;
            if (node == null) end = true;
        }

        // Mark the NPC as met, increase talk count
        npc.HasMet = true;
        npc.TimesTalked++;
    }

    // ---------- Helpers (no external dependency) ----------

    /// <summary>
    /// Minimal “typewriter” for text (word-by-word, with extra delay on punctuation).
    /// </summary>
    private static void TypeWrite(string text, int baseDelayMs, int commaExtra, int periodExtra)
    {
        var words = text.Replace("\r\n", "\n").Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            var token = words[i];
            if (token.Length == 0) { Console.Write(' '); continue; }

            Console.Write(token);

            // Compute delay depending on last char
            int delay = baseDelayMs;
            char last = token[^1];
            if (last is '.' or '!' or '?') delay += periodExtra;
            else if (last is ',' or ';' or ':') delay += commaExtra;

            // If token contains newline, don't delay (already a break)
            if (token.Contains('\n')) delay = 0;

            // Print a space after non-punctuation tokens (if next token exists)
            bool isPunct = token.Length == 1 && ".,;:!?".IndexOf(token[0]) >= 0;
            if (!isPunct && i < words.Length - 1) Console.Write(' ');

            Thread.Sleep(delay);
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Very small HUD for dialogues (simple, no colors per rarity here to keep Core clean).
    /// </summary>
    private static void RenderPlayerHud(Player p)
    {
        Console.WriteLine();
        Console.WriteLine($"Steps: {p.Steps} | LVL: {p.Level} | XP: {p.XP} | Gold: {p.Gold}");
        Console.WriteLine($"HP: {p.LifePoint}/{p.MaxLifePoint} | STR: {p.Strength} | ARM: {p.Armor} | SPD: {p.Speed} | VIS: {p.Vision}");

        if (p.Inventory.Any())
        {
            Console.WriteLine("Inventory:");
            foreach (var it in p.Inventory)
            {
                Console.WriteLine($"- {it.Name} ({it.EffectDescription}) [x{it.Value}]");
            }
        }
        Console.WriteLine();
    }
}
