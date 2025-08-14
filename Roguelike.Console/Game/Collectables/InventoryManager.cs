namespace Roguelike.Console.Game.Collectables;

using Roguelike.Console.Configuration;
using Roguelike.Console.Game.Characters.Players;
using Roguelike.Console.Game.Collectables.Items;
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

                return $"Upgraded {item.Name} to value {existing.Value}";
            }
            else
            {
                return $"You already own {item.Name}, not upgradable.";
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
        return $"Found item: {item.Name} – {item.EffectDescription}";
    }

    private static string HandleFullInventory(Player player, Item newItem, GameSettings settings)
    {
        Console.WriteLine("Inventory full. Choose item to drop:");

        var keys = new List<string>
        {
            settings.ControlsSettings.Choice1,
            settings.ControlsSettings.Choice2,
            settings.ControlsSettings.Choice3,
            settings.ControlsSettings.ExitGame
        };

        for (int i = 0; i < player.Inventory.Count && i < keys.Count; i++)
        {
            var inventoryItem = player.Inventory[i];
            ItemManager.WriteColored($"{keys[i]}. {inventoryItem.Name} ({inventoryItem.EffectDescription})\n", inventoryItem.Rarity);
        }

        ItemManager.WriteColored($"{keys.Last()}. {newItem.Name} ({newItem.EffectDescription}).", newItem.Rarity);
        Console.WriteLine(" (Keep current inventory).");

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
            return $"Dropped {dropped.Name}, picked {newItem.Name}.";
        }
        else
        {
            return "Kept current inventory.";
        }
    }
}
