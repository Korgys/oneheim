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
                Value = 10,
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
                Value = 10,
                UpgradableIncrementValue = 3
            },
            ItemId.RoyalGuardShield => new Item
            {
                Id = id,
                Name = Messages.RoyalGuardShield,
                Effect = Messages.RoyalGuardShieldDescription,
                Value = 10,
                UpgradableIncrementValue = 3
            },
            ItemId.BerserkerNecklace => new Item
            {
                Id = id,
                Name = Messages.BerserkerNecklace,
                Effect = Messages.BerserkerNecklaceDescription,
                Value = 30,
                UpgradableIncrementValue = 15
            },
            ItemId.PaladinNecklace => new Item
            {
                Id = id,
                Name = Messages.PaladinNecklace,
                Effect = Messages.PaladinNecklaceDescription,
                Value = 50,
                UpgradableIncrementValue = 10
            },
            ItemId.HolyBible => new Item
            {
                Id = id,
                Name = Messages.HolyBible,
                Effect = Messages.HolyBibleDescription,
                Value = 5,
                UpgradableIncrementValue = 4
            },
            ItemId.SacredCrucifix => new Item
            {
                Id = id,
                Name = Messages.SacredCrucifix,
                Effect = Messages.SacredCrucifixDescription,
                Value = 5,
                UpgradableIncrementValue = 4
            },
            ItemId.RingOfEndurance => new Item
            {
                Id = id,
                Name = Messages.RingOfEndurance,
                Effect = Messages.RingOfEnduranceDescription,
                Value = 1,
                UpgradableIncrementValue = 1
            },
            ItemId.BladeOfHeroes => new Item
            {
                Id = id,
                Name = Messages.BladeOfHeroes,
                Effect = Messages.BladeOfHeroesDescription,
                Value = 10,
                UpgradableIncrementValue = 5
            },
            ItemId.ShieldOfChampion => new Item
            {
                Id = id,
                Name = Messages.ShieldOfChampion,
                Effect = Messages.ShieldOfChampionDescription,
                Value = 10,
                UpgradableIncrementValue = 5
            },
            ItemId.FluteOfHunter => new Item
            {
                Id = id,
                Name = Messages.FluteOfHunter,
                Effect = Messages.FluteOfHunterDescription,
                Value = 5,
                UpgradableIncrementValue = 4
            },
            ItemId.EngravedFangs => new Item
            {
                Id = id,
                Name = Messages.EngravedFangs,
                Effect = Messages.EngravedFangsDescription,
                Value = 5,
                UpgradableIncrementValue = 4
            },
            ItemId.EnchantedPouch => new Item
            {
                Id = id,
                Name = Messages.EnchantedPouch,
                Effect = Messages.EnchantedPouchDescription,
                Value = 10,
                UpgradableIncrementValue = 10
            },
            ItemId.SealOfWisdom => new Item
            {
                Id = id,
                Name = Messages.SealOfWisdom,
                Effect = Messages.SealOfWisdomDescription,
                Value = 10,
                UpgradableIncrementValue = 10
            },
            ItemId.ProspectorKey => new Item
            {
                Id = id,
                Name = Messages.ProspectorKey,
                Effect = Messages.ProspectorKeyDescription,
                Value = 3,
                UpgradableIncrementValue = 4
            },
            ItemId.HawkEye => new Item
            {
                Id = id,
                Name = Messages.HawkEye,
                Effect = Messages.HawkEyeDescription,
                Value = 40,
                UpgradableIncrementValue = 20,
                Rarity = ItemRarity.Uncommon
            },
            ItemId.FidelityCard => new Item
            {
                Id = id,
                Name = Messages.FidelityCard,
                Effect = Messages.FidelityCardDescription,
                Value = 10,
                UpgradableIncrementValue = 10,
                Rarity = ItemRarity.Common // with this, max reduction is 50%, which is already pretty good
            },
            ItemId.TrollMushroom => new Item
            {
                Id = id,
                Name = Messages.TrollMushroom,
                Effect = Messages.TrollMushroomDescription,
                Value = 125,
                UpgradableIncrementValue = 25
            },
            ItemId.OldGiantWoodenClub => new Item
            {
                Id = id,
                Name = Messages.OldGiantWoodenClub,
                Effect = Messages.OldGiantWoodenClubDescription,
                Value = 3,
                UpgradableIncrementValue = 1
            },
            ItemId.LuckyMillorLeftHand => new Item
            {
                Id = id,
                Name = Messages.LuckyMillorLeftHand,
                Effect = Messages.LuckyMillorLeftHandDescription,
                Value = 10,
                UpgradableIncrementValue = 5
            },
            ItemId.GrolMokbarRing => new Item
            {
                Id = id,
                Name = Messages.GrolMokbarRing,
                Effect = Messages.GrolMokbarRingDescription,
                Value = 5,
                UpgradableIncrementValue = 5
            },
            ItemId.TalismanOfPeace => new Item
            {
                Id = id,
                Name = Messages.TalismanOfPeace,
                Effect = Messages.TalismanOfPeaceDescription,
                Value = 4,
                UpgradableIncrementValue = 4
            },
            ItemId.SealOfLivingFlesh => new Item
            {
                Id = id,
                Name = Messages.SealOfLivingFlesh,
                Effect = Messages.SealOfLivingFleshDescription,
                Value = 2,
                UpgradableIncrementValue = 2
            },
            _ => throw new ArgumentException("Unknown item type")
        };
    }
}
