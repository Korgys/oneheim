using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Demons;

public class DemonSlave : Enemy
{
    public DemonSlave(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (5 + _random.Next(2)) * level;
        MaxLifePoint = LifePoint;
        Armor = (4 + _random.Next(2)) * level;
        Strength = (5 + _random.Next(2)) * level;
        Speed = (3 + _random.Next(2)) * level;
        Vision = 1; // Limited vision range
        Name = Messages.DemonSlave;
        Category = EnemyType.Demon;
    }
}
