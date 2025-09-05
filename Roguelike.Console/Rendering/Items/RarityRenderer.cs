namespace Roguelike.Console.Rendering.Items;

using Roguelike.Core.Game.Collectables.Items;
using System;

public class RarityRenderer
{
    /// <summary>
    /// Write text in the console with a color based on the item rarity.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="rarity"></param>
    public static void WriteColoredByRarity(string text, ItemRarity rarity)
    {
        var original = Console.ForegroundColor;

        Console.ForegroundColor = rarity switch
        {
            ItemRarity.Broken => ConsoleColor.DarkGray,
            ItemRarity.Common => ConsoleColor.White,
            ItemRarity.Uncommon => ConsoleColor.DarkCyan,
            ItemRarity.Rare => ConsoleColor.Blue,
            ItemRarity.Epic => ConsoleColor.Green,
            ItemRarity.Legendary => ConsoleColor.Yellow,
            _ => original
        };

        Console.Write(text);
        Console.ForegroundColor = original;
    }
}
