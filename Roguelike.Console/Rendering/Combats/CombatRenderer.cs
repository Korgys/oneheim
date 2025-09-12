namespace Roguelike.Console.Rendering.Combats;

using Roguelike.Core.Game.Abstractions;
using Roguelike.Core.Game.Characters.Enemies;
using Roguelike.Core.Game.Characters.Players;
using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Game.Combat;
using Roguelike.Core.Properties.i18n;
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

        int colWidthTitle = 24;
        int colWidth = 20;
        var enemyHP = $"{enemy.LifePoint}/{enemy.MaxLifePoint}";
        var playerHP = $"{player.LifePoint}/{player.MaxLifePoint}";

        Console.WriteLine("Fight!");
        Console.WriteLine();

        string playerTitle = $"{Player.Character} {player.Name} ({Messages.Lvl} {player.Level})";
        string enemyTitle = $"{enemy.Name} ({Messages.Lvl} {enemy.Level})";
        Console.WriteLine($"{playerTitle.PadRight(colWidthTitle)}{enemyTitle.PadLeft(colWidthTitle)}");
        Console.WriteLine();
        Console.WriteLine($"HP: {playerHP}".PadRight(colWidth) + $"HP: {enemyHP}".PadLeft(colWidth));

        PrintStat("Attack", player.Strength, enemy.Strength, colWidth);
        PrintStat("Armor", player.Armor, enemy.Armor, colWidth);
        PrintStat("Speed", player.Speed, enemy.Speed, colWidth);

        Console.WriteLine();
        foreach (var line in logLines)
            Console.WriteLine(line);
    }

    public void OnCombatEnd(Enemy enemy, Player player, IReadOnlyCollection<string> finalLogLines, CombatReport combatReport)
    {
        // Show final state + logs and wait for key
        RenderTurn(enemy, player, finalLogLines);

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
            RenderRewardsAndGainXp(enemy, player, combatReport);

            // Check for level up
            if (player.Level > playerLevelBeforeXp)
            {
                RenderLevelUp(player, playerStatsBeforeXp);
            }

            Console.WriteLine($"Niv: {player.Level} | Exp: {player.XP}/{player.GetNextLevelXP()}");
        }

        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
    }

    private static void RenderRewardsAndGainXp(Enemy e, Player p, CombatReport combatReport)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(Messages.Rewards);
        Console.ResetColor();

        Console.WriteLine($"- {combatReport.Gold} {Messages.Gold}");
        // EnchantedPouch logic: increase gold
        var enchantedPouch = p.Inventory.FirstOrDefault(i => i.Id == ItemId.EnchantedPouch);
        if (enchantedPouch != null)
        {
            var goldFromEnchantedPouch = Math.Max(1, combatReport.Gold * enchantedPouch.Value / 100);
            Console.WriteLine($"- {goldFromEnchantedPouch} {Messages.Gold} ({Messages.EnchantedPouch})");
        }

        Console.WriteLine($"- {combatReport.Xp} {Messages.XP}");
        // SealOfWisdom logic: increase xp
        var sealOfWisdom = p.Inventory.FirstOrDefault(i => i.Id == ItemId.SealOfWisdom);
        if (sealOfWisdom != null)
        {
            var xpFromSealOfWisdom = Math.Max(1, combatReport.Xp * sealOfWisdom.Value / 100);
            Console.WriteLine($"- {xpFromSealOfWisdom} {Messages.XP} ({Messages.SealOfWisdom})");
        }

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
