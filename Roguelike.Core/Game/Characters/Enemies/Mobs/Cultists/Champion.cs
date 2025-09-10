using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Cultists;

public class Champion : Enemy
{
    public Champion(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (30 + _random.Next(3)) * level;
        MaxLifePoint = LifePoint;
        Armor = (5 + _random.Next(2)) * level;
        Strength = (16 + _random.Next(2)) * level;
        Speed = (9 + _random.Next(2)) * level;
        Vision = 3; // good vision range
        Name = Messages.Champion;
        Category = EnemyType.Cultist;
    }
}
