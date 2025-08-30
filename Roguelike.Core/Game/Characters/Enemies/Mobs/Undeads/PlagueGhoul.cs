using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Undeads;

public class PlagueGhoul : Enemy
{
    public PlagueGhoul(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (30 + _random.Next(0, 11)) * level + (level - 1);
        MaxLifePoint = LifePoint;
        Armor = (2 + _random.Next(0, 2)) * level + (level - 1);
        Strength = (15 + _random.Next(0, 2)) * level + (level - 1);
        Speed = (2 + _random.Next(0, 2)) * level + (level - 1);
        Name = Messages.PlagueGhoul;
        Category = EnemyType.Undead;
        StepsPerTurn = 0; // static
    }
}
