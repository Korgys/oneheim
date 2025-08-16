using Roguelike.Console.Game.Collectables.Items;
using Roguelike.Console.Properties.i18n;

namespace Roguelike.Console.Game.Characters.Enemies.Bosses;

public class Lich : Boss
{
    public Lich(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = 8 * level * level;
        MaxLifePoint = LifePoint;
        Armor = 18 * level;
        Strength = 18 * level;
        Speed = 18 * level;
        Vision = 30; // Boss vision range
        Name = Messages.TheElderLich;
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
