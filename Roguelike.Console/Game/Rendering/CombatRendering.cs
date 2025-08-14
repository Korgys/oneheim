using Roguelike.Console.Game.Levels;

namespace Roguelike.Console.Game.Rendering;

using Roguelike.Console.Game.Characters.Enemies;
using Roguelike.Console.Game.Characters.Players;
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

        Console.WriteLine("Fight !");
        Console.WriteLine();
        Console.WriteLine($"{Player.Character.ToString().PadRight(colWidth)}{e.Name.ToString().PadLeft(colWidth)}");
        Console.WriteLine();
        Console.WriteLine($"HP: {playerHP}".PadRight(colWidth) + $"HP: {enemyHP}".PadLeft(colWidth));

        PrintStat("Attack", p.Strength, e.Strength, colWidth);
        PrintStat("Armor", p.Armor, e.Armor, colWidth);
        PrintStat("Speed", p.Speed, e.Speed, colWidth);

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

        Console.WriteLine("Press any key to continue...");
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
        Console.WriteLine("Rewards :");
        Console.ResetColor();
        int gold = e.GetGoldValue();
        int xp = e.GetXpValue();
        p.Gold += gold;
        p.GainXP(xp);
        Console.WriteLine($"- {gold} gold");
        Console.WriteLine($"- {xp} XP");
        Console.WriteLine();
    }

    private static void RenderLevelUp(Player player, Dictionary<string, int> playerStatsBeforeXp)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"You leveled up to level {player.Level} !");
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
            if (stat.Value < newValue) Console.WriteLine($"- {stat.Key}: {stat.Value} -> {newValue}");
        }
        Console.WriteLine();
    }

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
