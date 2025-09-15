using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Undeads;

public class Skeleton : Enemy
{
    public Skeleton(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (5 + _random.Next(3)) * level;
        MaxLifePoint = LifePoint;
        Armor = (4 + _random.Next(2)) * level;
        Strength = (6 + _random.Next(2)) * level;
        Speed = (1 + _random.Next(3)) * level;
        Vision = 1; // Limited vision range
        Name = Messages.Skeleton;
        Category = EnemyType.Undead;
    }
}