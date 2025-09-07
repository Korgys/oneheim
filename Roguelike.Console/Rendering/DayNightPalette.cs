using Roguelike.Core.Game.Systems.Logics;
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Console.Rendering;

public static class DayNightPalette
{
    public readonly record struct Palette(
        ConsoleColor FogColor,
        ConsoleColor WallColor,
        ConsoleColor PlayerColor,
        ConsoleColor NpcColor,
        ConsoleColor EnemyColor,
        ConsoleColor MercColor,
        ConsoleColor TreasureColor,
        ConsoleColor BorderColor,
        ConsoleColor BackgroundColor,
        ConsoleColor ProgressBarColor,
        string Icon);

    public static Palette For(DayCycle cycle)
    {
        return cycle switch
        {
            DayCycle.Night => new Palette(
                FogColor: ConsoleColor.DarkGray,
                WallColor: ConsoleColor.DarkGray,
                PlayerColor: ConsoleColor.Gray,
                NpcColor: ConsoleColor.DarkGreen,
                EnemyColor: ConsoleColor.DarkRed,
                MercColor: ConsoleColor.DarkCyan,
                TreasureColor: ConsoleColor.DarkYellow,
                BorderColor: ConsoleColor.DarkGray,
                BackgroundColor: ConsoleColor.Black,
                ProgressBarColor: ConsoleColor.DarkBlue,
                Icon: Messages.Night
            ),
            DayCycle.Sunset => new Palette(
                FogColor: ConsoleColor.DarkYellow,
                WallColor: ConsoleColor.DarkGray,
                PlayerColor: ConsoleColor.Gray,
                NpcColor: ConsoleColor.Green,
                EnemyColor: ConsoleColor.Red,
                MercColor: ConsoleColor.Cyan,
                TreasureColor: ConsoleColor.Yellow,
                BorderColor: ConsoleColor.Yellow,
                BackgroundColor: ConsoleColor.DarkBlue,
                ProgressBarColor: ConsoleColor.Cyan,
                Icon: Messages.Sunset
            ),
            DayCycle.Sunrise => new Palette(
                FogColor: ConsoleColor.Yellow,
                WallColor: ConsoleColor.DarkGray,
                PlayerColor: ConsoleColor.Gray,
                NpcColor: ConsoleColor.Green,
                EnemyColor: ConsoleColor.Red,
                MercColor: ConsoleColor.Cyan,
                TreasureColor: ConsoleColor.Yellow,
                BorderColor: ConsoleColor.Yellow,
                BackgroundColor: ConsoleColor.DarkBlue,
                ProgressBarColor: ConsoleColor.Cyan,
                Icon: Messages.Sunrise
            ),
            _ => new Palette( // Day
                FogColor: ConsoleColor.White,
                WallColor: ConsoleColor.White,
                PlayerColor: ConsoleColor.White,
                NpcColor: ConsoleColor.Green,
                EnemyColor: ConsoleColor.Red,
                MercColor: ConsoleColor.Cyan,
                TreasureColor: ConsoleColor.Yellow,
                BorderColor: ConsoleColor.Gray,
                BackgroundColor: ConsoleColor.Blue,
                ProgressBarColor: ConsoleColor.DarkYellow,
                Icon: Messages.Day
            )
        };
    }
}

