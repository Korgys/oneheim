using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Demons;

public class DoomReaper : Enemy
{
    public DoomReaper(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (1 + _random.Next(level)) * level;
        MaxLifePoint = LifePoint;
        Armor = (10 + _random.Next(level)) * level;
        Strength = (15 + _random.Next(level)) * level;
        Speed = (10 + _random.Next(level)) * level;
        Name = Messages.DoomReaper;
        Category = EnemyType.Demon;
        Vision = 2; // Normal vision range
        StepsPerTurn = 2; // move really fast
    }
}