using Roguelike.Core.Configuration;
using Roguelike.Core.Game.Abstractions;
using Roguelike.Core.Game.Collectables;
using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Game.Levels;

namespace Roguelike.Core.Game.Characters.NPCs.Dialogues.Texts;

public static partial class NpcDialogues
{
    /// <summary>
    /// Ichem requires an ITreasurePicker so Core stays UI-agnostic.
    /// </summary>
    public static void BuildForIchem(Npc npc, LevelManager level, GameSettings settings, ITreasurePicker picker, IInventoryUI inventoryUI)
    {
        var player = level.Player;

        int GetCurrentPrice()
        {
            int basePrice = level.ChestPrice;
            var fidelityCard = player.Inventory.FirstOrDefault(i => i.Id == ItemId.FidelityCard);
            if (fidelityCard == null) return basePrice;

            float discountRate = Math.Clamp(fidelityCard.Value / 100f, 0f, 0.95f);
            int discounted = (int)MathF.Round(basePrice * (1 - discountRate));
            return Math.Max(1, discounted);
        }

        DialogueNode Node(Func<string> text) => new() { Text = text };

        string[] smallTalkLines =
        {
            "These lands change you. Sometimes for the better.",
            "Gold comes and goes. Choices linger.",
            "I once sold a boon that saved a kingdom. Or so they say.",
            "Storm’s coming. You can feel it in the stone.",
            "Power is a weight; spend it wisely."
        };

        var talk = Node(() => smallTalkLines[_random.Next(smallTalkLines.Length)]);
        talk.Options.Add(new DialogueOption { Label = "Back", Next = null });

        DialogueNode? mainMenu = null;

        mainMenu = Node(() =>
        {
            var intro = npc.HasMet ? "Back again, wanderer?" : "Greetings, wanderer.";
            return $"{intro} Care to buy a boon for {GetCurrentPrice()} gold?";
        });

        // Shop (uses the picker)
        mainMenu.Options.Add(new DialogueOption
        {
            LabelFactory = () => $"Buy a boon ({GetCurrentPrice()} gold)",
            Action = () =>
            {
                int price = GetCurrentPrice();
                if (player.Gold < price) return "You do not have enough gold.";

                // Let presentation layer drive the choice
                var chosen = TreasureSelector.ChooseWithPicker(player, settings, picker);

                player.Gold -= price;
                level.ChestPrice = (int)(level.ChestPrice * 1.06m); // soft inflation
                var msg = TreasureSelector.ApplyBonus(chosen, player, settings, inventoryUI);

                return $"Purchased: {msg}\nChest price has increased.";
            },
            Next = mainMenu
        });

        mainMenu.Options.Add(new DialogueOption
        {
            Label = "Just chat",
            Next = talk
        });

        mainMenu.Options.Add(new DialogueOption
        {
            Label = "Goodbye",
            Next = null
        });

        npc.Root = mainMenu;
    }
}
