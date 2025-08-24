using Roguelike.Console.Properties.i18n;

namespace Roguelike.Console.Game.Characters.Enemies.Mobs.FreshMeatCultists;

public class Champion : Enemy
{
    public Champion(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (15 + _random.Next(0, 3)) * level + (level - 1);
        MaxLifePoint = LifePoint;
        Armor = (8 + _random.Next(0, 2)) * level + (level - 1);
        Strength = (12 + _random.Next(0, 2)) * level + (level - 1);
        Speed = 7 * level + (level - 1);
        Vision = 2; // normal vision range
        Name = Messages.Champion;
        Category = EnemyType.Cultist;
    }
}
