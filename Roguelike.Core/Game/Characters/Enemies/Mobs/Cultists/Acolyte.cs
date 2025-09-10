using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Cultists;

public class Acolyte : Enemy
{
    public Acolyte(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (7 + _random.Next(3)) * level;
        MaxLifePoint = LifePoint;
        Armor = (2 + _random.Next(2)) * level;
        Strength = (5 + _random.Next(2)) * level;
        Speed = (2 + _random.Next(2)) * level;
        Vision = 2; // normal vision range
        Name = Messages.Acolyte;
        Category = EnemyType.Cultist;
    }
}