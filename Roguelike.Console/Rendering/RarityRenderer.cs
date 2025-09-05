namespace Roguelike.Console.Rendering;

using Roguelike.Core.Game.Collectables.Items;
using System;

public class RarityRenderer
{
    public static void WriteColored(string text, ItemRarity rarity)
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
