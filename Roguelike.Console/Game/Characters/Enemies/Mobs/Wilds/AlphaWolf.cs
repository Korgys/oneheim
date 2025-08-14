namespace Roguelike.Console.Game.Characters.Enemies.Mobs.Wilds;

public class AlphaWolf : Enemy
{
    public AlphaWolf(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (12 + _random.Next(0, 4)) * level + (level - 1);
        MaxLifePoint = LifePoint;
        Armor = (4 + _random.Next(0, 2)) * level + (level - 1);
        Strength = (10 + _random.Next(0, 4)) * level + (level - 1);
        Speed = (12 + _random.Next(0, 4)) * level + (level - 1);
        Vision = 4; // Wolves have a good vision range
        Name = "Alpha Wolf";
        Category = EnemyType.Wild;
        StepsPerTurn = 2; // move really fast
    }
}
