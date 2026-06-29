using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Humans;

public class OutlawSentinel : Enemy
{
    public OutlawSentinel(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (10 + _random.Next(9)) * level;
        MaxLifePoint = LifePoint;
        Armor = (5 + _random.Next(5)) * level;
        Strength = (5 + _random.Next(5)) * level;
        Speed = (4 + _random.Next(5)) * level;
        Name = Messages.OutlawSentinel;
        Category = EnemyType.Outlaws;
        StepsPerTurn = 0; // static
    }
}