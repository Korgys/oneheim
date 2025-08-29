namespace Roguelike.Console.Game.Collectables;

using Roguelike.Console.Configuration;
using Roguelike.Console.Game.Characters.Players;
using Roguelike.Console.Game.Collectables.Items;
using Roguelike.Console.Properties.i18n;
using System;

public static class InventoryManager
{
    public static string TryAddItem(Player player, Item item, GameSettings settings)
    {
        string message = string.Empty;
        var existing = player.Inventory.FirstOrDefault(i => i.Id == item.Id);

        // Existing item
        if (existing != null)
        {
            if (existing.UpgradableIncrementValue != 0)
            {
                existing.Value += existing.UpgradableIncrementValue;
                if (existing.Rarity < ItemRarity.Legendary) existing.Rarity++;

                // Sort items
                player.Inventory = [.. player.Inventory.OrderByDescending(i => i.Rarity)];

                // Check if the item is a special item that affects player stats
                if (existing.Id == ItemId.GlassesOfClairvoyance && player.Vision < existing.Value)
                {
                    player.Vision = existing.Value;
                }

                return string.Format(Messages.UpgradedItemTo, item.Name, existing.Value);
            }
            else
            {
                return string.Format(Messages.YouAlreadyOwnItemNotUpgradable, item.Name);
            }
        }

        // Sort items
        player.Inventory = [.. player.Inventory.OrderByDescending(i => i.Rarity)];

        // Check if the item is a special item that affects player stats
        if (item.Id == ItemId.GlassesOfClairvoyance && player.Vision < item.Value)
        {
            player.Vision = item.Value;
        }

        // Check if the player can carry more items
        if (player.Inventory.Count >= 3)
        {
            return HandleFullInventory(player, item, settings);
        }

        // Add the item to the inventory
        player.Inventory.Add(item);
        return string.Format(Messages.FoundItem, item.Name, item.EffectDescription);
    }

    private static string HandleFullInventory(Player player, Item newItem, GameSettings settings)
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
            ItemManager.WriteColored($"{keys[i]}. {inventoryItem.Name} ({inventoryItem.EffectDescription})\n", inventoryItem.Rarity);
        }

        ItemManager.WriteColored($"{keys.Last()}. {newItem.Name} ({newItem.EffectDescription}).", newItem.Rarity);
        Console.WriteLine($" ({Messages.KeepCurrentInventory}).");

        int chosenItemToDrop = -1;
        while (chosenItemToDrop == -1)
        {
            var key = Console.ReadKey(true).Key.ToString().ToUpperInvariant();
            if (keys.Contains(key)) chosenItemToDrop = keys.IndexOf(key);
        }

        if (chosenItemToDrop < player.Inventory.Count)
        {
            var dropped = player.Inventory[chosenItemToDrop];
            player.Inventory.RemoveAt(chosenItemToDrop);
            player.Inventory.Add(newItem);
            return $"{Messages.Dropped} {dropped.Name}, {Messages.Picked} {newItem.Name}.";
        }
        else
        {
            return Messages.KeepCurrentInventory;
        }
    }
}
