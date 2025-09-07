using Roguelike.Console.Rendering;
using Roguelike.Console.Rendering.Characters;
using Roguelike.Core.Game.Abstractions;
using Roguelike.Core.Game.GameLoop;
using Roguelike.Core.Properties.i18n;

public sealed class ConsoleRenderer : IRenderer
{
    private static string? _lastGameMessage = null;

    public void RenderFrame(GameStateView view)
    {
        Console.Clear();

        var player = view.Player;
        int width = view.GridWidth;
        int height = view.GridHeight;

        var palette = DayNightPalette.For(view.DayCycle);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Console.BackgroundColor = palette.BackgroundColor;

                // 1) Borders
                if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                {
                    Console.ForegroundColor = palette.BorderColor;
                    Console.Write('=');
                    Console.ResetColor();
                    continue;
                }

                // 2) Fog of war
                bool inFog = Math.Abs(x - player.X) > player.Vision || Math.Abs(y - player.Y) > player.Vision;
                if (inFog)
                {
                    Console.ForegroundColor = palette.FogColor;
                    Console.Write('.');
                    Console.ResetColor();
                    continue;
                }

                // 3) Structures (walls first)
                var structure = view.Structures.FirstOrDefault(s => s.Contains(x, y));
                if (structure != null && structure.IsWall(x, y))
                {
                    Console.ForegroundColor = palette.WallColor;
                    Console.Write('X');
                    Console.ResetColor();
                    continue;
                }

                // 4) Player
                if (x == player.X && y == player.Y)
                {
                    Console.ForegroundColor = palette.PlayerColor;
                    Console.Write(ConsoleRendererCharacter.Player);
                    Console.ResetColor();
                    continue;
                }

                // 5) NPCs
                var npc = view.Npcs.FirstOrDefault(n => n.X == x && n.Y == y);
                if (npc != null)
                {
                    Console.ForegroundColor = palette.NpcColor;
                    Console.Write(ConsoleRendererCharacter.GetNpcCharacter(npc.Id));
                    Console.ResetColor();
                    continue;
                }

                // 6) Treasures
                var treasure = view.Treasures.FirstOrDefault(t => t.X == x && t.Y == y);
                if (treasure != null)
                {
                    Console.ForegroundColor = palette.TreasureColor;
                    Console.Write(ConsoleRendererCharacter.Treasure);
                    Console.ResetColor();
                    continue;
                }

                // 7) Enemies
                var enemy = view.Enemies.FirstOrDefault(e => e.X == x && e.Y == y);
                if (enemy != null)
                {
                    Console.ForegroundColor = palette.EnemyColor;
                    Console.Write(enemy.Character);
                    Console.ResetColor();
                    continue;
                }

                // 8) Mercenaries
                var merc = view.Mercenaries.FirstOrDefault(m => m.X == x && m.Y == y);
                if (merc != null)
                {
                    Console.ForegroundColor = palette.MercColor;
                    Console.Write(merc.Character);
                    Console.ResetColor();
                    continue;
                }

                // 9) Interior/empty
                Console.Write(' ');
            }
            Console.WriteLine();
        }

        // Base HP hint (inchangé)
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

        // Info cycle jour/nuit + barre
        RenderDayNightBanner(view);

        // Messages (inchangé)
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

        // HUD
        PlayerRenderer.RendererPlayerFullInfo(player);

        // Fin de partie + help (inchangés) …
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
        else if (!view.HasPlayerUsedAValidKey)
        {
            var keys = view.Controls;
            Console.WriteLine(
                $"{Messages.Move}: {keys.MoveUp},{keys.MoveRight},{keys.MoveDown},{keys.MoveLeft} | " +
                $"{Messages.Choices}: {keys.Choice1},{keys.Choice2},{keys.Choice3} | " +
                $"{Messages.Quit}: {keys.Exit}");
        }
    }

    private static void RenderDayNightBanner(GameStateView view)
    {
        var palette = DayNightPalette.For(view.DayCycle);
        var icon = palette.Icon;

        // petite barre 20 cases
        int width = 20;
        int filled = (int)Math.Round(view.CycleProgress * width);
        filled = Math.Clamp(filled, 0, width);

        var bar = new string('■', filled) + new string('─', width - filled);

        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"[{icon}] ");
        Console.ResetColor();

        Console.ForegroundColor = palette.ProgressBarColor;
        Console.WriteLine(bar);
        Console.ResetColor();
    }

    public void ShowMessages(IEnumerable<string> messages)
    {
        foreach (var msg in messages) Console.WriteLine(msg);
    }
}
