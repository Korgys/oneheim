namespace Roguelike.Console.Rendering.Characters;

using Roguelike.Console.Rendering.Items;
using Roguelike.Core.Game.Characters.Players;
using Roguelike.Core.Properties.i18n;
using System;

public static class PlayerRenderer
{
    /// <summary>
    /// Print a compact single-line stats strip (used in treasure pick screens, etc.).
    /// </summary>
    public static void RendererPlayerStats(Player player)
    {
        // Example: HP: 8/12 | Str: 6 | Armor: 3 | Speed: 2 | Vision: 5 | Lvl: 3 | XP: 120 | Gold: 87 | Steps: 241
        Console.WriteLine(
            $"{Messages.HP}: {player.LifePoint}/{player.MaxLifePoint} | " +
            $"{Messages.Strength ?? "Strength"}: {player.Strength} | " +
            $"{Messages.Armor ?? "Armor"}: {player.Armor} | " +
            $"{Messages.Speed ?? "Speed"}: {player.Speed} | " +
            $"{Messages.Vision ?? "Vision"}: {player.Vision} | " +
            $"{Messages.Level ?? "Level"}: {player.Level} | " +
            $"{Messages.XP ?? "XP"}: {player.XP} | " +
            $"{Messages.Gold ?? "Gold"}: {player.Gold} | " +
            $"{Messages.Steps ?? "Steps"}: {player.Steps}"
        );
    }

    /// <summary>
    /// Print the player's inventory (items with colored rarity).
    /// </summary>
    public static void RenderPlayerInventory(Player player)
    {
        var count = player.Inventory.Count;
        if (count == 0) return;

        Console.WriteLine($"{Messages.Inventory}: {count}/{player.MaxInventorySize}");
        foreach (var it in player.Inventory)
        {
            RarityRenderer.WriteColoredByRarity($"- {it.Name} ({it.EffectDescription})", it.Rarity);
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Compact HUD tuned for dialogue screens: title line + two info lines + inventory.
    /// </summary>
    public static void RenderPlayerInfoInDialogues(Player player)
    {
        // Lines
        Console.WriteLine($"{Messages.HP}: {player.LifePoint}/{player.MaxLifePoint} | " +
                          $"{Messages.Level ?? "Level"}: {player.Level} | " +
                          $"{Messages.XP ?? "XP"}: {player.XP} | " +
                          $"{Messages.Gold ?? "Gold"}: {player.Gold}");

        Console.WriteLine($"{Messages.Strength ?? "Strength"}: {player.Strength} | " +
                          $"{Messages.Armor ?? "Armor"}: {player.Armor} | " +
                          $"{Messages.Speed ?? "Speed"}: {player.Speed} | " +
                          $"{Messages.Vision ?? "Vision"}: {player.Vision} | " +
                          $"{Messages.Steps ?? "Steps"}: {player.Steps}");

        // Inventory (short list)
        RenderPlayerInventory(player);
    }

    /// <summary>
    /// Full info block typically shown under the grid after a frame render.
    /// </summary>
    public static void RendererPlayerFullInfo(Player player)
    {
        // Primary stats
        Console.Write($"{Messages.Steps ?? "Steps"}: {player.Steps} ");
        Console.Write($"| {Messages.Lvl ?? "Lvl"}: {player.Level} ");
        Console.Write($"| {Messages.XP ?? "XP"}: {player.XP} ");
        Console.Write($"| {Messages.Gold ?? "Gold"}: {player.Gold}");
        Console.WriteLine();

        Console.Write($"{Messages.HP}: {player.LifePoint}/{player.MaxLifePoint} ");
        Console.Write($"| {Messages.Strength ?? "Strength"}: {player.Strength} ");
        Console.Write($"| {Messages.Armor ?? "Armor"}: {player.Armor} ");
        Console.Write($"| {Messages.Speed ?? "Speed"}: {player.Speed} ");
        Console.Write($"| {Messages.Vision ?? "Vision"}: {player.Vision}");
        Console.WriteLine();

        // Inventory (full)
        RenderPlayerInventory(player);
    }
}
