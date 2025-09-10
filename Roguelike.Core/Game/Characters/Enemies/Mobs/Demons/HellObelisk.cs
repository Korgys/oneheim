using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Demons;

public class HellObelisk : Enemy
{
    public HellObelisk(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (10 + _random.Next(3)) * level;
        MaxLifePoint = LifePoint;
        Armor = (16 + _random.Next(2)) * level;
        Strength = (16 + _random.Next(2)) * level;
        Speed = (10 + _random.Next(2)) * level;
        Name = Messages.HellObelisk;
        Category = EnemyType.Demon;
        StepsPerTurn = 0; // static
    }
}