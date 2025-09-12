using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Wilds;

public class WildLittleBear : Enemy
{
    public WildLittleBear(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (6 + _random.Next(3)) * level;
        MaxLifePoint = LifePoint;
        Armor = (2 + _random.Next(2)) * level;
        Strength = (4 + _random.Next(2)) * level;
        Speed = (6 + _random.Next(2)) * level;
        Vision = 2;
        Name = Messages.WildLittleBear;
        Category = EnemyType.Wild;
    }
}
