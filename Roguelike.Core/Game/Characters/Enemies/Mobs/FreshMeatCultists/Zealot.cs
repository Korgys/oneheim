using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.FreshMeatCultists;

public class Zealot : Enemy
{
    public Zealot(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (15 + _random.Next(0, 3)) * level + (level - 1);
        MaxLifePoint = LifePoint;
        Armor = (6 + _random.Next(0, 2)) * level + (level - 1);
        Strength = (9 + _random.Next(0, 2)) * level + (level - 1);
        Speed = 6 * level + (level - 1);
        Vision = 2; // normal vision range
        Name = Messages.Zealot;
        Category = EnemyType.Cultist;
    }
}