using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Bosses;

public class ChiefBrigand : Boss
{
    public ChiefBrigand(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = 10 * level * level; // lvl5: 250HP, lvl10: 1000HP
        MaxLifePoint = LifePoint;
        Armor = 12 * level;             // lvl5: 60, lvl10: 120
        Strength = 13 * level;          // lvl5: 65, lvl10: 130
        Speed = 13 * level;             // lvl5: 65, lvl10: 130
        Name = Messages.ChiefBrigand;
        Category = EnemyType.Outlaws;
        Inventory = new List<Item>
        {
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

