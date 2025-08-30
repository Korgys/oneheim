using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Mobs.Outlaws;

public class GreedyMercenary : Enemy
{
    public GreedyMercenary(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (12 + _random.Next(0, 3)) * level + (level - 1);
        MaxLifePoint = LifePoint;
        Armor = (7 + _random.Next(0, 3)) * level + (level - 1); // strong armor
        Strength = (7 + _random.Next(0, 3)) * level + (level - 1);
        Speed = 3 * level + (level - 1); // normal
        Vision = 2; // normal vision range
        Name = Messages.Mercenary;
        Category = EnemyType.Outlaws;
    }
}
