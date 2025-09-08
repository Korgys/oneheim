using Roguelike.Core.Configuration;
using Roguelike.Core.Game.Levels;

namespace Roguelike.Core.Game.Characters.NPCs.Dialogues.Texts;

public static partial class NpcDialogues
{
    public static void BuildForEber(Npc npc, LevelManager level, GameSettings settings)
    {
        var player = level.Player;

        int GetUnitPrice()
        {
            int basePrice = 120;
            int owned = level.Mercenaries.Count;
            return basePrice + Math.Min(owned * 10, 100);
        }

        int GetSquadPrice()
        {
            int unit = GetUnitPrice();
            return (int)MathF.Round(unit * 3 * 0.9f);
        }

        DialogueNode Node(Func<string> text) => new() { Text = text };

        DialogueNode? mainMenu = null;

        mainMenu = Node(() =>
        {
            var intro = npc.HasMet
                ? "Steel is cheap; loyalty is not. Looking to hire?"
                : "Name's Eber. I train Oneheim guards. You pay, they protect.";
            return intro;
        });

        mainMenu.Options.Add(new DialogueOption
        {
            LabelFactory = () => $"Hire a Oneheim guard ({GetUnitPrice()} gold)",
            Action = () =>
            {
                int price = GetUnitPrice();
                if (player.Gold < price) return "You do not have enough gold.";

                if (!level.TryHireMercenaries(1, out int hired, out string reason))
                    return reason;

                player.Gold -= price;
                return hired > 0
                    ? "Hired 1 Oneheim guard. He will circle the camp and strike nearby foes."
                    : "No suitable spot around the camp to deploy a guard.";
            },
            Next = mainMenu
        });

        mainMenu.Options.Add(new DialogueOption
        {
            LabelFactory = () => $"Hire a Oneheim squad (3) ({GetSquadPrice()} gold)",
            Action = () =>
            {
                int price = GetSquadPrice();
                if (player.Gold < price) return "You do not have enough gold.";

                if (!level.TryHireMercenaries(3, out int hired, out string reason))
                    return reason;

                if (hired == 0) return "No suitable spots around the camp to deploy a squad.";

                player.Gold -= price;
                return hired == 3
                    ? "Hired a Oneheim squad. They will patrol the perimeter and intercept threats."
                    : $"Hired {hired} guard(s). Patrol will start immediately.";
            },
            Next = mainMenu
        });

        string[] lines =
        {
            "Mercenaries keep blades sharp and eyes sharper.",
            "Perimeter first. Then the roads.",
            "Oneheim steel doesn’t bend.",
            "Pay now, bleed less later."
        };
        var talk = Node(() => lines[_random.Next(lines.Length)]);
        talk.Options.Add(new DialogueOption { Label = "Back", Next = mainMenu });

        mainMenu.Options.Add(new DialogueOption { Label = "Just talk", Next = talk });
        mainMenu.Options.Add(new DialogueOption { Label = "Goodbye", Next = null });

        npc.Root = mainMenu;
    }
}
