using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Undeads;

public class PlagueGhoul : Enemy
{
    public PlagueGhoul(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (25 + _random.Next(9)) * level;
        MaxLifePoint = LifePoint;
        Armor = (6 + _random.Next(4)) * level;
        Strength = (15 + _random.Next(2)) * level;
        Speed = (1 + _random.Next(2)) * level;
        Name = Messages.PlagueGhoul;
        Category = EnemyType.Undead;
        StepsPerTurn = 0; // static
    }
}
