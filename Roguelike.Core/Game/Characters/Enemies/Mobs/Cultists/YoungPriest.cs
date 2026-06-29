using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Cultists;

public class YoungPriest : Enemy
{
    public YoungPriest(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (17 + _random.Next(3)) * level;
        MaxLifePoint = LifePoint;
        Armor = (3 + _random.Next(2)) * level;
        Strength = (10 + _random.Next(2)) * level;
        Speed = (3 + _random.Next(2)) * level;
        Name = Messages.YoungPriest;
        Category = EnemyType.Cultist;
        StepsPerTurn = 0; // static
    }
}
