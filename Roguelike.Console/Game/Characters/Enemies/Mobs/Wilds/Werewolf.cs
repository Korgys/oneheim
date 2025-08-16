using Roguelike.Console.Properties.i18n;

namespace Roguelike.Console.Game.Characters.Enemies.Mobs.Wilds;

public class Werewolf : Enemy
{
    public Werewolf(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (28 + _random.Next(0, 8)) * level + (level - 1);
        MaxLifePoint = LifePoint;
        Armor = (4 + _random.Next(0, 2)) * level + (level - 1);
        Strength = (15 + _random.Next(0, 5)) * level + (level - 1);
        Speed = (12 + _random.Next(0, 5)) * level + (level - 1);
        Vision = 4; // Wolves have a good vision range
        Name = Messages.Werewolf;
        Category = EnemyType.Wild;
        StepsPerTurn = 2; // move really fast
    }
}