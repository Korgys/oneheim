using Roguelike.Console.Game.Collectables.Items;
using Roguelike.Console.Properties.i18n;

namespace Roguelike.Console.Game.Characters.Enemies.Bosses;

public class Lich : Boss
{
    public Lich(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = 10 * level * level; // lvl5: 250HP, lvl10: 1000HP
        MaxLifePoint = LifePoint;
        Armor = 14 * level;             // lvl5: 70, lvl10: 150
        Strength = 18 * level;          // lvl5: 90, lvl10: 180
        Speed = 10 * level;             // lvl5: 50, lvl10: 100
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
                Value = 4 * level * level // lvl5: 100, lvl10: 400
            }
        };
    }
}
