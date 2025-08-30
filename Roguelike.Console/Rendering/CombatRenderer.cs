namespace Roguelike.Console.Rendering;

using Roguelike.Core.Game.Abstractions;
using Roguelike.Core.Game.Characters.Players;
using Roguelike.Core.Game.Characters.Enemies;
using System;
using System.Collections.Generic;

public sealed class ConsoleCombatRenderer : ICombatRenderer
{
    public void OnCombatStart(bool isBoss)
    {
        Blink(isBoss);
    }

    public void RenderTurn(Enemy enemy, Player player, IReadOnlyCollection<string> logLines)
    {
        Console.Clear();

        int colWidth = 16;
        var enemyHP = $"{enemy.LifePoint}/{enemy.MaxLifePoint}";
        var playerHP = $"{player.LifePoint}/{player.MaxLifePoint}";

        System.Console.WriteLine("Fight!");
        System.Console.WriteLine();

        System.Console.WriteLine($"{Player.Character.ToString().PadRight(colWidth)}{enemy.Name.PadLeft(colWidth)}");
        System.Console.WriteLine();
        System.Console.WriteLine($"HP: {playerHP}".PadRight(colWidth) + $"HP: {enemyHP}".PadLeft(colWidth));

        PrintStat("Attack", player.Strength, enemy.Strength, colWidth);
        PrintStat("Armor", player.Armor, enemy.Armor, colWidth);
        PrintStat("Speed", player.Speed, enemy.Speed, colWidth);

        System.Console.WriteLine();
        foreach (var line in logLines)
            System.Console.WriteLine(line);
    }

    public void OnCombatEnd(Enemy enemy, Player player, IReadOnlyCollection<string> finalLogLines)
    {
        // Show final state + logs and wait for key
        RenderTurn(enemy, player, finalLogLines);
        System.Console.WriteLine();
        System.Console.WriteLine("Press any key to continue...");
        System.Console.ReadKey(true);
    }

    private static void PrintStat(string label, int playerStat, int enemyStat, int colWidth)
    {
        string playerText = $"{label}: {playerStat}".PadRight(colWidth);
        string enemyText = $"{label}: {enemyStat}".PadLeft(colWidth);

        System.Console.ForegroundColor = ConsoleColor.White;
        System.Console.Write(playerText);

        System.Console.ForegroundColor = GetColor(enemyStat - playerStat);
        System.Console.WriteLine(enemyText);

        System.Console.ResetColor();
    }

    private static ConsoleColor GetColor(int diff) =>
        diff switch
        {
            >= 6 => ConsoleColor.Red,
            >= 2 => ConsoleColor.DarkYellow,
            <= -5 => ConsoleColor.DarkGray,
            _ => ConsoleColor.White
        };

    private static void Blink(bool isBoss)
    {
        if (isBoss) BlinkBoss();
        else BlinkEnemy();
    }

    private static void BlinkEnemy()
    {
        System.Console.Clear();
        for (int i = 4; i >= 0; i--)
        {
            for (int y = 0; y < 22; y++)
            {
                for (int x = 0; x < 60; x++)
                    System.Console.Write(i);
                System.Console.WriteLine();
            }
            System.Threading.Thread.Sleep(100);
            System.Console.Clear();
        }
    }

    private static void BlinkBoss()
    {
        System.Console.Clear();
        bool darkRed = true;
        for (int i = 10; i >= 0; i--)
        {
            for (int y = 0; y < 22; y++)
            {
                for (int x = 0; x < 60; x++)
                    System.Console.Write(i);
                System.Console.WriteLine();
            }
            System.Threading.Thread.Sleep(100);
            System.Console.Clear();
            System.Console.BackgroundColor = darkRed ? ConsoleColor.Black : ConsoleColor.DarkRed;
            darkRed = !darkRed;
        }
        System.Console.ResetColor();
    }
}
