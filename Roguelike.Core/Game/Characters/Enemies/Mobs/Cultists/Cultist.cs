using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Cultists;

public class Cultist : Enemy
{
    public Cultist(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (10 + _random.Next(3)) * level;
        MaxLifePoint = LifePoint;
        Armor = (3 + _random.Next(2)) * level;
        Strength = (8 + _random.Next(2)) * level;
        Speed = (3 + _random.Next(2)) * level;
        Vision = 2; // normal vision range
        Name = Messages.Cultist;
        Category = EnemyType.Cultist;
    }
}
