namespace Roguelike.Console.Rendering;

using Roguelike.Console.Configuration;
using Roguelike.Console.Game.Characters.Players;
using Roguelike.Console.Game.Collectables;
using Roguelike.Console.Game.Levels;
using Roguelike.Console.Game.Rendering;
using Roguelike.Console.Properties.i18n;
using System;

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

                // 8) Mercenaries allies
                if (level.Mercenaries.Any(m => m.X == x && m.Y == y))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(level.Mercenaries.First(m => m.X == x && m.Y == y).Character);
                    Console.ResetColor();
                    continue;
                }

                // 9) Interior of structure or empty space
                Console.Write(' ');
            }
            Console.WriteLine();
        }

        // Display base HP only if severly damaged and not under attack
        var baseStructure = level.Structures.FirstOrDefault(s => s.Name == Messages.BaseCamp);
        if (baseStructure != null && ((double)baseStructure.Hp / (double)baseStructure.MaxHp) <= 0.3 && !level.IsBaseCampUnderAttack())
        {
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

        PlayerRenderer.RendererPlayerFullInfo(player);

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
}
