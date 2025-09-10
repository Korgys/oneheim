using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Undeads;

public class PlagueGhoul : Enemy
{
    public PlagueGhoul(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (30 + _random.Next(10)) * level;
        MaxLifePoint = LifePoint;
        Armor = (7 + _random.Next(4)) * level;
        Strength = (18 + _random.Next(2)) * level;
        Speed = (1 + _random.Next(2)) * level;
        Name = Messages.PlagueGhoul;
        Category = EnemyType.Undead;
        StepsPerTurn = 0; // static
    }
}
