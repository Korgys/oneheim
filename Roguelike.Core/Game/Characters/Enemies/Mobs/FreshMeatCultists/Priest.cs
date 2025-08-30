using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.FreshMeatCultists;

public class Priest : Enemy
{
    public Priest(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (30 + _random.Next(0, 3)) * level;
        MaxLifePoint = LifePoint;
        Armor = (3 + _random.Next(0, 2)) * level;
        Strength = (16 + _random.Next(0, 2)) * level;
        Speed = 2 * level + (level - 1);
        Name = Messages.Priest;
        Category = EnemyType.Cultist;
        StepsPerTurn = 0; // static
    }
}
