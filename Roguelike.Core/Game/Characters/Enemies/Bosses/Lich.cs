using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Bosses;

public class Lich : Boss
{
    public Lich(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = 10 * level * level; // lvl5: 250HP, lvl10: 1000HP
        MaxLifePoint = LifePoint;
        Armor = 10 * level;             // lvl5: 50, lvl10: 100
        Strength = 18 * level;          // lvl5: 90, lvl10: 180
        Speed = 7 * level;              // lvl5: 35, lvl10:  70
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
                Name = Messages.TalismanOfTheLastBreath,
                Effect = Messages.TalismanOfTheLastBreathDescription,
                Value = 4 * level * level // lvl5: 100, lvl10: 400
            }
        };
    }
}
