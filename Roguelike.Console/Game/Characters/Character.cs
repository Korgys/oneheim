using Roguelike.Console.Game.Collectables.Items;

namespace Roguelike.Console.Game.Characters;

public abstract class Character
{
    public int X { get; set; }
    public int Y { get; set; }
    public int LifePoint { get; set; }
    public int MaxLifePoint { get; set; }
    public int Strength { get; set; }
    public int Armor { get; set; }
    public int Speed { get; set; }
    public int Vision { get; set; }
    public int Gold { get; set; }
    public int XP { get; set; } // Experience points
    public int Level { get; set; } = 1; // Character level
    public List<Item> Inventory { get; set; } = new List<Item>();

    protected static Random _random = new Random();

    public int GetGoldValue()
    {
        Random random = new Random();
        int minValue = Math.Max(1, (Level + Strength + Armor + Speed) / 2);
        return random.Next(minValue, minValue * 2);
    }

    public int GetXpValue()
    {
        Random random = new Random();
        int minValue = Math.Max(1, (Level + Strength + Armor + Speed) / 2);
        return random.Next(minValue, minValue * 2);
    }

    public float GetLifeRatio()
    {
        return (float)LifePoint / (float)MaxLifePoint;
    }
}
