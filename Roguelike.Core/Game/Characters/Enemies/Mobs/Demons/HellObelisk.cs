using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Demons;

public class HellObelisk : Enemy
{
    public HellObelisk(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (8 + _random.Next(3)) * level;
        MaxLifePoint = LifePoint;
        Armor = (20 + _random.Next(2)) * level;
        Strength = (20 + _random.Next(2)) * level;
        Speed = (8 + _random.Next(2)) * level;
        Name = Messages.HellObelisk;
        Category = EnemyType.Demon;
        StepsPerTurn = 0; // static
    }
}