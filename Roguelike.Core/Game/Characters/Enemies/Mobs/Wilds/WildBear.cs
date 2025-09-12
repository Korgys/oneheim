using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Wilds;

public class WildBear : Enemy
{
    public WildBear(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (12 + _random.Next(4)) * level;
        MaxLifePoint = LifePoint;
        Armor = (4 + _random.Next(2)) * level;
        Strength = (6 + _random.Next(4)) * level;
        Speed = (6 + _random.Next(2)) * level;
        Vision = 2;
        Name = Messages.WildLittleBear;
        Category = EnemyType.Wild;
    }
}
