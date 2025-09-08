using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Bosses;

public class HighPriest : Boss
{
    public HighPriest(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = 8 * level * level; // lvl5: 200HP, lvl10: 800HP
        MaxLifePoint = LifePoint;
        Armor = 9 * level;              // lvl5: 45, lvl10: 90
        Strength = 18 * level;          // lvl5: 90, lvl10: 180
        Speed = 10 * level;             // lvl5: 50, lvl10: 100
        Name = Messages.HighPriest;
        Category = EnemyType.Cultist;
        Inventory = new List<Item>
        {
            new Item
            {
                Id = ItemId.SealOfLivingFlesh,
                Name = Messages.SealOfLivingFlesh,
                Effect = Messages.SealOfLivingFleshDescription,
                Value = level,
            },
            new Item
            {
                Id = ItemId.RoyalGuardGauntlet,
                Name = Messages.RoyalGuardGauntlet,
                Effect = Messages.RoyalGuardGauntletDescription,
                Value = 2 * level,
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
