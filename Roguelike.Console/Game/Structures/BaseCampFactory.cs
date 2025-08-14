namespace Roguelike.Console.Game.Structures;

public static class BaseCampFactory
{
    // Replace dots by spaces in code for readability
    private static readonly string[] RawLayout = new[]
    {
        "XXXX XXXX",
        "X       X",
        "X       X",
        "X       X",
        "X       X",
        "XXXX XXXX",
    };

    public static Structure CreateBaseCamp(int topLeftX, int topLeftY, int hp = 1000)
    {
        // Ensure layout contains only 'X' and ' ' (spaces)
        var layout = RawLayout.Select(s => s.Replace('.', ' ')).ToArray();
        return new Structure("Base Camp", topLeftX, topLeftY, layout, hp);
    }
}
