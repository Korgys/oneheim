using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Cultists;

public class Novice : Enemy
{
    public Novice(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (5 + _random.Next(3)) * level;
        MaxLifePoint = LifePoint;
        Armor = (1 + _random.Next(2)) * level; // Weakiest enemy in the game
        Strength = (4 + _random.Next(2)) * level;
        Speed = (2 + _random.Next(2)) * level;
        Vision = 1; // Limited vision range
        Name = Messages.Novice;
        Category = EnemyType.Cultist;
    }
}
