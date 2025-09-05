namespace Roguelike.Console.Rendering.Combats;

using Roguelike.Core.Game.Abstractions;
using Roguelike.Core.Game.Characters.Players;
using Roguelike.Core.Game.Characters.Enemies;
using System;
using System.Collections.Generic;

public sealed class ConsoleCombatRenderer : ICombatRenderer
{
    public void OnCombatStart(bool isBoss)
    {
        CombatAnimations.Blink(isBoss);
    }

    public void RenderTurn(Enemy enemy, Player player, IReadOnlyCollection<string> logLines)
    {
        Console.Clear();

        int colWidth = 16;
        var enemyHP = $"{enemy.LifePoint}/{enemy.MaxLifePoint}";
        var playerHP = $"{player.LifePoint}/{player.MaxLifePoint}";

        Console.WriteLine("Fight!");
        Console.WriteLine();

        Console.WriteLine($"{Player.Character.ToString().PadRight(colWidth)}{enemy.Name.PadLeft(colWidth)}");
        Console.WriteLine();
        Console.WriteLine($"HP: {playerHP}".PadRight(colWidth) + $"HP: {enemyHP}".PadLeft(colWidth));

        PrintStat("Attack", player.Strength, enemy.Strength, colWidth);
        PrintStat("Armor", player.Armor, enemy.Armor, colWidth);
        PrintStat("Speed", player.Speed, enemy.Speed, colWidth);

        Console.WriteLine();
        foreach (var line in logLines)
            Console.WriteLine(line);
    }

    public void OnCombatEnd(Enemy enemy, Player player, IReadOnlyCollection<string> finalLogLines)
    {
        // Show final state + logs and wait for key
        RenderTurn(enemy, player, finalLogLines);
        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
    }

    private static void PrintStat(string label, int playerStat, int enemyStat, int colWidth)
    {
        string playerText = $"{label}: {playerStat}".PadRight(colWidth);
        string enemyText = $"{label}: {enemyStat}".PadLeft(colWidth);

        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(playerText);

        Console.ForegroundColor = CombatDisplayRules.GetColorStatsGap(enemyStat - playerStat);
        Console.WriteLine(enemyText);

        Console.ResetColor();
    }
}
