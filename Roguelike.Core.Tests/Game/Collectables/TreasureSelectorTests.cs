using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Roguelike.Core.Configuration;
using Roguelike.Core.Game.Characters.Enemies;
using Roguelike.Core.Game.Characters.Players;
using Roguelike.Core.Game.Collectables;
using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Game.Levels;
using Roguelike.Core.Tests.Fakes;

namespace Roguelike.Core.Tests.Game.Collectables;

[TestClass]
public class TreasureSelectorTests
{
    [TestMethod]
    public void BuildOptionViews_Item_UsesNextRarityAndFormatsDescription()
    {
        var baseItem = ItemFactory.CreateItem(ItemId.HolyWater);
        var existing = new Item
        {
            Id = baseItem.Id,
            Name = baseItem.Name,
            Effect = baseItem.Effect,
            Value = baseItem.Value,
            UpgradableIncrementValue = baseItem.UpgradableIncrementValue,
            Rarity = ItemRarity.Common
        };

        var p = NewPlayer(items: new[] { existing });
        var choices = new[]
        {
            new Treasure { Type = BonusType.Item, Value = (int)ItemId.HolyWater }
        };

        var views = TreasureSelector.BuildOptionViews(choices, p);

        Assert.AreEqual(1, views.Count);
        Assert.AreEqual(BonusType.Item, views[0].Treasure.Type);
        // When item already owned & upgradable, rarity shown to user is next step:
        Assert.AreEqual(ItemRarity.Uncommon, views[0].Rarity);
        // Don’t pin to exact i18n text; just ensure it mentions the item name.
        StringAssert.Contains(views[0].Description, baseItem.Name);
    }

    [TestMethod]
    public void BuildOptionViews_VisionAndStats_MapToExpectedRarities()
    {
        var p = NewPlayer(maxLife: 200);

        var choices = new[]
        {
            new Treasure { Type = BonusType.Vision, Value = 1 }, // Common
            new Treasure { Type = BonusType.Vision, Value = 2 }, // Uncommon
            new Treasure { Type = BonusType.Vision, Value = 3 }, // Rare
            new Treasure { Type = BonusType.Strength, Value = 1 }, // Broken
            new Treasure { Type = BonusType.Speed, Value = 2 },    // Common
            new Treasure { Type = BonusType.Armor, Value = 4 },    // Rare
        };

        var views = TreasureSelector.BuildOptionViews(choices, p);

        Assert.AreEqual(ItemRarity.Common, views[0].Rarity);
        Assert.AreEqual(ItemRarity.Uncommon, views[1].Rarity);
        Assert.AreEqual(ItemRarity.Rare, views[2].Rarity);
        Assert.AreEqual(ItemRarity.Broken, views[3].Rarity);
        Assert.AreEqual(ItemRarity.Common, views[4].Rarity);
        Assert.AreEqual(ItemRarity.Rare, views[5].Rarity);
    }

    [TestMethod]
    public void BuildOptionViews_MaxHp_RarityBasedOnRatio()
    {
        var p = NewPlayer(maxLife: 100);
        var choices = new[]
        {
            new Treasure { Type = BonusType.MaxLifePoint, Value = 4 },
            new Treasure { Type = BonusType.MaxLifePoint, Value = 7 },
            new Treasure { Type = BonusType.MaxLifePoint, Value = 14 },
            new Treasure { Type = BonusType.MaxLifePoint, Value = 17 },
            new Treasure { Type = BonusType.MaxLifePoint, Value = 20 },
        };

        var views = TreasureSelector.BuildOptionViews(choices, p)
            .Select(v => v.Rarity).ToList();

        CollectionAssert.AreEqual(
            new List<ItemRarity> {
                ItemRarity.Broken, ItemRarity.Common, ItemRarity.Uncommon, ItemRarity.Rare, ItemRarity.Epic
            }, views);
    }


