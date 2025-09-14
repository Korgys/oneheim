using Roguelike.Core.Configuration;
using Roguelike.Core.Game.Abstractions;
using Roguelike.Core.Game.Collectables;
using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Game.Levels;
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.NPCs.Dialogues.Texts;

public static partial class NpcDialogues
{
    /// <summary>
    /// Ichem requires an ITreasurePicker so Core stays UI-agnostic.
    /// </summary>
    public static void BuildForIchem(Npc npc, LevelManager level, GameSettings settings, ITreasurePicker picker, IInventoryUI inventoryUI)
    {
        var player = level.Player;
        bool hasPurchased = false;

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
        talk.Options.Add(new DialogueOption { Label = Messages.Back, Next = null });

        DialogueNode? mainMenu = null;

        mainMenu = Node(() =>
        {
            if (hasPurchased) return string.Format(Messages.IchemIntroPurchase1, GetCurrentPrice());

            return npc.HasMet
             ? string.Format(Messages.IchemIntro2, GetCurrentPrice())
             : string.Format(Messages.IchemIntro1, GetCurrentPrice());
        });

        // Shop (uses the picker)
        mainMenu.Options.Add(new DialogueOption
        {
            LabelFactory = () => string.Format(Messages.BuyBoonForGold, GetCurrentPrice()),
            Action = () =>
            {
                int price = GetCurrentPrice();
                if (player.Gold < price) return Messages.YouDoNotHaveEnoughGold;

                // Let presentation layer drive the choice
                var chosen = TreasureSelector.ChooseWithPicker(player, settings, picker);

                player.Gold -= price;
                level.ChestPrice = (int)(level.ChestPrice * 1.06m); // soft inflation
                hasPurchased = true; // only for dialogue text purposes
                var msg = TreasureSelector.ApplyBonus(chosen, player, settings, inventoryUI);

                return $"{Messages.Purchased}: {msg}\n{Messages.ChestPriceHasIncreased}";
            },
            Next = mainMenu
        });

        mainMenu.Options.Add(new DialogueOption
        {
            Label = Messages.JustTalking,
            Next = talk
        });

        mainMenu.Options.Add(new DialogueOption
        {
            Label = Messages.Goodbye,
            Next = null
        });

        npc.Root = mainMenu;
    }
}
