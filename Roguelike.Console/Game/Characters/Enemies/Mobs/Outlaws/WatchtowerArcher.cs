using Roguelike.Console.Properties.i18n;

namespace Roguelike.Console.Game.Characters.Enemies.Mobs.Humans;

public class WatchtowerArcher : Enemy
{
    public WatchtowerArcher(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (20 + _random.Next(0, 11)) * level + (level - 1);
        MaxLifePoint = LifePoint;
        Armor = (5 + _random.Next(0, 2)) * level + (level - 1);
        Strength = (13 + _random.Next(0, 2)) * level + (level - 1);
        Speed = (4 + _random.Next(0, 2)) * level + (level - 1);
        Name = Messages.WatchtowerArcher;
        Category = EnemyType.Outlaws;
        StepsPerTurn = 0; // static
    }
}