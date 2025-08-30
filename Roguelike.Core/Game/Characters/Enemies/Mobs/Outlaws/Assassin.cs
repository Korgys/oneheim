using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Humans;

public class Assassin : Enemy
{
    public Assassin(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (15 + _random.Next(0, 4)) * level; // low life to compensate
        MaxLifePoint = LifePoint;
        Armor = (4 + _random.Next(0, 2)) * level + (level - 1); // low armor to compensate
        Strength = (22 + _random.Next(0, 3)) * level + (level - 1);
        Speed = (22 + _random.Next(0, 3)) * level + (level - 1); // some of the fastest enemies in the game
        Vision = 3;
        Name = Messages.Assassin;
        Category = EnemyType.Outlaws;
    }
}