using Roguelike.Console.Game.Levels;

namespace Roguelike.Console.Game.Rendering;

using Roguelike.Console.Game.Characters.Enemies;
using Roguelike.Console.Game.Characters.Players;
using Roguelike.Console.Properties.i18n;
using System;

public class CombatRendering
{
    public static void BlinkConsole(bool isBoss)
    {
        if (isBoss) BlinkConsoleForBoss();
        else BlinkConsoleForEnemy();
    }

    public static void RenderFight(Enemy e, Player p)
    {
        Console.Clear();

        int colWidth = 16;
        var enemyHP = $"{e.LifePoint}/{e.MaxLifePoint}";
        var playerHP = $"{p.LifePoint}/{p.MaxLifePoint}";

        Console.WriteLine(Messages.Fight);
        Console.WriteLine();
        Console.WriteLine($"{Player.Character.ToString().PadRight(colWidth)}{e.Name.ToString().PadLeft(colWidth)}");
        Console.WriteLine();
        Console.WriteLine($"{Messages.HP}: {playerHP}".PadRight(colWidth) + $"{Messages.HP}: {enemyHP}".PadLeft(colWidth));

        PrintStat(Messages.Strength, p.Strength, e.Strength, colWidth);
        PrintStat(Messages.Armor, p.Armor, e.Armor, colWidth);
        PrintStat(Messages.Speed, p.Speed, e.Speed, colWidth);

        Console.WriteLine();
    }

    public static void RenderEndFight(Enemy enemy, Player player, Queue<string> fightLog)
    {
        RenderFight(enemy, player);
        RenderLogFight(fightLog);

        // Manage rewards and level up
        if (player.LifePoint > 0)
        {
            // Save stats before xp gain
            int playerLevelBeforeXp = player.Level;
            Dictionary<string, int> playerStatsBeforeXp = new()
            {
                { "MaxLifePoint", player.MaxLifePoint },
                { "Strength", player.Strength },
                { "Armor", player.Armor },
                { "Speed", player.Speed }
            };

            // Render + xp gain
            RenderRewardsAndGainXp(enemy, player);

            // Check for level up
            if (player.Level > playerLevelBeforeXp)
            {
                RenderLevelUp(player, playerStatsBeforeXp);
            }
        }

        Console.WriteLine(Messages.PressAnyKeyToContinue);
        Console.ReadKey(true);
    }

    private static void RenderLogFight(Queue<string> fightLog)
    {
        Console.WriteLine(string.Join("\n", fightLog));
    }

    private static void RenderRewardsAndGainXp(Enemy e, Player p)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(Messages.Rewards);
        Console.ResetColor();
        int gold = e.GetGoldValue();
        int xp = e.GetXpValue();
        p.Gold += gold;
        p.GainXP(xp);
        Console.WriteLine($"- {gold} {Messages.Gold}");
        Console.WriteLine($"- {xp} {Messages.XP}");
        Console.WriteLine();
    }

    /// <summary>
    /// Render the level up message and the stats that have been increased.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="playerStatsBeforeXp"></param>
    private static void RenderLevelUp(Player player, Dictionary<string, int> playerStatsBeforeXp)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(string.Format(Messages.YouLeveledUp, player.Level));
        Console.ResetColor();
        foreach (var stat in playerStatsBeforeXp)
        {
            int newValue = stat.Key switch
            {
                "MaxLifePoint" => player.MaxLifePoint,
                "Strength" => player.Strength,
                "Armor" => player.Armor,
                "Speed" => player.Speed,
                _ => 0
            };
            if (stat.Value < newValue)
            {
                string label = Messages.ResourceManager.GetString(stat.Key) ?? stat.Key; // Translate stat name
                Console.WriteLine($"- {label}: {stat.Value} -> {newValue}");
            }
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Print a stat with its value for both player and enemy, with appropriate padding and color.
    /// </summary>
    /// <param name="label"></param>
    /// <param name="playerStat"></param>
    /// <param name="enemyStat"></param>
    /// <param name="colWidth"></param>
    private static void PrintStat(string label, int playerStat, int enemyStat, int colWidth)
    {
        string playerText = $"{label}: {playerStat}".PadRight(colWidth);
        string enemyText = $"{label}: {enemyStat}".PadLeft(colWidth);

        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(playerText);

        Console.ForegroundColor = GetColor(enemyStat - playerStat);
        Console.WriteLine(enemyText);

        Console.ResetColor();
    }

    /// <summary>
    /// Get the color based on the difference between player and enemy stats.
    /// </summary>
    /// <param name="diff"></param>
    /// <returns></returns>
    private static ConsoleColor GetColor(int diff)
    {
        return diff switch
        {
            >= 6 => ConsoleColor.Red,
            >= 2 => ConsoleColor.DarkYellow,
            <= -5 => ConsoleColor.DarkGray,
            _ => ConsoleColor.White
        };
    }

    private static void BlinkConsoleForEnemy()
    {
        Console.Clear();
        for (int i = 4; i >= 0; i--)
        {
            for (int y = 0; y < LevelManager.GridHeight; y++)
            {
                for (int x = 0; x < LevelManager.GridWidth; x++)
                {
                    Console.Write(i);
                }
                Console.WriteLine();
            }
            Thread.Sleep(100);
            Console.Clear();
        }
    }

    private static void BlinkConsoleForBoss()
    {
        Console.Clear();
        bool darkRed = true;
        for (int i = 10; i >= 0; i--)
        {
            for (int y = 0; y < LevelManager.GridHeight; y++)
            {
                for (int x = 0; x < LevelManager.GridWidth; x++)
                {
                    Console.Write(i);
                }
                Console.WriteLine();
            }
            Thread.Sleep(100);
            Console.Clear();
            Console.BackgroundColor = darkRed ? ConsoleColor.Black : ConsoleColor.DarkRed;
        }
    }
}
