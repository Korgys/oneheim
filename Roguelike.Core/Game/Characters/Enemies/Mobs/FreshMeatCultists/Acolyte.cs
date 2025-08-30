using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.FreshMeatCultists;

public class Acolyte : Enemy
{
    public Acolyte(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (12 + _random.Next(0, 3)) * level + (level - 1);
        MaxLifePoint = LifePoint;
        Armor = (4 + _random.Next(0, 2)) * level + (level - 1);
        Strength = (6 + _random.Next(0, 2)) * level + (level - 1);
        Speed = 4 * level + (level - 1);
        Vision = 2; // normal vision range
        Name = Messages.Acolyte;
        Category = EnemyType.Cultist;
    }
}