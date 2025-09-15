using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Demons;

public class Hellhound : Enemy
{
    public Hellhound(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (6 + _random.Next(2)) * level;
        MaxLifePoint = LifePoint;
        Armor = (5 + _random.Next(2)) * level;
        Strength = (9 + _random.Next(2)) * level;
        Speed = (7 + _random.Next(2)) * level;
        Vision = 5; // Best vision range in the game for a mob
        StepsPerTurn = 2; // Can move 2 cases per turn
        Name = Messages.Hellhound;
        Category = EnemyType.Demon;
    }
}