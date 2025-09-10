using Roguelike.Core.Game.Characters.Enemies;

namespace Roguelike.Core.Game.Collectables.Items;

public static class ItemIdHelper
{
    public static List<ItemId> GetItemIdsSpecificByEnemyType(EnemyType enemyType)
    {
        return enemyType switch
        {
            EnemyType.Undead => new List<ItemId>
            {
                ItemId.HolyBible,
                ItemId.SacredCrucifix,
            },
            EnemyType.Wild => new List<ItemId>
            {
                ItemId.FluteOfHunter,
                ItemId.EngravedFangs
            },
            EnemyType.Outlaws => new List<ItemId>
            {
                ItemId.ArbalestBoltOfTheKingsValley,
                ItemId.NordheimWatcherLantern
            },
            EnemyType.Cultist => new List<ItemId>
            {
                ItemId.ButchersThornChaplet,
                ItemId.SauerkrautEffigy
            },
            //EnemyType.Demon => new List<ItemId>
            //{
            //},
            _ => new List<ItemId>()
        };
    }
}
