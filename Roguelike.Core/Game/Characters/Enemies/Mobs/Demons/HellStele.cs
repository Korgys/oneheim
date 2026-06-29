using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Demons;

public class HellStele : Enemy
{
    public HellStele(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (7 + _random.Next(3)) * level;
        MaxLifePoint = LifePoint;
        Armor = (14 + _random.Next(2)) * level;
        Strength = (10 + _random.Next(2)) * level;
        Speed = (7 + _random.Next(2)) * level;
        Name = Messages.HellStele;
        Category = EnemyType.Demon;
        StepsPerTurn = 0; // static
    }
}