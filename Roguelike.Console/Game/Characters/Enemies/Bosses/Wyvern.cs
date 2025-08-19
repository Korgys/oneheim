using Roguelike.Console.Game.Collectables.Items;
using Roguelike.Console.Properties.i18n;

namespace Roguelike.Console.Game.Characters.Enemies.Bosses;

public class Wyvern : Boss
{
    public Wyvern(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = 8 * level * level; // lvl5: 200HP, lvl10: 800HP
        MaxLifePoint = LifePoint;
        Armor = 11 * level;             // lvl5: 55, lvl10: 110
        Strength = 11 * level;          // lvl5: 55, lvl10: 110
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