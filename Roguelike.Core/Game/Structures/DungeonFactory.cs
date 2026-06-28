using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Structures;

public static class DungeonFactory
{
    private static readonly string[] RawLayout =
    [
        "XXXXX",
        "X   X",
        "X   X",
        "X   X",
        "XX XX",
    ];

    public static Structure CreateDungeon(int topLeftX, int topLeftY, int hp = 2500)
    {
        int entranceX = topLeftX + RawLayout[0].Length / 2;
        int entranceY = topLeftY + RawLayout.Length;
        HashSet<(int x, int y)> entranceTiles = new()
        {
            (entranceX, topLeftY + RawLayout.Length - 1),
            (entranceX, entranceY)
        };

        return new Structure(Messages.Get("Dungeon"), topLeftX, topLeftY, RawLayout, entranceTiles, hp);
    }
}
