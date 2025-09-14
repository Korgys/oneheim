namespace Roguelike.Console.Rendering;

using Roguelike.Console.Rendering.Characters;
using Roguelike.Core.Game.Abstractions;
using Roguelike.Core.Game.Characters.Enemies;
using Roguelike.Core.Game.Characters.NPCs;
using Roguelike.Core.Game.Characters.NPCs.Allies;
using Roguelike.Core.Game.Collectables;
using Roguelike.Core.Game.GameLoop;
using Roguelike.Core.Properties.i18n;
using System.Runtime.CompilerServices;
using System.Text;
using System;

public sealed class ConsoleRenderer : IRenderer
{
    private static string? _lastGameMessage = null;

    /// <summary>
    /// helper: pack (x,y) into an int key (works for grids up to ~32k)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Key(int x, int y) => (y << 16) | (x & 0xFFFF);

    public void RenderFrame(GameStateView view)
    {
        var player = view.Player;
        int width = view.GridWidth;
        int height = view.GridHeight;

        var defaultBg = Console.BackgroundColor;
        var palette = DayNightPalette.For(view.DayCycle);

        // 1) Build O(1) lookups for dynamic stuff
        // If two entities share the same tile, the draw order below decides.
        var npcAt = new Dictionary<int, Npc>(capacity: view.Npcs.Count);
        var treasAt = new Dictionary<int, Treasure>(capacity: view.Treasures.Count);
        var enemyAt = new Dictionary<int, Enemy>(capacity: view.Enemies.Count);
        var mercAt = new Dictionary<int, Mercenary>(capacity: view.Mercenaries.Count);

        foreach (var npc in view.Npcs) npcAt[Key(npc.X, npc.Y)] = npc;
        foreach (var treasure in view.Treasures) treasAt[Key(treasure.X, treasure.Y)] = treasure;
        foreach (var enemy in view.Enemies) enemyAt[Key(enemy.X, enemy.Y)] = enemy;
        foreach (var mercenary in view.Mercenaries) mercAt[Key(mercenary.X, mercenary.Y)] = mercenary;

        // 2) Fog of war bounds (square vision)
        int left = player.X - player.Vision;
        int right = player.X + player.Vision;
        int top = player.Y - player.Vision;
        int bottom = player.Y + player.Vision;

        // 3) Prepare to emit fewer console calls (perf)
        Console.SetCursorPosition(0, 0); // cheaper than Clear() per frame
        Console.BackgroundColor = palette.BackgroundColor;

        // We'll collect segments with same color per line.
        var segment = new StringBuilder(width + 8);

        Console.SetCursorPosition(0, 0);
        Console.BackgroundColor = palette.BackgroundColor;

        // Ensure that the buffer is tall enough to hold the grid + HUD + messages
        const int HUD_LINES = 14;
        EnsureBufferHeight(height + HUD_LINES);

        // Render the grid
        for (int y = 0; y < height; y++)
        {
            Console.BackgroundColor = palette.BackgroundColor; // background limited to the grid
            ConsoleColor currentFg = (ConsoleColor)(-1);
            segment.Clear();

            if (y == 0 || y == height - 1)
            {
                EnsureColorAndFlush(ref currentFg, palette.BorderColor, segment);
                segment.Append(new string('=', width));
                FlushLineAndPad(segment, width, palette.BackgroundColor, defaultBg);
                continue;
            }

            // left border
            EnsureColorAndFlush(ref currentFg, palette.BorderColor, segment);
            segment.Append('=');

            int x = 1;
            while (x < width - 1)
            {
                // Fog of war run
                bool inFog = (x < left || x > right || y < top || y > bottom);
                if (inFog)
                {
                    int runStart = x, runEnd = x;
                    while (runEnd + 1 < width - 1 &&
                           ((runEnd + 1) < left || (runEnd + 1) > right || y < top || y > bottom))
                        runEnd++;

                    EnsureColorAndFlush(ref currentFg, palette.FogColor, segment);
                    segment.Append(new string('.', runEnd - runStart + 1));
                    x = runEnd + 1;
                    continue;
                }

                // Player
                if (x == player.X && y == player.Y)
                {
                    EnsureColorAndFlush(ref currentFg, palette.PlayerColor, segment);
                    segment.Append(ConsoleRendererCharacter.Player);
                    x++; continue;
                }

                // Structures/walls
                var structure = view.Structures.FirstOrDefault(s => s.Contains(x, y));
                if (structure != null && structure.IsWall(x, y))
                {
                    EnsureColorAndFlush(ref currentFg, palette.WallColor, segment);
                    segment.Append('X');
                    x++; continue;
                }

                // NPC
                if (npcAt.TryGetValue(Key(x, y), out var npc))
                {
                    EnsureColorAndFlush(ref currentFg, palette.NpcColor, segment);
                    segment.Append(ConsoleRendererCharacter.GetNpcCharacter(npc.Id));
                    x++; continue;
                }

                // Treasure
                if (treasAt.TryGetValue(Key(x, y), out _))
                {
                    EnsureColorAndFlush(ref currentFg, palette.TreasureColor, segment);
                    segment.Append(ConsoleRendererCharacter.Treasure);
                    x++; continue;
                }

                // Enemy
                if (enemyAt.TryGetValue(Key(x, y), out var enemy))
                {
                    EnsureColorAndFlush(ref currentFg, palette.EnemyColor, segment);
                    segment.Append(enemy.Character);
                    x++; continue;
                }

                // Mercenaries
                if (mercAt.TryGetValue(Key(x, y), out var merc))
                {
                    EnsureColorAndFlush(ref currentFg, palette.MercColor, segment);
                    segment.Append(merc.Character);
                    x++; continue;
                }

                // Empty
                segment.Append(' ');
                x++;
            }

            // right border
            EnsureColorAndFlush(ref currentFg, palette.BorderColor, segment);
            segment.Append('=');

            // Fin de ligne + padding hors grille en fond par défaut
            FlushLineAndPad(segment, width, palette.BackgroundColor, defaultBg);
        }

        // Reset colors after the grid
        Console.BackgroundColor = defaultBg;
        Console.ResetColor();

        // IMPORTANT: clear the HUD area before writing into it
        int hudTop = height;
        ClearLines(hudTop, 12, defaultBg); // 12 lines for HUD + messages

        Console.SetCursorPosition(0, hudTop);

        // Day and night banner
        RenderDayNightBanner(view);

        // Game messages
        var message = view.CurrentMessage;
        if (string.IsNullOrEmpty(message) && !string.IsNullOrEmpty(_lastGameMessage))
            message = _lastGameMessage;

        if (!string.IsNullOrEmpty(message))
        {
            if (message.Contains(Messages.BeCarefullYouAreNotSafeHere)
                || message.Contains(Messages.IsUnderAttack)
                || message.Contains(string.Format(Messages.HasBeenDestroy, Messages.BaseCamp))
                || message.Contains(Messages.GameOverPressEnterToExit)
                || message.Contains(Messages.KilledBy)
                || message.Contains(Messages.ABossArrives))
                Console.ForegroundColor = ConsoleColor.Red;
            else if (message.Contains(Messages.ANewTravelerComesToTheBaseCamp) 
                || message.Contains(Messages.YouDefeatedAllBosses))
                Console.ForegroundColor = ConsoleColor.Green;
            else
                Console.ForegroundColor = ConsoleColor.Gray;

            Console.WriteLine(message);
            Console.ResetColor();
            _lastGameMessage = message;
        }

        // HUD
        PlayerRenderer.RendererPlayerFullInfo(player);

        // Endgame + tutorial
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

    // helpers: flush segment if color change needed
    private static void EnsureColorAndFlush(ref ConsoleColor currentFg, ConsoleColor desired, StringBuilder sb)
    {
        if (currentFg != desired)
        {
            // écrire ce qui est accumulé avec l'ancienne couleur
            if (sb.Length > 0)
            {
                Console.Write(sb);
                sb.Clear();
            }
            Console.ForegroundColor = desired;
            currentFg = desired;
        }
    }

    // Write the current segment, then pad the rest of the line with spaces in default background
    private static void FlushLineAndPad(StringBuilder sb, int gridWidth, ConsoleColor gridBg, ConsoleColor defaultBg)
    {
        if (sb.Length > 0) Console.Write(sb);
        // fill the rest of the line if the console is wider than the grid
        int tail = Math.Max(0, Console.WindowWidth - gridWidth);
        if (tail > 0)
        {
            var keepFg = Console.ForegroundColor;
            var keepBg = Console.BackgroundColor;
            Console.BackgroundColor = defaultBg;
            Console.ForegroundColor = keepFg;
            Console.Write(new string(' ', tail));
            Console.BackgroundColor = keepBg;
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Render the day/night cycle banner with icon and progress bar
    /// </summary>
    /// <param name="view"></param>
    private static void RenderDayNightBanner(GameStateView view)
    {
        var palette = DayNightPalette.For(view.DayCycle);
        var icon = palette.Icon;

        int width = 20;
        int filled = Math.Clamp((int)Math.Round(view.CycleProgress * width), 0, width);
        var bar = new string('■', filled) + new string('─', width - filled);

        var keepBg = Console.BackgroundColor;
        var keepFg = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"[{icon}] ");
        Console.ForegroundColor = palette.ProgressBarColor;
        Console.WriteLine(bar);

        Console.ForegroundColor = keepFg;
        Console.BackgroundColor = keepBg;
    }

    public void ShowMessages(IEnumerable<string> messages)
    {
        foreach (var msg in messages) Console.WriteLine(msg);
    }

    private static void EnsureBufferHeight(int minHeight)
    {
        try
        {
            // Agrandit le buffer si nécessaire (ne réduit jamais)
            if (Console.BufferHeight < minHeight)
            {
                int bw = Math.Max(Console.BufferWidth, Console.WindowWidth);
                int bh = Math.Max(minHeight, Console.BufferHeight);
                Console.SetBufferSize(bw, bh);
            }
        }
        catch
        {
            // Si redirection/host ne supporte pas, on ignore et on bornera plus bas
        }
    }

    private static void ClearLines(int startY, int count, ConsoleColor bg)
    {
        // Borne dans le buffer courant
        int maxY = Console.BufferHeight - 1;
        if (startY < 0) startY = 0;
        if (startY > maxY) return;

        int last = Math.Min(startY + count - 1, maxY);
        int w = Console.BufferWidth;

        var keepFg = Console.ForegroundColor;
        var keepBg = Console.BackgroundColor;

        Console.BackgroundColor = bg;
        for (int y = startY; y <= last; y++)
        {
            Console.SetCursorPosition(0, y);
            Console.Write(new string(' ', w));
        }

        Console.ForegroundColor = keepFg;
        Console.BackgroundColor = keepBg;

        // Remet le curseur au début de la zone nettoyée
        Console.SetCursorPosition(0, startY);
    }

}