    [TestMethod]
    [DoNotParallelize]
    public void GenerateBonusChoices_AddsDominantStatFocus_AndRespectsFilters()
    {
        // Dominant Strength (>10 and greater than others) should force a stat focus entry.
        var p = NewPlayer(life: 20, maxLife: 100, strength: 30, armor: 5, speed: 4, vision: 9, steps: 0);
        var settings = new GameSettings();

        // Ensure the static item pool is reset before the test loop to avoid concurrent modification issues.
        var poolField = typeof(TreasureSelector).GetField("_selectedItemPool", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.IsNotNull(poolField, "Expected TreasureSelector to expose _selectedItemPool field.");
        poolField.SetValue(null, new List<ItemId>());

        int trials = 100;
        bool hasStrength = false;

        for (int i = 0; i < trials || !hasStrength; i++)
        {
            var choices = TreasureSelector.GenerateBonusChoices(p, settings);
            Assert.IsTrue(choices.Count > 0, "Should produce at least one choice.");
            Assert.IsTrue(choices.Count <= 3);
            hasStrength = choices.Any(t => t.Type == BonusType.Strength);
        }

        Assert.IsTrue(hasStrength, "Dominant Strength should lead to a Strength-focused bonus being added.");
    }


    [TestMethod]
    public void EnsureItemPoolInitialization_ExcludesEnemySpecificItems()
    {
        var forcedEnemyTypes = new[] { EnemyType.Undead, EnemyType.Wild, EnemyType.Outlaws, EnemyType.Cultist };
        var originalEnemyTypes = LevelManager.EnemyTypes.ToList();

        var poolField = typeof(TreasureSelector).GetField("_selectedItemPool", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.IsNotNull(poolField, "Expected TreasureSelector to expose _selectedItemPool field.");

        var originalPoolReference = (List<ItemId>)poolField!.GetValue(null)!;
        var originalPoolSnapshot = originalPoolReference.ToList();

        var ensureMethod = typeof(TreasureSelector).GetMethod("EnsureItemPoolInitialized", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.IsNotNull(ensureMethod, "EnsureItemPoolInitialized should exist.");

        try
        {
            LevelManager.EnemyTypes.Clear();
            LevelManager.EnemyTypes.AddRange(forcedEnemyTypes);

            var excludedItems = forcedEnemyTypes
                .SelectMany(ItemIdHelper.GetItemIdsSpecificByEnemyType)
                .ToHashSet();

            Assert.IsTrue(excludedItems.Count > 0, "Test setup should exclude at least one item.");

            for (int attempt = 0; attempt < 5; attempt++)
            {
                poolField.SetValue(null, new List<ItemId>());
                ensureMethod!.Invoke(null, null);

                var currentPool = (List<ItemId>)poolField.GetValue(null)!;
                var forbidden = currentPool.Where(excludedItems.Contains).ToList();

                Assert.IsTrue(currentPool.Count > 0, "Item pool should contain entries after initialization.");
                Assert.AreEqual(0, forbidden.Count,
                    $"Item pool should exclude enemy-specific items. Found forbidden: {string.Join(", ", forbidden)}");
            }
        }
        finally
        {
            LevelManager.EnemyTypes.Clear();
            LevelManager.EnemyTypes.AddRange(originalEnemyTypes);

            originalPoolReference.Clear();
            originalPoolReference.AddRange(originalPoolSnapshot);
            poolField.SetValue(null, originalPoolReference);
        }
    }

    [TestMethod]
    public void ApplyBonus_Item_New_AddsToInventory_AndMayAffectPlayerStats()
    {
        var p = NewPlayer(vision: 0, items: Array.Empty<Item>());
        var settings = new GameSettings();

        // Pick an item with a special stat side-effect (GlassesOfClairvoyance sets Vision to item.Value if higher)
        var itemId = ItemId.GlassesOfClairvoyance;
        var msg = TreasureSelector.ApplyBonus(
            new Treasure { Type = BonusType.Item, Value = (int)itemId }, p, settings, ui: new FakeInventoryUI(0));

        Assert.IsTrue(p.Inventory.Any(i => i.Id == itemId));
        // Vision should be at least the item's value after picking it.
        var picked = p.Inventory.First(i => i.Id == itemId);
        Assert.IsTrue(p.Vision >= picked.Value);
        // Message contains item name (don’t assert full i18n)
        StringAssert.Contains(msg, picked.Name);
    }

    [TestMethod]
    public void ApplyBonus_Item_ExistingUpgradable_IncreasesValueAndRarity()
    {
        var baseItem = ItemFactory.CreateItem(ItemId.EnchantedPouch);
        var existing = new Item
        {
            Id = baseItem.Id,
            Name = baseItem.Name,
            Effect = baseItem.Effect,
            Value = baseItem.Value,
            UpgradableIncrementValue = baseItem.UpgradableIncrementValue, // > 0 (upgradable)
            Rarity = ItemRarity.Common
        };

        var p = NewPlayer(items: new[] { existing });
        var settings = new GameSettings();

        var msg = TreasureSelector.ApplyBonus(
            new Treasure { Type = BonusType.Item, Value = (int)existing.Id }, p, settings, ui: new FakeInventoryUI(0));

        var upgraded = p.Inventory.First(i => i.Id == existing.Id);
        Assert.AreEqual(baseItem.Value + baseItem.UpgradableIncrementValue, upgraded.Value);
        Assert.AreEqual(ItemRarity.Common + 1, upgraded.Rarity);
        StringAssert.Contains(msg, baseItem.Name);
    }

    [TestMethod]
    public void ApplyBonus_Item_ExistingNotUpgradable_ReturnsAlreadyOwnedMessage()
    {
        var baseItem = ItemFactory.CreateItem(ItemId.TalismanOfTheLastBreath);
        Assert.AreEqual(0, baseItem.UpgradableIncrementValue, "Test expects a non-upgradable item.");

        var existing = new Item
        {
            Id = baseItem.Id,
            Name = baseItem.Name,
            Effect = baseItem.Effect,
            Value = baseItem.Value,
            UpgradableIncrementValue = 0,
            Rarity = baseItem.Rarity
        };

        var p = NewPlayer(items: new[] { existing });
        var settings = new GameSettings();

        var msg = TreasureSelector.ApplyBonus(
            new Treasure { Type = BonusType.Item, Value = (int)existing.Id }, p, settings, ui: new FakeInventoryUI(0));

        // Inventory unchanged, message indicates already owned non-upgradable
        Assert.AreEqual(1, p.Inventory.Count);
        StringAssert.Contains(msg, baseItem.Name);
    }

    [TestMethod]
    public void ApplyBonus_Item_FullInventory_DropReplacesItem()
    {
        var settings = new GameSettings();

        // 3 items already (full)
        var i1 = ItemFactory.CreateItem(ItemId.HolyBible);
        var i2 = ItemFactory.CreateItem(ItemId.SacredCrucifix);
        var i3 = ItemFactory.CreateItem(ItemId.RingOfEndurance);
        var p = NewPlayer(items: new[] { i1, i2, i3 });

        var incoming = ItemId.BootsOfEchoStep;

        // UI chooses to drop index 0
        var msg = TreasureSelector.ApplyBonus(
            new Treasure { Type = BonusType.Item, Value = (int)incoming }, p, settings, ui: new FakeInventoryUI(0));

        Assert.AreEqual(3, p.Inventory.Count);
        Assert.IsFalse(p.Inventory.Any(i => i.Id == i1.Id), "Index 0 should be dropped.");
        Assert.IsTrue(p.Inventory.Any(i => i.Id == incoming), "New item should be added.");
    }

    [TestMethod]
    public void ApplyBonus_Item_FullInventory_KeepCurrent()
    {
        var settings = new GameSettings();

        var i1 = ItemFactory.CreateItem(ItemId.HolyBible);
        var i2 = ItemFactory.CreateItem(ItemId.SacredCrucifix);
        var i3 = ItemFactory.CreateItem(ItemId.RingOfEndurance);
        var p = NewPlayer(items: new[] { i1, i2, i3 });

        var incoming = ItemId.HawkEye;

        // UI returns an out-of-range drop index -> keep current inventory
        var msg = TreasureSelector.ApplyBonus(
            new Treasure { Type = BonusType.Item, Value = (int)incoming }, p, settings, ui: new FakeInventoryUI(dropIndex: 99));

        Assert.AreEqual(3, p.Inventory.Count);
        Assert.IsFalse(p.Inventory.Any(i => i.Id == incoming));
    }

    private static Player NewPlayer(
        int life = 50, int maxLife = 100, int strength = 1, int armor = 1, int speed = 1, int vision = 0, int steps = 0,
        IEnumerable<Item>? items = null)
    {
        return new Player
        {
            LifePoint = life,
            MaxLifePoint = maxLife,
            Strength = strength,
            Armor = armor,
            Speed = speed,
            Vision = vision,
            Steps = steps,
            Inventory = items?.ToList() ?? new List<Item>()
        };
    }
}
