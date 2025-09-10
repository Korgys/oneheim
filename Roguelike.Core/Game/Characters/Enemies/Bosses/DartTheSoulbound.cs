using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Bosses;

public class DartTheSoulbound : Boss
{
    public DartTheSoulbound(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = 7 * level * level; // lvl5: 175HP, lvl10: 700HP
        MaxLifePoint = LifePoint;
        Armor = 13 * level;             // lvl5: 60, lvl10: 120
        Strength = 13 * level;          // lvl5: 65, lvl10: 130
        Speed = 12 * level;             // lvl5: 65, lvl10: 130
        Name = Messages.TheSoulbound;
        Category = EnemyType.Demon;
        Inventory = new List<Item>();
    }
}
