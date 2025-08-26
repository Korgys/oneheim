using Roguelike.Console.Properties.i18n;

namespace Roguelike.Console.Game.Characters.Enemies.Mobs.Outlaws;

public class Drunkard : Enemy
{
    public Drunkard(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (10 + _random.Next(0, 3)) * level + (level - 1);
        MaxLifePoint = LifePoint;
        Armor = (1 + _random.Next(0, 2)) * level + (level - 1); // weak armor
        Strength = (4 + _random.Next(0, 2)) * level + (level - 1);
        Speed = 2 * level + (level - 1); // slow
        Vision = 1; // weak vision range
        Name = Messages.Drunkard;
        Category = EnemyType.Outlaws;
    }
}