namespace Roguelike.Console.Rendering.Items;

using Roguelike.Core.Configuration;
using Roguelike.Core.Game.Abstractions;
using Roguelike.Core.Game.Characters.Players;
using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Properties.i18n;
using System.Linq;
using System;

public class InventoryUI : IInventoryUI
{
    public int PromptDropIndex(Player player, Item newItem, GameSettings settings)
    {
        Console.WriteLine();
        Console.WriteLine(Messages.InventoryFull);

        var keys = new List<string>
        {
            settings.Controls.Choice1,
            settings.Controls.Choice2,
            settings.Controls.Choice3,
            settings.Controls.Exit.ToUpper()
        };

        for (int i = 0; i < player.Inventory.Count && i < keys.Count; i++)
        {
            var inventoryItem = player.Inventory[i];
            RarityRenderer.WriteColoredByRarity($"{keys[i]}. {inventoryItem.Name} ({inventoryItem.EffectDescription})\n", inventoryItem.Rarity);
        }

        RarityRenderer.WriteColoredByRarity($"{keys.Last()}. {newItem.Name} ({newItem.EffectDescription}).", newItem.Rarity);
        Console.WriteLine($" ({Messages.KeepCurrentInventory}).");

        int chosenItemToDrop = -1;
        while (chosenItemToDrop == -1)
        {
            var key = Console.ReadKey(true).Key.ToString().ToUpperInvariant();
            if (keys.Contains(key)) chosenItemToDrop = keys.IndexOf(key);
        }

        return chosenItemToDrop;
    }
}
