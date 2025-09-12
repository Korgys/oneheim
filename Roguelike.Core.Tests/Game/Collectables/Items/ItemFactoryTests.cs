using Roguelike.Core.Game.Collectables.Items;

namespace Roguelike.Core.Tests.Game.Collectables.Items
{
    [TestClass]
    public class ItemFactoryTests
    {
        // Covers every switch arm with simple, data-driven cases.
        [DataTestMethod]
        [DynamicData(nameof(GetItemCases), DynamicDataSourceType.Method)]
        public void CreateItem_KnownIds_ReturnsExpectedBasics(ItemId id, int expectedValue, int expectedInc)
        {
            // Act
            var item = ItemFactory.CreateItem(id);

            // Assert
            Assert.IsNotNull(item, "Factory returned null.");
            Assert.AreEqual(id, item.Id, "Id should flow through unchanged.");
            Assert.AreEqual(expectedValue, item.Value, $"Unexpected Value for {id}.");
            Assert.AreEqual(expectedInc, item.UpgradableIncrementValue, $"Unexpected UpgradableIncrementValue for {id}.");

            // Don’t assert the exact localized strings; just ensure they’re present.
            Assert.IsFalse(string.IsNullOrWhiteSpace(item.Name), "Name should be populated from i18n Messages.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(item.Effect), "Effect should be populated from i18n Messages.");
        }

        // Targeted rarity checks for the items that explicitly set Rarity in the factory.
        [DataTestMethod]
        [DataRow(ItemId.TalismanOfTheLastBreath, ItemRarity.Rare)]
        [DataRow(ItemId.HawkEye, ItemRarity.Uncommon)]
        [DataRow(ItemId.FidelityCard, ItemRarity.Common)]
        public void CreateItem_SpecificRarities_AreSet(ItemId id, ItemRarity expectedRarity)
        {
            var item = ItemFactory.CreateItem(id);
            Assert.AreEqual(expectedRarity, item.Rarity, $"Unexpected rarity for {id}.");
        }

        // Ensures the default branch is covered.
        [TestMethod]
        public void CreateItem_UnknownId_ThrowsArgumentException()
        {
            var unknown = (ItemId)999_999;
            Assert.ThrowsException<ArgumentException>(() => ItemFactory.CreateItem(unknown));
        }

        // ---- Test data mapping (easy to maintain in one place) ----
        // Only asserts numbers (Value & UpgradableIncrementValue) so tests don't depend on resource text.
        public static IEnumerable<object[]> GetItemCases()
        {
            yield return Row(ItemId.DaggerLifeSteal, 1, 1);
            yield return Row(ItemId.CapeOfInvisibility, 1, 1);
            yield return Row(ItemId.GlassesOfClairvoyance, 1, 1);
            yield return Row(ItemId.BootsOfEchoStep, 10, 5);
            yield return Row(ItemId.TalismanOfTheLastBreath, 1, 0);
            yield return Row(ItemId.ThornBreastplate, 1, 1);
            yield return Row(ItemId.FeathersOfHope, 1, 1);
            yield return Row(ItemId.RoyalGuardGauntlet, 10, 3);
            yield return Row(ItemId.RoyalGuardShield, 10, 3);
            yield return Row(ItemId.BerserkerNecklace, 30, 15);
            yield return Row(ItemId.PaladinNecklace, 50, 10);
            yield return Row(ItemId.HolyBible, 20, 10);
            yield return Row(ItemId.SacredCrucifix, 20, 10);
            yield return Row(ItemId.RingOfEndurance, 1, 1);
            yield return Row(ItemId.BladeOfHeroes, 20, 10);
            yield return Row(ItemId.ShieldOfChampion, 20, 10);
            yield return Row(ItemId.FluteOfHunter, 20, 10);
            yield return Row(ItemId.EngravedFangs, 20, 10);
            yield return Row(ItemId.EnchantedPouch, 10, 10);
            yield return Row(ItemId.SealOfWisdom, 10, 10);
            yield return Row(ItemId.ProspectorKey, 4, 2);
            yield return Row(ItemId.HawkEye, 40, 20);
            yield return Row(ItemId.FidelityCard, 10, 10);
            yield return Row(ItemId.TrollMushroom, 125, 25);
            yield return Row(ItemId.OldGiantWoodenClub, 3, 1);
            yield return Row(ItemId.LuckyMillorLeftHand, 10, 5);
            yield return Row(ItemId.GrolMokbarRing, 5, 5);
            yield return Row(ItemId.TalismanOfPeace, 4, 4);
            yield return Row(ItemId.SealOfLivingFlesh, 2, 2);
            yield return Row(ItemId.StopWatch, 15, -1);
            yield return Row(ItemId.SauerkrautEffigy, 20, 10);
            yield return Row(ItemId.ButchersThornChaplet, 20, 10);
            yield return Row(ItemId.NordheimWatcherLantern, 20, 10);
            yield return Row(ItemId.ArbalestOfTheKingsValley, 20, 10);
            yield return Row(ItemId.LightningAmulet, 20, 10);
            yield return Row(ItemId.HolyWater, 20, 10);
        }

        private static object[] Row(ItemId id, int value, int inc) => new object[] { id, value, inc };
    }
}
