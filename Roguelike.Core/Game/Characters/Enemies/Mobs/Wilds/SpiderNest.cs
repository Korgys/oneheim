using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Wilds;

public class SpiderNest : Enemy
{
    public SpiderNest(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (30 + _random.Next(0, 11)) * level + (level - 1);
        MaxLifePoint = LifePoint;
        Armor = 0;
        Strength = 10 * level + (level - 1);
        Speed = 10 * level + (level - 1);
        Name = Messages.SpiderNest;
        Category = EnemyType.Wild;
        StepsPerTurn = 0; // static
    }
}
