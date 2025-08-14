namespace Roguelike.Console.Game.Collectables.Items;

public class ItemFactory
{
    public static Item CreateItem(ItemId id)
    {
        return id switch
        {
            ItemId.DaggerLifeSteal => new Item
            {
                Id = id,
                Name = "Dagger of life steal",
                Effect = "60% chances to steal up to {0} life points from the enemy on hit.",
                Value = 1,
                UpgradableIncrementValue = 1
            },
            ItemId.CapeOfInvisibility => new Item
            {
                Id = id,
                Name = "Cape Of Invisibility",
                Effect = "Decrease by {0} enemies vision range",
                Value = 1,
                UpgradableIncrementValue = 1
            },
            ItemId.GlassesOfClairvoyance => new Item
            {
                Id = id,
                Name = "Glasses of Clairvoyance",
                Effect = "Your vision range cannot go below {0}",
                Value = 1,
                UpgradableIncrementValue = 1
            },
            ItemId.BootsOfEchoStep => new Item
            {
                Id = id,
                Name = "Boots of Echo Step",
                Effect = "Dodge increased by {0}% if your faster than you opponent",
                Value = 5,
                UpgradableIncrementValue = 5
            },
            ItemId.TalismanOfTheLastBreath => new Item
            {
                Id = id,
                Name = "Talisman of the Last Breath",
                Effect = "Survive only once to fatal blow",
                Value = 1,
                UpgradableIncrementValue = 0,
                Rarity = ItemRarity.Rare
            },
            ItemId.ThornBreastplate => new Item
            {
                Id = id,
                Name = "Thorn Breastplate",
                Effect = "60% chances that enemies take {0} damage if they hit you",
                Value = 1,
                UpgradableIncrementValue = 1
            },
            ItemId.FeathersOfHope => new Item
            {
                Id = id,
                Name = "Feathers Of Hope",
                Effect = "Gain {0}% HP on dodge",
                Value = 1,
                UpgradableIncrementValue = 1
            },
            ItemId.RoyalGuardGauntlet => new Item
            {
                Id = id,
                Name = "Royal Guard Gauntlet",
                Effect = "+{0}% critical chance",
                Value = 2,
                UpgradableIncrementValue = 1
            },
            ItemId.RoyalGuardShield => new Item
            {
                Id = id,
                Name = "Royal Guard Shield",
                Effect = "Reduce enemies critical chance by {0}%",
                Value = 3,
                UpgradableIncrementValue = 1
            },
            ItemId.BerserkerNecklace => new Item
            {
                Id = id,
                Name = "Berserker Necklace",
                Effect = "+{0}% critical damage",
                Value = 15,
                UpgradableIncrementValue = 5
            },
            ItemId.PaladinNecklace => new Item
            {
                Id = id,
                Name = "Paladin Necklace",
                Effect = "Reduce enemies critical chance by {0}%",
                Value = 20,
                UpgradableIncrementValue = 5
            },
            ItemId.HolyBible => new Item
            {
                Id = id,
                Name = "Holy Bible",
                Effect = "Reduce undead strengh by {0}",
                Value = 3,
                UpgradableIncrementValue = 3
            },
            ItemId.SacredCrucifix => new Item
            {
                Id = id,
                Name = "Sacred Crucifix",
                Effect = "Reduce undead armor by {0}",
                Value = 3,
                UpgradableIncrementValue = 3
            },
            _ => throw new ArgumentException("Unknown item type")
        };
    }
}
