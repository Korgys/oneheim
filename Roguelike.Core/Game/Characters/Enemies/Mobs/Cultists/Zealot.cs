using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Cultists;

public class Zealot : Enemy
{
    public Zealot(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (13 + _random.Next(3)) * level;
        MaxLifePoint = LifePoint;
        Armor = (4 + _random.Next(2)) * level;
        Strength = (10 + _random.Next(2)) * level;
        Speed = (4 + _random.Next(2)) * level;
        Vision = 2; // normal vision range
        Name = Messages.Zealot;
        Category = EnemyType.Cultist;
    }
}