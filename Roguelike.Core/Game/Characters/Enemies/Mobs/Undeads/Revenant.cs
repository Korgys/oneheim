using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Undeads;

public class Revenant : Enemy
{
    public Revenant(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (30 + _random.Next(0, 5)) * level;
        MaxLifePoint = LifePoint;
        Armor = (10 + _random.Next(0, 3)) * level; 
        Strength = (12 + _random.Next(0, 4)) * level;
        Speed = (10 + _random.Next(0, 2)) * level; // Revenants are faster than regular zombies but still slower than most enemies
        Vision = 3; // good vision range
        Name = Messages.Revenant;
        Category = EnemyType.Undead;
    }
}
