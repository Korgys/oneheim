using Roguelike.Console.Properties.i18n;

namespace Roguelike.Console.Game.Characters.Enemies.Mobs.Humans;

public class Brigand : Enemy
{
    public Brigand(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (10 + _random.Next(0, 3)) * level + (level - 1);
        MaxLifePoint = LifePoint;
        Armor = (4 + _random.Next(0, 2)) * level + (level - 1);
        Strength = (5 + _random.Next(0, 2)) * level + (level - 1);
        Speed = 3 * level + (level - 1);
        Vision = 2; // normal vision range
        Name = Messages.Brigand;
        Category = EnemyType.Outlaws;
    }
}
