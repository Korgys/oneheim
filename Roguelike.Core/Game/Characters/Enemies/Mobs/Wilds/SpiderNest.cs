using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Wilds;

public class SpiderNest : Enemy
{
    public SpiderNest(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (30 + _random.Next(15)) * level;
        MaxLifePoint = LifePoint;
        Armor = level;
        Strength = (7 + _random.Next(9)) * level;
        Speed = (16 + _random.Next(9)) * level;
        Name = Messages.SpiderNest;
        Category = EnemyType.Wild;
        StepsPerTurn = 0; // static
    }
}
