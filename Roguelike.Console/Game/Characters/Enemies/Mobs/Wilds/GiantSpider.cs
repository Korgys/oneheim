namespace Roguelike.Console.Game.Characters.Enemies.Mobs.Wilds;

public class GiantSpider : Enemy
{
    public GiantSpider(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (30 + _random.Next(0, 11)) * level + (level - 1);
        MaxLifePoint = LifePoint;
        Armor = 0;
        Strength = 10 * level + (level - 1);
        Speed = 10 * level + (level - 1);
        Name = "Spider Nest";
        Category = EnemyType.Wild;
        StepsPerTurn = 0; // static
    }
}
