using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Bosses;

public class Troll : Boss
{
    public Troll(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = 32 * level * level; // lvl5: 800HP, lvl10: 3200HP
        MaxLifePoint = LifePoint;
        Armor = 8 * level;              // lvl5: 40, lvl10: 80
        Strength = 14 * level;          // lvl5: 70, lvl10: 140
        Speed = 6 * level;              // lvl5: 30, lvl10: 60
        Name = Messages.CaveTroll;
        Category = EnemyType.Wild;
        Inventory = new List<Item>
        {
            new Item
            {
                Id = ItemId.TrollMushroom,
                Name = Messages.TrollMushroom,
                Effect = Messages.TrollMushroomDescription,
                Value = 150
            },
            new Item
            {
                Id = ItemId.OldGiantWoodenClub,
                Name = Messages.OldGiantWoodenClub,
                Effect = Messages.OldGiantWoodenClubDescription,
                Value = 5,
            },
        };
    }
}
