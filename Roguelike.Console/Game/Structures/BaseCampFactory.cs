using Roguelike.Console.Properties.i18n;

namespace Roguelike.Console.Game.Structures;

public static class BaseCampFactory
{
    private static string _baseCampName = Messages.BaseCamp;

    // Replace dots by spaces in code for readability
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
        // Ensure layout contains only 'X' and ' ' (spaces)
        var layout = RawLayout.Select(s => s.Replace('.', ' ')).ToArray();
        return new Structure(_baseCampName, topLeftX, topLeftY, layout, hp);
    }
}
