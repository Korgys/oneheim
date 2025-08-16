using Roguelike.Console.Properties.i18n;

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
                Name = Messages.DaggerOfLifeSteal,
                Effect = Messages.DaggerOfLifeStealDescription,
                Value = 1,
                UpgradableIncrementValue = 1
            },
            ItemId.CapeOfInvisibility => new Item
            {
                Id = id,
                Name = Messages.CapeOfInvisibility,
                Effect = Messages.CapeOfInvisibilityDescription,
                Value = 1,
                UpgradableIncrementValue = 1
            },
            ItemId.GlassesOfClairvoyance => new Item
            {
                Id = id,
                Name = Messages.GlassesOfClairvoyance,
                Effect = Messages.GlassesOfClairvoyanceDescription,
                Value = 1,
                UpgradableIncrementValue = 1
            },
            ItemId.BootsOfEchoStep => new Item
            {
                Id = id,
                Name = Messages.BootsOfEchoStep,
                Effect = Messages.BootsOfEchoStepDescription,
                Value = 5,
                UpgradableIncrementValue = 5
            },
            ItemId.TalismanOfTheLastBreath => new Item
            {
                Id = id,
                Name = Messages.TalismanOfTheLastBreath,
                Effect = Messages.TalismanOfTheLastBreathDescription,
                Value = 1,
                UpgradableIncrementValue = 0,
                Rarity = ItemRarity.Rare
            },
            ItemId.ThornBreastplate => new Item
            {
                Id = id,
                Name = Messages.ThornBreastplate,
                Effect = Messages.ThornBreastplateDescription,
                Value = 1,
                UpgradableIncrementValue = 1
            },
            ItemId.FeathersOfHope => new Item
            {
                Id = id,
                Name = Messages.FeathersOfHope,
                Effect = Messages.FeathersOfHopeDescription,
                Value = 1,
                UpgradableIncrementValue = 1
            },
            ItemId.RoyalGuardGauntlet => new Item
            {
                Id = id,
                Name = Messages.RoyalGuardGauntlet,
                Effect = Messages.RoyalGuardGauntletDescription,
                Value = 2,
                UpgradableIncrementValue = 1
            },
            ItemId.RoyalGuardShield => new Item
            {
                Id = id,
                Name = Messages.RoyalGuardShield,
                Effect = Messages.RoyalGuardShieldDescription,
                Value = 3,
                UpgradableIncrementValue = 1
            },
            ItemId.BerserkerNecklace => new Item
            {
                Id = id,
                Name = Messages.BerserkerNecklace,
                Effect = Messages.BerserkerNecklaceDescription,
                Value = 15,
                UpgradableIncrementValue = 5
            },
            ItemId.PaladinNecklace => new Item
            {
                Id = id,
                Name = Messages.PaladinNecklace,
                Effect = Messages.PaladinNecklaceDescription,
                Value = 20,
                UpgradableIncrementValue = 5
            },
            ItemId.HolyBible => new Item
            {
                Id = id,
                Name = Messages.HolyBible,
                Effect = Messages.HolyBibleDescription,
                Value = 3,
                UpgradableIncrementValue = 3
            },
            ItemId.SacredCrucifix => new Item
            {
                Id = id,
                Name = Messages.SacredCrucifix,
                Effect = Messages.SacredCrucifixDescription,
                Value = 3,
                UpgradableIncrementValue = 3
            },
            ItemId.RingOfEndurance => new Item
            {
                Id = id,
                Name = Messages.RingOfEndurance,
                Effect = Messages.RingOfEnduranceDescription,
                Value = 1,
                UpgradableIncrementValue = 0 // This item does not upgrade, so increment value is 0
            },
            ItemId.BladeOfHeroes => new Item
            {
                Id = id,
                Name = Messages.BladeOfHeroes,
                Effect = Messages.BladeOfHeroesDescription,
                Value = 5,
                UpgradableIncrementValue = 5
            },
            ItemId.ShieldOfChampion => new Item
            {
                Id = id,
                Name = Messages.ShieldOfChampion,
                Effect = Messages.ShieldOfChampionDescription,
                Value = 5,
                UpgradableIncrementValue = 5
            },
            ItemId.FluteOfHunter => new Item
            {
                Id = id,
                Name = Messages.FluteOfHunter,
                Effect = Messages.FluteOfHunterDescription,
                Value = 3,
                UpgradableIncrementValue = 3
            },
            ItemId.EngravedFangs => new Item
            {
                Id = id,
                Name = Messages.EngravedFangs,
                Effect = Messages.EngravedFangsDescription,
                Value = 3,
                UpgradableIncrementValue = 3
            },
            _ => throw new ArgumentException("Unknown item type")
        };
    }
}
