using Roguelike.Core.Game.Characters.Enemies;
using Roguelike.Core.Game.Characters.Players;
using System.Runtime.CompilerServices;

namespace Roguelike.Core.Game.Characters.Moves;

public sealed class GreedyMover : IEnemyMover
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Key(int x, int y) => (y << 16) | (x & 0xFFFF);

    public bool TryStepTowardPlayer(
        Enemy enemy,
        Player player,
        int[,] dist,
        int gridWidth,
        int gridHeight,
        HashSet<int> staticBlocked,
        HashSet<int> occupiedNow,
        HashSet<int> reservedTargets,
        List<Enemy> combatQueue)
    {
        // engage if already adjacent
        if (Math.Abs(enemy.X - player.X) + Math.Abs(enemy.Y - player.Y) == 1)
        {
            combatQueue.Add(enemy);
            return false;
        }

        int ex = enemy.X, ey = enemy.Y;
        int here = dist[ey, ex];

        int bestNx = ex, bestNy = ey;
        int bestD = int.MaxValue;

        Span<(int dx, int dy)> dirs = stackalloc (int dx, int dy)[] { (1, 0), (-1, 0), (0, 1), (0, -1) };

        foreach (var (dx, dy) in dirs)
        {
            int nx = ex + dx, ny = ey + dy;
            if ((uint)nx >= (uint)gridWidth || (uint)ny >= (uint)gridHeight) continue;

            int key = Key(nx, ny);
            if (staticBlocked.Contains(key)) continue;
            if (occupiedNow.Contains(key)) continue;
            if (reservedTargets.Contains(key)) continue;

            int d = dist[ny, nx];
            if (d < bestD && d < here)
            {
                bestD = d;
                bestNx = nx;
                bestNy = ny;
            }
        }

        if (bestD == int.MaxValue) return false;

        // stepping onto the player is handled by adjacency check, but keep this as a guard
        if (bestNx == player.X && bestNy == player.Y)
        {
            combatQueue.Add(enemy);
            return false;
        }

        // reserve and move
        int toKey = Key(bestNx, bestNy);
        reservedTargets.Add(toKey);

        int fromKey = Key(ex, ey);
        occupiedNow.Remove(fromKey);
        occupiedNow.Add(toKey);

        enemy.X = bestNx;
        enemy.Y = bestNy;
        return true;
    }
}
