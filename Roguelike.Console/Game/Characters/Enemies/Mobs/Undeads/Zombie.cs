using Roguelike.Console.Properties.i18n;

namespace Roguelike.Console.Game.Characters.Enemies.Mobs.Undeads;

public class Zombie : Enemy
{
    public Zombie(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (10 + _random.Next(0, 3)) * level + (level - 1);
        MaxLifePoint = LifePoint;
        Armor = (5 + _random.Next(0, 2)) * level + (level - 1);
        Strength = (5 + _random.Next(0, 2)) * level + (level - 1);
        Speed = 2 * level + (level - 1); // Zombies are slow
        Vision = 1; // Limited vision range
        Name = Messages.Zombie;
        Category = EnemyType.Undead;
    }
}
