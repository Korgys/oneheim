using Roguelike.Console.Properties.i18n;

namespace Roguelike.Console.Game.Characters.Enemies.Mobs.FreshMeatCultists;

public class Champion : Enemy
{
    public Champion(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (20 + _random.Next(0, 4)) * level;
        MaxLifePoint = LifePoint;
        Armor = (9 + _random.Next(0, 3)) * level;
        Strength = (15 + _random.Next(0, 4)) * level;
        Speed = 9 * level;
        Vision = 3; // good vision range
        Name = Messages.Champion;
        Category = EnemyType.Cultist;
    }
}
