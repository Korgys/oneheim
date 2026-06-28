using Roguelike.Core.Configuration;
using Roguelike.Core.Game.Levels;
using Roguelike.Core.Properties.i18n;

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

        mainMenu = Node(() => npc.HasMet ? Messages.Get("EberIntroReturning") : Messages.Get("EberIntroFirst"));

        mainMenu.Options.Add(new DialogueOption
        {
            LabelFactory = () => Messages.Format("HireOneheimGuard", GetUnitPrice()),
            Action = () =>
            {
                int price = GetUnitPrice();
                if (player.Gold < price) return Messages.YouDoNotHaveEnoughGold;

                if (!level.TryHireMercenaries(1, out int hired, out string reason))
                    return reason;

                player.Gold -= price;
                return hired > 0
                    ? Messages.Get("HiredOneheimGuard")
                    : Messages.Get("NoSuitableGuardSpot");
            },
            Next = mainMenu
        });

        mainMenu.Options.Add(new DialogueOption
        {
            LabelFactory = () => Messages.Format("HireOneheimSquad", GetSquadPrice()),
            Action = () =>
            {
                int price = GetSquadPrice();
                if (player.Gold < price) return Messages.YouDoNotHaveEnoughGold;

                if (!level.TryHireMercenaries(3, out int hired, out string reason))
                    return reason;

                if (hired == 0) return Messages.Get("NoSuitableSquadSpots");

                player.Gold -= price;
                return hired == 3
                    ? Messages.Get("HiredOneheimSquad")
                    : Messages.Format("HiredPartialGuardSquad", hired);
            },
            Next = mainMenu
        });

        string[] lines =
        {
            Messages.Get("EberSmallTalk1"),
            Messages.Get("EberSmallTalk2"),
            Messages.Get("EberSmallTalk3"),
            Messages.Get("EberSmallTalk4")
        };
        var talk = Node(() => lines[_random.Next(lines.Length)]);
        talk.Options.Add(new DialogueOption { Label = Messages.Back, Next = mainMenu });

        mainMenu.Options.Add(new DialogueOption { Label = Messages.JustTalking, Next = talk });
        mainMenu.Options.Add(new DialogueOption { Label = Messages.Goodbye, Next = null });

        npc.Root = mainMenu;
    }
}
