using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Wilds;

public class RatsNest : Enemy
{
    public RatsNest(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (25 + _random.Next(10)) * level;
        MaxLifePoint = LifePoint;
        Armor = level;
        Strength = (4 + _random.Next(6)) * level;
        Speed = (13 + _random.Next(6)) * level;
        Name = Messages.RatsNest;
        Category = EnemyType.Wild;
        StepsPerTurn = 0; // static
    }
}
