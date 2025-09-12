using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Wilds;

public class AlphaWolf : Enemy
{
    public AlphaWolf(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (15 + _random.Next(0, 4)) * level + (level - 1);
        MaxLifePoint = LifePoint;
        Armor = (4 + _random.Next(0, 2)) * level + (level - 1);
        Strength = (11 + _random.Next(0, 4)) * level + (level - 1);
        Speed = (12 + _random.Next(0, 4)) * level + (level - 1);
        Vision = 4; // Wolves have a good vision range
        Name = Messages.AlphaWolf;
        Category = EnemyType.Wild;
        StepsPerTurn = 2; // move really fast
    }
}
