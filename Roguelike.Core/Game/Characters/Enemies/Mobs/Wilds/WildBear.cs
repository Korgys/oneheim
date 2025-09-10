using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Wilds;

public class WildBear : Enemy
{
    public WildBear(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (10 + _random.Next(0, 4)) * level + (level - 1);
        MaxLifePoint = LifePoint;
        Armor = (3 + _random.Next(0, 2)) * level + (level - 1);
        Strength = (6 + _random.Next(0, 4)) * level + (level - 1);
        Speed = (5 + _random.Next(0, 2)) * level + (level - 1);
        Vision = 1;
        Name = Messages.WildBear;
        Category = EnemyType.Wild;
        StepsPerTurn = 1;
    }
}
