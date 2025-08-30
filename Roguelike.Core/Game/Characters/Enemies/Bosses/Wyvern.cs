using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Bosses;

public class Wyvern : Boss
{
    public Wyvern(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = 12 * level * level; // lvl5: 300HP, lvl10: 1200HP
        MaxLifePoint = LifePoint;
        Armor = 10 * level;             // lvl5: 50, lvl10: 100
        Strength = 10 * level;          // lvl5: 50, lvl10: 100
        Speed = 18 * level;             // lvl5: 80, lvl10: 180
        Name = Messages.TheJuvenileGreenWyvern;
        Category = EnemyType.Wild;
        Inventory = new List<Item>
        {
            new Item
            {
                Id = ItemId.RoyalGuardShield,
                Name = Messages.RoyalGuardShield,
                Effect = Messages.RoyalGuardShieldDescription,
                Value = 15
            },
        };
    }
}