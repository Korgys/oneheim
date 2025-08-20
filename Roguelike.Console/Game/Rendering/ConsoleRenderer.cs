namespace Roguelike.Console.Rendering;

using Roguelike.Console.Configuration;
using Roguelike.Console.Game.Characters.Players;
using Roguelike.Console.Game.Collectables;
using Roguelike.Console.Game.Collectables.Items;
using Roguelike.Console.Game.Levels;
using Roguelike.Console.Properties.i18n;
using System;
using System.Numerics;

public static class ConsoleRenderer
{
    private static string _lastGameMessage = null;

    public static void RenderGrid(
        LevelManager level,
        GameSettings settings,
        bool hasPlayerUsedAValidKey,
        string gameMessage,
        bool isGameEnded)
    {
        Console.Clear();
        var player = level.Player;

        for (int y = 0; y < LevelManager.GridHeight; y++)
        {
            for (int x = 0; x < LevelManager.GridWidth; x++)
            {
                // 1) Borders
                if (x == 0 || y == 0 || x == LevelManager.GridWidth - 1 || y == LevelManager.GridHeight - 1)
                {
                    Console.Write('=');
                    continue;
                }

                // 2) Fog of war
                if (Math.Abs(x - player.X) > player.Vision || Math.Abs(y - player.Y) > player.Vision)
                {
                    Console.Write('.');
                    continue;
                }

                // 3) Structure walls
                var structure = level.Structures.FirstOrDefault(s => s.Contains(x, y));
                if (structure != null && structure.IsWall(x, y))
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write('X');
                    Console.ResetColor();
                    continue;
                }

                // 4) Player
                if (x == player.X && y == player.Y)
                {
                    Console.Write(Player.Character);
                    continue;
                }

                // 5) NPCs
                var npc = level.Npcs.FirstOrDefault(n => n.X == x && n.Y == y);
                if (npc != null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(npc.Character);
                    Console.ResetColor();
                    continue;
                }

                // 6) Treasures
                var treasure = level.Treasures.FirstOrDefault(t => t.X == x && t.Y == y);
                if (treasure != null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(Treasure.Character);
                    Console.ResetColor();
                    continue;
                }

                // 7) Enemies
                var enemy = level.Enemies.FirstOrDefault(e => e.X == x && e.Y == y);
                if (enemy != null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(enemy.Character);
                    Console.ResetColor();
                    continue;
                }

                // 8) Interior of structure or empty space
                if (structure != null)
                {
                    Console.Write(' '); // interior floor
                }
                else
                {
                    Console.Write(' '); // empty floor
                }
            }
            Console.WriteLine();
        }

        // Display base HP only if severly damaged and not under attack
        var baseStructure = level.Structures.FirstOrDefault(s => s.Name == Messages.BaseCamp);
        if (baseStructure != null && ((double)baseStructure.Hp / (double)baseStructure.MaxHp) <= 0.3 && !level.IsBaseCampUnderAttack())
        {
            Console.WriteLine();
            Console.Write(Messages.BaseCampHp);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{baseStructure.Hp}/{baseStructure.MaxHp}");
            Console.ResetColor();
            Console.WriteLine();
        }

        // Display game messages
        if (string.IsNullOrEmpty(gameMessage) && !string.IsNullOrEmpty(_lastGameMessage))
        {
            gameMessage = _lastGameMessage;
        }
        if (!string.IsNullOrEmpty(gameMessage))
        {
            // Danger in red
            if (gameMessage.Contains(Messages.BeCarefullYouAreNotSafeHere)
                || gameMessage.Contains(Messages.IsUnderAttack)
                || gameMessage.Contains(string.Format(Messages.HasBeenDestroy, Messages.BaseCamp))
                || gameMessage.Contains(Messages.ABossArrives))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(gameMessage);
                Console.ResetColor();
            }
            // Good things in green
            else if (gameMessage.Contains(Messages.YouDefeatedAllBosses))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(gameMessage);
                Console.ResetColor();
            }
            // Normal
            else
            {
                Console.WriteLine(gameMessage);
            }

            _lastGameMessage = gameMessage;
        }

        // Player info
        Console.Write(string.Format(Messages.StepAndLevel, player.Steps, player.Level));
        if (player.XP > 0) Console.Write($" | {Messages.XP}: {player.XP}/{player.GetNextLevelXP()}");
        if (player.Gold > 0) Console.Write($" | {Messages.Gold}: {player.Gold}");
        Console.WriteLine();
        Console.WriteLine(
            $"{Messages.HP}: {player.LifePoint}/{player.MaxLifePoint} | " +
            $"{Messages.Strength}: {player.Strength} | {Messages.Armor}: {player.Armor} | " +
            $"{Messages.Speed}: {player.Speed} | {Messages.Vision}: {player.Vision}");

        RenderPlayerInventory(player);

        // End game or controls help
        if (isGameEnded)
        {
            if (gameMessage.Contains(Messages.YouDefeatedAllBosses))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Messages.TheEndPressEnterToExit);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Messages.GameOverPressEnterToExit);
            }

            Console.ResetColor();
            Console.ReadLine();
        }
        else
        {
            if (!hasPlayerUsedAValidKey)
            {
                var keys = settings.Controls;
                Console.WriteLine(
                    $"{Messages.Move}: {keys.MoveUp},{keys.MoveRight},{keys.MoveDown},{keys.MoveLeft} | " +
                    $"{Messages.Choices}: {keys.Choice1},{keys.Choice2},{keys.Choice3} | " +
                    $"{Messages.Quit}: {keys.Exit}");
            }
        }
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
}
