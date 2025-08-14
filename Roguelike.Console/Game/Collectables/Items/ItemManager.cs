namespace Roguelike.Console.Game.Collectables.Items;

using System;

public static class ItemManager
{
    public static void WriteColored(string text, ItemRarity rarity)
    {
        var original = Console.ForegroundColor;

        Console.ForegroundColor = rarity switch
        {
            ItemRarity.Broken => ConsoleColor.DarkGray,
            ItemRarity.Common => ConsoleColor.White,
            ItemRarity.Uncommon => ConsoleColor.Cyan,
            ItemRarity.Rare => ConsoleColor.Green,
            ItemRarity.Epic => ConsoleColor.Magenta,
            ItemRarity.Legendary => ConsoleColor.Yellow,
            _ => original
        };

        Console.Write(text);
        Console.ForegroundColor = original;
    }
}
