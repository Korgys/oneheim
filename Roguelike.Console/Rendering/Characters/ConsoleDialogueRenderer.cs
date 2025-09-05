namespace Roguelike.Console.Rendering.Characters;

using Roguelike.Core.Game.Abstractions;
using Roguelike.Core.Game.Characters.NPCs;
using Roguelike.Core.Game.Characters.NPCs.Dialogues;
using Roguelike.Core.Configuration;
using System;
using Roguelike.Core.Game.Characters.Players;

public sealed class ConsoleDialogueRenderer : IDialogueRenderer
{
    private readonly GameSettings _settings;

    public ConsoleDialogueRenderer(GameSettings settings)
    {
        _settings = settings;
    }

    public void ShowDialogue(Npc npc, Player player, DialogueNode root)
    {
        var node = root;

        while (node != null)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{npc.Name}:");
            Console.ResetColor();

            DialogueUI.RenderTypewriter(node.Text?.Invoke() ?? "", new TypewriterOptions
            {
                BaseWordDelayMs = 35,
                CommaExtraDelayMs = 120,
                PeriodExtraDelayMs = 240,
                EnableColorMarkup = true
            });

            if (node.Options.Count == 0)
            {
                Console.WriteLine("Press any key to return...");
                Console.ReadKey(true);
                break;
            }

            Console.WriteLine();

            var map = new[]
            {
                _settings.Controls.Choice1,
                _settings.Controls.Choice2,
                _settings.Controls.Choice3,
                _settings.Controls.Exit
            };

            for (int i = 0; i < node.Options.Count && i < map.Length; i++)
            {
                var label = node.Options[i].LabelFactory?.Invoke() ?? node.Options[i].Label;
                Console.WriteLine($"{map[i]}. {label}");
            }

            Console.WriteLine();
            PlayerRenderer.RenderPlayerInfoInDialogues(player);

            int choice = -1;
            while (choice == -1)
            {
                var key = Console.ReadKey(true).Key.ToString().ToUpperInvariant();
                for (int i = 0; i < node.Options.Count && i < map.Length; i++)
                    if (key == map[i].ToUpperInvariant())
                    {
                        choice = i;
                        break;
                    }
            }

            var opt = node.Options[choice];
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

            node = opt.Next;
        }
    }
}
