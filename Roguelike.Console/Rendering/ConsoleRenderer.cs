namespace Roguelike.Console.Rendering;

using Roguelike.Core.Game.Abstractions;
using Roguelike.Core.Game.GameLoop;
using Roguelike.Core.Properties.i18n;
using System;
using System.Collections.Generic;
using System.Linq;

public sealed class ConsoleRenderer : IRenderer
{
    private static string? _lastGameMessage = null;

    public void RenderFrame(GameStateView view)
    {
        Console.Clear();

        var player = view.Player;
        int width = view.GridWidth;
        int height = view.GridHeight;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // 1) Borders
                if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
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

                // 3) Structures (walls first)
                var structure = view.Structures.FirstOrDefault(s => s.Contains(x, y));
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
                    Console.Write(ConsoleRendererCharacter.Player);
                    continue;
                }

                // 5) NPCs
                var npc = view.Npcs.FirstOrDefault(n => n.X == x && n.Y == y);
                if (npc != null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(ConsoleRendererCharacter.GetNpcCharacter(npc.Id));
                    Console.ResetColor();
                    continue;
                }

                // 6) Treasures
                var treasure = view.Treasures.FirstOrDefault(t => t.X == x && t.Y == y);
                if (treasure != null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(ConsoleRendererCharacter.Treasure);
                    Console.ResetColor();
                    continue;
                }

                // 7) Enemies
                var enemy = view.Enemies.FirstOrDefault(e => e.X == x && e.Y == y);
                if (enemy != null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(enemy.Character);
                    Console.ResetColor();
                    continue;
                }

                // 8) Mercenaries
                var merc = view.Mercenaries.FirstOrDefault(m => m.X == x && m.Y == y);
                if (merc != null)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(merc.Character);
                    Console.ResetColor();
                    continue;
                }

                // 9) Interior/empty
                Console.Write(' ');
            }
            Console.WriteLine();
        }

        // Base structure HP (if severely damaged and not under attack)
        var baseStructure = view.Structures.FirstOrDefault(s => s.Name == Messages.BaseCamp);
        if (baseStructure != null)
        {
            double hpRatio = (double)baseStructure.Hp / baseStructure.MaxHp;
            if (hpRatio <= 0.3 && !view.IsBaseCampUnderAttack)
            {
                Console.Write(Messages.BaseCampHp);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{baseStructure.Hp}/{baseStructure.MaxHp}");
                Console.ResetColor();
                Console.WriteLine();
            }
        }

        // Game messages
        var message = view.CurrentMessage;
        if (string.IsNullOrEmpty(message) && !string.IsNullOrEmpty(_lastGameMessage))
            message = _lastGameMessage;

        if (!string.IsNullOrEmpty(message))
        {
            if (message.Contains(Messages.BeCarefullYouAreNotSafeHere)
                || message.Contains(Messages.IsUnderAttack)
                || message.Contains(string.Format(Messages.HasBeenDestroy, Messages.BaseCamp))
                || message.Contains(Messages.ABossArrives))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(message);
                Console.ResetColor();
            }
            else if (message.Contains(Messages.YouDefeatedAllBosses))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(message);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine(message);
            }

            _lastGameMessage = message;
        }

        // Player HUD
        PlayerRenderer.RendererPlayerFullInfo(player);

        // End of game
        if (view.IsGameEnded)
        {
            if (message != null && message.Contains(Messages.YouDefeatedAllBosses))
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
            // Controls help
            if (!view.HasPlayerUsedAValidKey)
            {
                var keys = view.Controls;
                Console.WriteLine(
                    $"{Messages.Move}: {keys.MoveUp},{keys.MoveRight},{keys.MoveDown},{keys.MoveLeft} | " +
                    $"{Messages.Choices}: {keys.Choice1},{keys.Choice2},{keys.Choice3} | " +
                    $"{Messages.Quit}: {keys.Exit}");
            }
        }
    }

    public void ShowMessages(IEnumerable<string> messages)
    {
        foreach (var msg in messages)
            Console.WriteLine(msg);
    }
}
