using Roguelike.Core.Game.Levels;
using System.Runtime.CompilerServices;

namespace Roguelike.Core.Game.Characters.Moves;

public sealed class DefaultWalkability : IWalkability
{
    private readonly LevelManager _level;

    public DefaultWalkability(LevelManager level) => _level = level;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Key(int x, int y) => (y << 16) | (x & 0xFFFF);

    public HashSet<int> BuildStaticBlockedSet()
    {
        var blocked = new HashSet<int>(128);

        // borders
        for (int x = 0; x < LevelManager.GridWidth; x++)
        {
            blocked.Add(Key(x, 0));
            blocked.Add(Key(x, LevelManager.GridHeight - 1));
        }
        for (int y = 0; y < LevelManager.GridHeight; y++)
        {
            blocked.Add(Key(0, y));
            blocked.Add(Key(LevelManager.GridWidth - 1, y));
        }

        // walls (>=10% HP)
        foreach (var s in _level.Structures)
        {
            if (!s.IsSeverelyEndomaged())
            {
                foreach (var p in s.WallTiles()) blocked.Add(Key(p.x, p.y));
            }
        }

        // treasures
        foreach (var t in _level.Treasures) blocked.Add(Key(t.X, t.Y));

        // mercenaries
        foreach (var m in _level.Mercenaries) blocked.Add(Key(m.X, m.Y));

        return blocked;
    }
}

