namespace Roguelike.Console.Rendering.Combats;

public class CombatDisplayRules
{
    /// <summary>
    /// Get the color to display the enemy stat compared to the player stat.
    /// </summary>
    /// <param name="diff"></param>
    /// <returns></returns>
    public static ConsoleColor GetColorStatsGap(int diff) =>
        diff switch
        {
            >= 6 => ConsoleColor.Red,
            >= 2 => ConsoleColor.DarkYellow,
            <= -5 => ConsoleColor.DarkGray,
            _ => ConsoleColor.White
        };
}
