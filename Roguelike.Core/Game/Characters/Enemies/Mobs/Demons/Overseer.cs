using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Demons;

public class Overseer : Enemy
{
    public Overseer(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (6 + _random.Next(3)) * level;
        MaxLifePoint = LifePoint;
        Armor = (15 + _random.Next(2)) * level;
        Strength = (15 + _random.Next(2)) * level;
        Speed = (6 + _random.Next(2)) * level;
        Vision = 3; // Good vision range
        Name = Messages.Overseer;
        Category = EnemyType.Demon;
    }
}
