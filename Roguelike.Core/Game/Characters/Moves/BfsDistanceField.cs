using System.Runtime.CompilerServices;

namespace Roguelike.Core.Game.Characters.Moves;

public sealed class BfsDistanceField : IDistanceField
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Key(int x, int y) => (y << 16) | (x & 0xFFFF);

    public int[,] Build(int w, int h, int playerX, int playerY, HashSet<int> staticBlocked, HashSet<int> occupiedEnemies)
    {
        var dist = new int[h, w];
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                dist[y, x] = int.MaxValue;

        var q = new Queue<(int x, int y)>();
        if ((uint)playerX < (uint)w && (uint)playerY < (uint)h)
        {
            dist[playerY, playerX] = 0;
            q.Enqueue((playerX, playerY));
        }

        Span<(int dx, int dy)> dirs = stackalloc (int dx, int dy)[] { (1, 0), (-1, 0), (0, 1), (0, -1) };

        while (q.Count > 0)
        {
            var (x, y) = q.Dequeue();
            int d = dist[y, x] + 1;

            foreach (var (dx, dy) in dirs)
            {
                int nx = x + dx, ny = y + dy;
                if ((uint)nx >= (uint)w || (uint)ny >= (uint)h) continue;

                int key = Key(nx, ny);
                if (staticBlocked.Contains(key)) continue;
                if (occupiedEnemies.Contains(key)) continue;

                if (d < dist[ny, nx])
                {
                    dist[ny, nx] = d;
                    q.Enqueue((nx, ny));
                }
            }
        }

        return dist;
    }
}
