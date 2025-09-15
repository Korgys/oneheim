using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Undeads;

public class Zombie : Enemy
{
    public Zombie(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (14 + _random.Next(4)) * level;
        MaxLifePoint = LifePoint;
        Armor = (7 + _random.Next(2)) * level;
        Strength = (7 + _random.Next(2)) * level;
        Speed = 2 * level; // Zombies are slow
        Vision = 1; // Limited vision range
        Name = Messages.Zombie;
        Category = EnemyType.Undead;
    }
}
