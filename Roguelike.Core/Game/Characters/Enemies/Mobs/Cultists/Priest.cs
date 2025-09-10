using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Cultists;

public class Priest : Enemy
{
    public Priest(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (30 + _random.Next(3)) * level;
        MaxLifePoint = LifePoint;
        Armor = (4 + _random.Next(2)) * level;
        Strength = (16 + _random.Next(2)) * level;
        Speed = (4 + _random.Next(2)) * level;
        Name = Messages.Priest;
        Category = EnemyType.Cultist;
        StepsPerTurn = 0; // static
    }
}
