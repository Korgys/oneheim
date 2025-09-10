using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Demons;

public class Hellhound : Enemy
{
    public Hellhound(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (6 + _random.Next(2)) * level;
        MaxLifePoint = LifePoint;
        Armor = (5 + _random.Next(2)) * level;
        Strength = (8 + _random.Next(2)) * level;
        Speed = (8 + _random.Next(2)) * level;
        Vision = 4; // Very good vision range
        StepsPerTurn = 2; // Can move 2 cases per turn
        Name = Messages.Hellhound;
        Category = EnemyType.Demon;
    }
}