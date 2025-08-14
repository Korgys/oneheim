using Roguelike.Console.Game.Collectables.Items;

namespace Roguelike.Console.Game.Characters.Enemies.Bosses;

public class Lich : Boss
{
    public Lich(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = 8 * level * level;
        MaxLifePoint = LifePoint;
        Armor = 20 * level;
        Strength = 20 * level;
        Speed = 20 * level;
        Vision = 30; // Boss vision range
        Name = "The Elder Lich";
        Category = EnemyType.Undead;
        Inventory = new List<Item>
        {
            new Item
            {
                Id = ItemId.DaggerLifeSteal,
                Name = "Life steal",
                Effect = "Steal up to {0} life points from the player on hit.",
                Value = level
            },
            new Item
            {
                Id = ItemId.TalismanOfTheLastBreath,
                Name = "Talisman of the Last Breath",
                Effect = "Cannot be instant kill",
                Value = 2 * level * level
            }
        };
    }
}
