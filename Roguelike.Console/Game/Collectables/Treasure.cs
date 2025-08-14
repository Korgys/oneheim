namespace Roguelike.Console.Game.Collectables;

public class Treasure
{
    public static char Character { get; set; } = '$'; // Default treasure character
    public int X { get; set; }
    public int Y { get; set; }
    public BonusType Type { get; set; }
    public int Value { get; set; }
}
