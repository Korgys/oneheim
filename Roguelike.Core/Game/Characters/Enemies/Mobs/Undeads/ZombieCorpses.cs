using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Undeads;

public class ZombieCorpses : Enemy
{
    public ZombieCorpses(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (30 + _random.Next(10)) * level;
        MaxLifePoint = LifePoint;
        Armor = (6 + _random.Next(2)) * level;
        Strength = (15 + _random.Next(2)) * level;
        Speed = (1 + _random.Next(2)) * level;
        Name = Messages.PileOfZombieCorpses;
        Category = EnemyType.Undead;
        StepsPerTurn = 0; // static
    }
}
