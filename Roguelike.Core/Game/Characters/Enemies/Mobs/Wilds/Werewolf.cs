using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Wilds;

public class Werewolf : Enemy
{
    public Werewolf(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (30 + _random.Next(0, 8)) * level;
        MaxLifePoint = LifePoint;
        Armor = (8 + _random.Next(0, 4)) * level;
        Strength = (20 + _random.Next(0, 4)) * level;
        Speed = (20 + _random.Next(0, 4)) * level;
        Vision = 5; // best vision range for a mob
        Name = Messages.Werewolf;
        Category = EnemyType.Wild;
        StepsPerTurn = 2; // move really fast
    }
}