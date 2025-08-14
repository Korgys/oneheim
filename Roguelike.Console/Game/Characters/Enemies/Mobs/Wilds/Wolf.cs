namespace Roguelike.Console.Game.Characters.Enemies.Mobs.Wilds;

public class Wolf : Enemy
{
    public Wolf(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (10 + _random.Next(0, 4)) * level + (level - 1);
        MaxLifePoint = LifePoint;
        Armor = (3 + _random.Next(0, 2)) * level + (level - 1);
        Strength = (6 + _random.Next(0, 4)) * level + (level - 1);
        Speed = (8 + _random.Next(0, 2)) * level + (level - 1);
        Vision = 3; // Wolves have a good vision range
        Name = "Wolf";
        Category = EnemyType.Wild;
        StepsPerTurn = 2; // move really fast
    }
}
