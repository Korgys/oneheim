using Roguelike.Console.Properties.i18n;

namespace Roguelike.Console.Game.Characters.Enemies.Mobs.Humans;

public class Assassin : Enemy
{
    public Assassin(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (10 + _random.Next(0, 3)) * level + (level - 1); // low life to compensate
        MaxLifePoint = LifePoint;
        Armor = (4 + _random.Next(0, 2)) * level + (level - 1);
        Strength = (15 + _random.Next(0, 2)) * level + (level - 1);
        Speed = (15 + _random.Next(0, 2)) * level + (level - 1); // some of the fastest enemies in the game
        Vision = 2;
        Name = Messages.Assassin;
        Category = EnemyType.Outlaws;
    }
}