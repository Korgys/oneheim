namespace Roguelike.Core.Game.Collectables;

using Roguelike.Core.Configuration;
using Roguelike.Core.Utils;
using Roguelike.Core.Game.Abstractions;
using Roguelike.Core.Game.Characters.Players;
using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Game.Levels;
using Roguelike.Core.Properties.i18n;

public static class TreasureSelector
{
    private static List<ItemId> _selectedItemPool = new();
    private static readonly Random _random = new();
    private const int _numberOfItemInPool = 9;

    public static List<Treasure> GenerateBonusChoices(Player player, GameSettings settings)
    {
        EnsureItemPoolInitialized();

        var types = Enum.GetValues<BonusType>().ToList();

        // Avoid life refill if HP ratio >= 50%
        double hpRatio = player.MaxLifePoint > 0 ? (double)player.LifePoint / player.MaxLifePoint : 0;
        if (hpRatio >= 0.5) types.Remove(BonusType.LifePoint);

        // Stop offering vision when already high (9 max)
        if (_random.Next(11) <= player.Vision) types.Remove(BonusType.Vision);

        var result = new List<Treasure>();
        var usedNonItemTypes = new HashSet<BonusType>();

        // Optional stat focus (50%) if a stat dominates (>10)
        if (_random.NextDouble() <= 0.5)
            types.Remove(TryAddStatFocus(player, result));

        // Fill remaining slots up to 3
        int safety = 50;
        while (result.Count < 3 && safety-- > 0)
        {
            var available = types.Where(t => !usedNonItemTypes.Contains(t)).ToList();
            if (available.Count == 0) break;

            var type = available[_random.Next(available.Count)];
            if (type != BonusType.Item) usedNonItemTypes.Add(type);

            int value = GenerateValueForBonus(type, player, result);
            result.Add(new Treasure { Type = type, Value = value });
        }

        return result;
    }

    /// <summary>
    /// Build UI-ready option models (description + rarity) from raw treasures.
    /// </summary>
    public static List<TreasureOptionView> BuildOptionViews(
        IReadOnlyList<Treasure> choices, Player player)
    {
        var list = new List<TreasureOptionView>(choices.Count);
        foreach (var t in choices)
        {
            ItemRarity? itemRarity = null;
            string desc = t.Type == BonusType.Item
                ? FormatItemDescription((ItemId)t.Value, player, out itemRarity)
                : $"+{t.Value} {Messages.ResourceManager.GetString(t.Type.ToString()) ?? t.Type.ToString()}";

            var rarity = GetRarityForBonus(t.Type, t.Value, player, itemRarity);
            list.Add(new TreasureOptionView
            {
                Treasure = t,
                Description = desc,
                Rarity = rarity
            });
        }
        return list;
    }

    /// <summary>
    /// UI-agnostic "choose" flow: use an injected picker to select a treasure.
    /// </summary>
    public static Treasure ChooseWithPicker(
        Player player,
        GameSettings settings,
        ITreasurePicker picker)
    {
        var choices = GenerateBonusChoices(player, settings);
        var views = BuildOptionViews(choices, player);

        var ctx = new TreasurePickerContext
        {
            Player = player,
            Title = Messages.YouFoundATreasureChooseABonus
        };

        int idx = picker.Pick(ctx, views);
        if (idx < 0 || idx >= choices.Count) idx = 0;
        return choices[idx];
    }

    public static string ApplyBonus(Treasure bonus, Player player, GameSettings settings, IInventoryUI ui) =>
        bonus.Type switch
        {
            BonusType.LifePoint => ApplyLifePointBonus(player, bonus.Value),
            BonusType.MaxLifePoint => ApplyMaxLifePointBonus(player, bonus.Value),
            BonusType.Strength => ApplyStatBonus(player, nameof(player.Strength), bonus.Value),
            BonusType.Armor => ApplyStatBonus(player, nameof(player.Armor), bonus.Value),
            BonusType.Speed => ApplyStatBonus(player, nameof(player.Speed), bonus.Value),
            BonusType.Vision => ApplyStatBonus(player, nameof(player.Vision), bonus.Value),
            BonusType.Item => ApplyItemBonus(player, settings, (ItemId)bonus.Value, ui),
            _ => "Unknown bonus type"
        };

    /// <summary>
    /// If a stat is clearly dominating (>10 and > other stats), offer it as a bonus.
    /// </summary>
    /// <param name="p"></param>
    /// <param name="res"></param>
    /// <returns></returns>
    private static BonusType TryAddStatFocus(Player p, List<Treasure> res)
    {
        bool strDom = p.Strength > p.Speed && p.Strength > p.Armor && p.Strength > 10;
        bool spdDom = p.Speed > p.Strength && p.Speed > p.Armor && p.Speed > 10;

        if (strDom)
        {
            res.Add(new Treasure { Type = BonusType.Strength, Value = GenerateValueForBonus(BonusType.Strength, p) });
            return BonusType.Strength;
        }
        else if (spdDom)
        {
            res.Add(new Treasure { Type = BonusType.Speed, Value = GenerateValueForBonus(BonusType.Speed, p) });
            return BonusType.Speed;
        }
        else // by default, offer Armor
        {
            res.Add(new Treasure { Type = BonusType.Armor, Value = GenerateValueForBonus(BonusType.Armor, p) });
            return BonusType.Armor;
        }
    }

