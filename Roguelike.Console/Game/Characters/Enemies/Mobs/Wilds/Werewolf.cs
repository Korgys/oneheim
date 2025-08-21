using Roguelike.Console.Properties.i18n;

namespace Roguelike.Console.Game.Characters.Enemies.Mobs.Wilds;

public class Werewolf : Enemy
{
    public Werewolf(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (28 + _random.Next(0, 8)) * level + (level - 1);
        MaxLifePoint = LifePoint;
        Armor = (4 + _random.Next(0, 2)) * level + (level - 1);
        Strength = (13 + _random.Next(0, 4)) * level + (level - 1);
        Speed = (12 + _random.Next(0, 4)) * level + (level - 1);
        Vision = 3; // Wolves have a good vision range
        Name = Messages.Werewolf;
        Category = EnemyType.Wild;
        StepsPerTurn = 2; // move really fast
    }
}