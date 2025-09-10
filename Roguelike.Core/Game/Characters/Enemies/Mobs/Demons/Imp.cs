using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Demons;

public class Imp : Enemy
{
    public Imp(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (4 + _random.Next(2)) * level;
        MaxLifePoint = LifePoint;
        Armor = (1 + _random.Next(2)) * level;
        Strength = (3 + _random.Next(2)) * level;
        Speed = (3 + _random.Next(2)) * level;
        Vision = 2; // Normal vision range
        Name = Messages.Imp;
        Category = EnemyType.Demon;
    }
}