    private static int GenerateValueForBonus(BonusType type, Player player, List<Treasure>? currentSelection = null)
    {
        switch (type)
        {
            case BonusType.Item:
                {
                    var valid = _selectedItemPool.Where(id =>
                    {
                        var existing = player.Inventory.FirstOrDefault(i => i.Id == id);
                        if (existing != null)
                        {
                            if (existing.UpgradableIncrementValue == 0) return false;   // owned & non-upgradable
                            if (existing.Rarity >= ItemRarity.Legendary) return false; // maxed rarity
                        }
                        if (currentSelection?.Any(t => t.Type == BonusType.Item && (ItemId)t.Value == id) == true)
                            return false; // avoid duplicates among the 3 choices
                        return true;
                    }).ToList();

                    if (valid.Count == 0) return -1;
                    return (int)valid[_random.Next(valid.Count)];
                }

            case BonusType.Vision:
                {
                    int v = 1; // base +1 (Common)
                    var hawkEye = player.Inventory.FirstOrDefault(i => i.Id == ItemId.HawkEye);
                    if (hawkEye != null && _random.NextDouble() <= hawkEye.Value / 100.0)
                        v = _random.NextDouble() <= 0.10 ? 3 : 2; // 10%: +3 else +2
                    return v;
                }

            case BonusType.LifePoint:
                return (int)((player.MaxLifePoint - player.LifePoint) * 0.7); // Offer 70% of missing HP

            case BonusType.MaxLifePoint:
                {
                    decimal x = _random.Next(8, 12); // 8-11%
                    int roll = Math.Max(2, (int)Math.Round(player.MaxLifePoint / x)); // up to ~12.5% of max HP
                    int minCap = Math.Max(50, player.Steps) / 50; // at least 1, up to 30
                    return Math.Max(minCap, roll);
                }

            default: // Strength / Armor / Speed
                {
                    int s = player.Steps switch
                    {
                        < 200  => _random.NextWeighted(new Dictionary<int, int> { { 1, 2 }, { 2, 1 } }),           // 1-1-2
                        < 400  => _random.Next(1, 3),                                                              // 1-2 (not weighted)
                        < 600  => _random.NextWeighted(new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 1 } }), // 1-2-2-3
                        < 800  => _random.Next(2, 4),                                                              // 2-3
                        < 1200 => _random.Next(2, 5),                                                              // 2-3-4
                        _      => _random.Next(3, 6)                                                               // 3-4
                    };

                    // LuckyMillorLeftHand: +1 proc
                    var lucky = player.Inventory.FirstOrDefault(i => i.Id == ItemId.LuckyMillorLeftHand);
                    if (lucky != null && _random.NextDouble() <= (double)lucky.Value / 100.0) s += 1;
                    return s;
                }
        }
    }

    /// <summary>
    /// Ensures that the item pool is initialized with a randomized selection of items.
    /// </summary>
    /// <remarks>This method populates the item pool with a subset of items, excluding those associated with
    /// the current enemy types. The selection is randomized and limited to a predefined number of items.</remarks>
    private static void EnsureItemPoolInitialized()
    {
        if (_selectedItemPool.Count == 0)
        {
            var all = Enum.GetValues<ItemId>().ToList();

            // Filter on enemy types
            var excludedItems = new List<ItemId>();
            foreach (var enemyType in LevelManager.EnemyTypes)
                excludedItems.AddRange(ItemIdHelper.GetItemIdsSpecificByEnemyType(enemyType));

            var filteredItems = all.Except(excludedItems).ToList();
            _selectedItemPool = all.OrderBy(_ => _random.Next()).Take(_numberOfItemInPool).ToList();
        }
    }

    private static string FormatItemDescription(ItemId itemId, Player player, out ItemRarity? itemRarity)
    {
        var item = ItemFactory.CreateItem(itemId);
        var existing = player.Inventory.FirstOrDefault(i => i.Id == itemId);

        int displayValue = existing != null
            ? existing.Value + existing.UpgradableIncrementValue
            : item.Value;

        itemRarity = existing != null ? existing.Rarity + 1 : item.Rarity;
        return $"{item.Name} : {item.GetEffectDescription(displayValue)}";
    }

    private static ItemRarity GetRarityForBonus(BonusType type, int value, Player player, ItemRarity? fromItem)
    {
        if (type == BonusType.Item && fromItem.HasValue) return fromItem.Value;

        return type switch
        {
            BonusType.Vision => value switch
            {
                <= 1 => ItemRarity.Common,
                2 => ItemRarity.Uncommon,
                3 => ItemRarity.Rare,
                _ => ItemRarity.Epic
            },

            BonusType.MaxLifePoint => MapByRatio(value, player.MaxLifePoint),

            BonusType.LifePoint => MapHealRarity(value, player),

            _ => value switch // STR / ARM / SPD
            {
                <= 1 => ItemRarity.Broken,
                2 => ItemRarity.Common,
                3 => ItemRarity.Uncommon,
                4 => ItemRarity.Rare,
                5 => ItemRarity.Epic,
                _ => ItemRarity.Legendary
            }
        };

        static ItemRarity MapByRatio(int inc, int maxHp)
        {
            double pct = maxHp > 0 ? (double)inc / maxHp : 0;
            if (pct < 0.05) return ItemRarity.Broken;
            if (pct < 0.10) return ItemRarity.Common;
            if (pct < 0.15) return ItemRarity.Uncommon;
            if (pct < 0.20) return ItemRarity.Rare;
            return ItemRarity.Epic;
        }

        static ItemRarity MapHealRarity(int amount, Player p)
        {
            double ratio = p.MaxLifePoint > 0 ? (p.LifePoint * 100) / p.MaxLifePoint : 0;

            if (ratio > 0.25) return ItemRarity.Broken;
            if (ratio > 0.16) return ItemRarity.Common;
            if (ratio > 0.08) return ItemRarity.Uncommon;
            if (ratio > 0.04) return ItemRarity.Rare;
            if (ratio > 0.02) return ItemRarity.Epic;
            return ItemRarity.Legendary;
        }
    }

    private static string ApplyLifePointBonus(Player p, int amount)
    {
        int healed = Math.Min(p.MaxLifePoint - p.LifePoint, amount);
        p.LifePoint += healed;
        return $"+{healed} {Messages.HP}";
    }

    private static string ApplyMaxLifePointBonus(Player p, int amount)
    {
        p.MaxLifePoint += amount;

        var diff = ConfigurationReader.LoadGameSettings().Difficulty;
        if (diff != Difficulty.Hell)
            p.LifePoint += amount;

        return $"+{amount} {Messages.MaxHp}";
    }

    private static string ApplyStatBonus(Player p, string statName, int value)
    {
        switch (statName)
        {
            case nameof(p.Strength): p.Strength += value; break;
            case nameof(p.Armor): p.Armor += value; break;
            case nameof(p.Speed): p.Speed += value; break;
            case nameof(p.Vision): p.Vision += value; break;
        }

        string label = Messages.ResourceManager.GetString(statName) ?? statName;
        return $"+{value} {label}";
    }

    private static string ApplyItemBonus(Player p, GameSettings settings, ItemId itemId, IInventoryUI ui)
    {
        var item = ItemFactory.CreateItem(itemId);
        return TryAddItem(p, item, settings, ui);
    }

    private static string TryAddItem(Player player, Item item, GameSettings settings, IInventoryUI ui)
    {
        string message = string.Empty;
        var existing = player.Inventory.FirstOrDefault(i => i.Id == item.Id);

        // Existing item
        if (existing != null)
        {
            if (existing.UpgradableIncrementValue != 0)
            {
                existing.Value += existing.UpgradableIncrementValue;
                if (existing.Rarity < ItemRarity.Legendary) existing.Rarity++;

                // Sort items
                player.Inventory = [.. player.Inventory.OrderByDescending(i => i.Rarity)];

                // Check if the item is a special item that affects player stats
                if (existing.Id == ItemId.GlassesOfClairvoyance && player.Vision < existing.Value)
                {
                    player.Vision = existing.Value;
                }

                return string.Format(Messages.UpgradedItemTo, item.Name, existing.Value);
            }
            else
            {
                return string.Format(Messages.YouAlreadyOwnItemNotUpgradable, item.Name);
            }
        }

        // Sort items
        player.Inventory = [.. player.Inventory.OrderByDescending(i => i.Rarity)];

        // Check if the item is a special item that affects player stats
        if (item.Id == ItemId.GlassesOfClairvoyance && player.Vision < item.Value)
        {
            player.Vision = item.Value;
        }

        // Check if the player can carry more items
        if (player.Inventory.Count >= 3)
        {
            return HandleFullInventory(player, item, settings, ui);
        }

        // Add the item to the inventory
        player.Inventory.Add(item);
        return string.Format(Messages.FoundItem, item.Name, item.EffectDescription);
    }

    private static string HandleFullInventory(Player player, Item newItem, GameSettings settings, IInventoryUI ui)
    {
        var dropIndex = ui.PromptDropIndex(player, newItem, settings);

        if (dropIndex < player.Inventory.Count)
        {
            var dropped = player.Inventory[dropIndex];
            player.Inventory.RemoveAt(dropIndex);
            player.Inventory.Add(newItem);

            return $"{Messages.Dropped} {dropped.Name}, {Messages.Picked} {newItem.Name}.";
        }
        else
        {
            return Messages.KeepCurrentInventory;
        }
    }
}
