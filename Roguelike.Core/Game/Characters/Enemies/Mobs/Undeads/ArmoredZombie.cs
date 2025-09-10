using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Undeads;

public class ArmoredZombie : Enemy
{
    public ArmoredZombie(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (15 + _random.Next(5)) * level;
        MaxLifePoint = LifePoint;
        Armor = (8 + _random.Next(3)) * level;
        Strength = (7 + _random.Next(3)) * level;
        Speed = 3 * level; // Zombies are slow
        Vision = 2; // Limited vision range
        Name = Messages.ArmoredZombie;
        Category = EnemyType.Undead;
    }
}
