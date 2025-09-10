using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Undeads;

public class LeglessZombie : Enemy
{
    public LeglessZombie(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (5 + _random.Next(3)) * level;
        MaxLifePoint = LifePoint;
        Armor = (3 + _random.Next(2)) * level; 
        Strength = (3 + _random.Next(2)) * level;
        Speed = 1 * level; // Slowest enemy in the game
        Vision = 1; // Limited vision range
        Name = Messages.LeglessZombie;
        Category = EnemyType.Undead;
    }
}
