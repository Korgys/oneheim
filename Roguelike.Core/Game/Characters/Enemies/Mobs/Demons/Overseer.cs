using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Demons;

public class Overseer : Enemy
{
    public Overseer(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (6 + _random.Next(3)) * level;
        MaxLifePoint = LifePoint;
        Armor = (10 + _random.Next(2)) * level;
        Strength = (10 + _random.Next(2)) * level;
        Speed = (6 + _random.Next(2)) * level;
        Vision = 2; // Normal vision range
        Name = Messages.Overseer;
        Category = EnemyType.Demon;
    }
}
