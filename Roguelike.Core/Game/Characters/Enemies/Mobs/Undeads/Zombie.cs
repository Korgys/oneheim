using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Undeads;

public class Zombie : Enemy
{
    public Zombie(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (13 + _random.Next(0, 3)) * level + (level - 1);
        MaxLifePoint = LifePoint;
        Armor = (4 + _random.Next(0, 2)) * level + (level - 1);
        Strength = (5 + _random.Next(0, 2)) * level + (level - 1);
        Speed = 2 * level + (level - 1); // Zombies are slow
        Vision = 1; // Limited vision range
        Name = Messages.Zombie;
        Category = EnemyType.Undead;
    }
}
