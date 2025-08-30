using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies.Bosses;

public class YannTheSilent : Boss
{
    public YannTheSilent(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = 8 * level * level; // lvl5: 200HP, lvl10: 800HP
        MaxLifePoint = LifePoint;
        Armor = 8 * level;             // lvl5: 40, lvl10: 80
        Strength = 16 * level;          // lvl5: 80, lvl10: 160
        Speed = 20 * level;             // lvl5: 100, lvl10: 200
        Name = Messages.YannTheSilent;
        Category = EnemyType.Outlaws;
        Inventory = new List<Item>
        {
        };
    }
}
