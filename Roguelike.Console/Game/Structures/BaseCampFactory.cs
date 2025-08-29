using Roguelike.Console.Properties.i18n;

namespace Roguelike.Console.Game.Structures;

public static class BaseCampFactory
{
    private static string _baseCampName = Messages.BaseCamp;

    private static readonly string[] RawLayout =
    [
        "XXXX XXXX",
        "X       X",
        "X       X",
        "X       X",
        "X       X",
        "XXXX XXXX",
    ];

    public static Structure CreateBaseCamp(int topLeftX, int topLeftY, int hp = 1000)
    {
        int middleX = topLeftX + RawLayout[0].Length / 2;
        HashSet<(int x, int y)> entranceTiles = new()
        {
            (middleX, topLeftY),                         // Top entrance
            (middleX, topLeftY + 1),                     // Top entrance
            (middleX, topLeftY + RawLayout.Count() - 1), // Bottom entrance
            (middleX, topLeftY + RawLayout.Count()),     // Bottom entrance
        };
        return new Structure(_baseCampName, topLeftX, topLeftY, RawLayout, entranceTiles, hp);
    }
}
