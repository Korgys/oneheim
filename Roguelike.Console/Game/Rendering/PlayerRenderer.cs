namespace Roguelike.Console.Game.Rendering;

using Roguelike.Console.Game.Characters.Players;
using Roguelike.Console.Game.Collectables.Items;
using Roguelike.Console.Properties.i18n;
using System;

public class PlayerRenderer
{
    public static void RendererPlayerFullInfo(Player player)
    {
        RendererPlayerXpAndGold(player);
        Console.WriteLine();
        RendererPlayerStats(player);
        RenderPlayerInventory(player);
    }

    public static void RendererPlayerXpAndGold(Player player)
    {
        Console.Write(string.Format(Messages.StepAndLevel, player.Steps, player.Level));
        if (player.XP > 0) Console.Write($" | {Messages.XP}: {player.XP}/{player.GetNextLevelXP()}");
        if (player.Gold > 0) Console.Write($" | {Messages.Gold}: {player.Gold}");
    }

    public static void RendererPlayerStats(Player player)
    {
        Console.WriteLine(
            $"{Messages.HP}: {player.LifePoint}/{player.MaxLifePoint} | " +
            $"{Messages.Strength}: {player.Strength} | {Messages.Armor}: {player.Armor} | " +
            $"{Messages.Speed}: {player.Speed} | {Messages.Vision}: {player.Vision}");
    }

    /// <summary>
    /// Renders the player's inventory to the console, displaying item details and inventory capacity.
    /// </summary>
    /// <remarks>This method outputs the player's inventory items, including their names, effects, and rarity,
    /// along with the current and maximum inventory capacity. If the inventory is empty, no output is
    /// produced.</remarks>
    /// <param name="player">The player whose inventory will be rendered.</param>
    public static void RenderPlayerInventory(Player player)
    {
        if (player != null && player.Inventory.Any())
        {
            Console.WriteLine($"{Messages.Inventory}: {player.Inventory.Count}/{player.MaxInventorySize}");
            foreach (var item in player.Inventory)
            {
                ItemManager.WriteColored($"- {item.Name} ({item.EffectDescription})", item.Rarity);
                Console.WriteLine();
            }
        }
    }

    public static void RenderPlayerInfoInDialogues(Player player)
    {
        Console.WriteLine();
        Console.WriteLine(new string('-', 50));
        RendererPlayerXpAndGold(player);
        Console.WriteLine();
        RendererPlayerStats(player);
    }
}
