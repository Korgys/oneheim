using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Bosses;

public class AzrakelTheForsaken : Boss
{
    public AzrakelTheForsaken(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = 6 * level * level; // lvl5: 150HP, lvl10: 600HP
        MaxLifePoint = LifePoint;
        Armor = 17 * level;             // lvl5: 85, lvl10: 170
        Strength = 17 * level;          // lvl5: 85, lvl10: 170
        Speed = 8 * level;              // lvl5: 40, lvl10:  80
        Name = Messages.AzrakelTheForsaken;
        Category = EnemyType.Demon;
        Inventory = new List<Item>
        {
        };
    }
}

