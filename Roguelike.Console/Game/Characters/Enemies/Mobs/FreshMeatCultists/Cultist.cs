using Roguelike.Console.Properties.i18n;

namespace Roguelike.Console.Game.Characters.Enemies.Mobs.FreshMeatCultists;

public class Cultist : Enemy
{
    public Cultist(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (7 + _random.Next(0, 3)) * level + (level - 1);
        MaxLifePoint = LifePoint;
        Armor = (3 + _random.Next(0, 2)) * level + (level - 1);
        Strength = (4 + _random.Next(0, 2)) * level + (level - 1);
        Speed = 3 * level + (level - 1);
        Vision = 2; // normal vision range
        Name = Messages.Cultist;
        Category = EnemyType.Cultist;
    }
}
