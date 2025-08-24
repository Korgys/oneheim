namespace Roguelike.Console.Game.Collectables;

using Roguelike.Console.Configuration;
using Roguelike.Console.Game.Characters.Players;
using Roguelike.Console.Game.Collectables.Items;
using Roguelike.Console.Game.Rendering;
using Roguelike.Console.Properties.i18n;
using System;

public static class TreasureSelector
{
    private static List<ItemId> _selectedItemPool = new();
    private static readonly Random _random = new();
    private static readonly int _numberOfItemInPool = 9;

    public static List<Treasure> GenerateBonusChoices(Player player, GameSettings settings)
    {
        EnsureItemPoolInitialized();

        var types = Enum.GetValues<BonusType>().ToList();

        // Avoid life refill if HP ratio >= 50%
        double hpRatio = player.MaxLifePoint > 0 ? (double)player.LifePoint / player.MaxLifePoint : 0;
        if (hpRatio >= 0.5) types.Remove(BonusType.LifePoint);

        // Stop offering vision if already high (>= 9)
        if (player.Vision >= 9) types.Remove(BonusType.Vision);

        var result = new List<Treasure>();
        var usedNonItemTypes = new HashSet<BonusType>();

        // Optional stat focus (50%) if a stat is strictly dominant and > 10
        if (_random.NextDouble() <= 0.5) TryAddStatFocus(player, result, usedNonItemTypes);

        // Fill remaining up to 3
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

        // Safety top-up
        while (result.Count < 3)
            result.Add(new Treasure { Type = BonusType.MaxLifePoint, Value = Math.Max(2, player.MaxLifePoint / 12) });

        return result;
    }

    private static void TryAddStatFocus(Player player, List<Treasure> result, HashSet<BonusType> usedNonItemTypes)
    {
        bool armorDom = player.Armor > player.Speed && player.Armor > player.Strength && player.Armor > 10;
        bool strDom = player.Strength > player.Speed && player.Strength > player.Armor && player.Strength > 10;
        bool spdDom = player.Speed > player.Strength && player.Speed > player.Armor && player.Speed > 10;

        if (armorDom)
        {
            result.Add(new Treasure { Type = BonusType.Armor, Value = GenerateValueForBonus(BonusType.Armor, player) });
            usedNonItemTypes.Add(BonusType.Armor);
        }
        else if (strDom)
        {
            result.Add(new Treasure { Type = BonusType.Strength, Value = GenerateValueForBonus(BonusType.Strength, player) });
            usedNonItemTypes.Add(BonusType.Strength);
        }
        else if (spdDom)
        {
            result.Add(new Treasure { Type = BonusType.Speed, Value = GenerateValueForBonus(BonusType.Speed, player) });
            usedNonItemTypes.Add(BonusType.Speed);
        }
    }

    private static int GenerateValueForBonus(BonusType type, Player player, List<Treasure>? currentSelection = null)
    {
        switch (type)
        {
            case BonusType.Item:
                var validItems = _selectedItemPool
                    .Where(itemId =>
                    {
                        var existing = player.Inventory.FirstOrDefault(i => i.Id == itemId);
                        if (existing != null)
                        {
                            if (existing.UpgradableIncrementValue == 0) return false;            // non-upgradable owned
                            if (existing.Rarity >= ItemRarity.Legendary) return false;          // maxed rarity
                        }
                        if (currentSelection?.Any(t => t.Type == BonusType.Item && (ItemId)t.Value == itemId) == true)
                            return false; // avoid duplicates in the same choice set
                        return true;
                    })
                    .ToList();

                if (validItems.Count == 0) return -1; // no valid item
                return (int)validItems[_random.Next(validItems.Count)];

            case BonusType.Vision:
                // Vision is powerful; keep tight bounds. Base = +1 (Common).
                // HawkEye synergy: chance to roll higher (+2 or +3).
                int vision = 1;
                var hawkEye = player.Inventory.FirstOrDefault(i => i.Id == ItemId.HawkEye);
                if (hawkEye != null && _random.NextDouble() <= (float)hawkEye.Value / 100.0)
                {
                    vision = _random.NextDouble() <= 0.10 ? 3 : 2; // 10% chance for +3, else +2
                }
                return vision;

            case BonusType.LifePoint:
                // Heal ~70% of missing HP (as before).
                return (int)((player.MaxLifePoint - player.LifePoint) * 0.7);

            case BonusType.MaxLifePoint:
                // Calibrate Max HP gain by progression: make it meaningful but not explosive.
                // Use tiers by Steps and clamp by a soft percent of current Max HP.
                int steps = Math.Max(0, player.Steps);
                int baseMin, baseMax;
                if (steps < 200) { baseMin = 2; baseMax = 4; }
                else if (steps < 500) { baseMin = 3; baseMax = 6; }
                else if (steps < 800) { baseMin = 4; baseMax = 7; }
                else { baseMin = 5; baseMax = 8; }

                // Encourage build based on max life points
                int basePlayer = Math.Max(1, player.LifePoint / 16);
                baseMin += basePlayer;
                baseMax += basePlayer;

                int roll = _random.Next(baseMin, baseMax + 1);
                // Soft cap by 20% of current max HP per pickup (prevents absurd spikes)
                int cap = Math.Max(3, (int)Math.Round(player.MaxLifePoint * 0.20));
                return Math.Min(roll, cap);

            default: // Strength, Armor, Speed
                // LuckyMillorLeftHand item logic
                int bonus = 0;
                var luckyMillorLeftHand = player.Inventory.FirstOrDefault(i => i.Id == ItemId.LuckyMillorLeftHand);
                if (luckyMillorLeftHand != null && _random.NextDouble() <= (double)(luckyMillorLeftHand.Value / 100m)) bonus = 1;
                // Scale small stat bumps with progression, but keep them modest.
                // Early: +1~2, Mid: +2~3, Late: +3~4
                int s;
                if (player.Steps < 200) s = _random.Next(1, 3);
                else if (player.Steps < 600) s = _random.Next(2, 4);
                else s = _random.Next(3, 5);
                return s + bonus;
        }
    }

    private static void EnsureItemPoolInitialized()
    {
        if (_selectedItemPool.Count == 0)
        {
            var all = Enum.GetValues<ItemId>().ToList();
            _selectedItemPool = all.OrderBy(_ => _random.Next()).Take(_numberOfItemInPool).ToList();
        }
    }

    private static ItemRarity GetRarityForBonus(BonusType type, int value, Player player, ItemRarity? itemRarityFromItem)
    {
        // Items: use their own rarity (existing or base)
        if (type == BonusType.Item && itemRarityFromItem.HasValue)
            return itemRarityFromItem.Value;

        switch (type)
        {
            case BonusType.Vision:
                return value switch
                {
                    <= 1 => ItemRarity.Common,
                    2 => ItemRarity.Uncommon,
                    3 => ItemRarity.Rare,
                    _ => ItemRarity.Epic
                };

            case BonusType.MaxLifePoint:
                // Rate by *relative gain* (% of current Max HP)
                // <5% → Common, <10% → Uncommon, <15% → Rare, <20% → Epic, else Legendary
                double pct = player.MaxLifePoint > 0 ? (double)value / player.MaxLifePoint : 0;
                if (pct < 0.05) return ItemRarity.Broken;
                if (pct < 0.10) return ItemRarity.Common;
                if (pct < 0.15) return ItemRarity.Uncommon;
                if (pct < 0.20) return ItemRarity.Rare;
                return ItemRarity.Epic;

            case BonusType.LifePoint:
                // Rate by % of missing HP healed (bigger refills feel rarer)
                int missing = Math.Max(0, player.MaxLifePoint - player.LifePoint);
                double healPct = missing > 0 ? (double)Math.Min(value, missing) / missing : 0;
                if (healPct < 0.25) return ItemRarity.Broken;
                if (healPct < 0.50) return ItemRarity.Common;
                if (healPct < 0.75) return ItemRarity.Uncommon;
                if (healPct < 0.95) return ItemRarity.Rare;
                return ItemRarity.Epic;

            default:
                // Strength/Armor/Speed: rate by absolute bump with light scaling by current stat
                // Very small (+1) → Common, +2 → Uncommon, +3 → Rare, +4 → Epic, >=5 → Legendary
                return value switch
                {
                    <= 1 => ItemRarity.Broken,
                    2 => ItemRarity.Common,
                    3 => ItemRarity.Uncommon,
                    4 => ItemRarity.Rare,
                    5 => ItemRarity.Epic,
                    _ => ItemRarity.Legendary
                };
        }
    }

    public static Treasure PromptPlayerForBonus(List<Treasure> choices, Player player, GameSettings settings)
    {
        Console.Clear();
        PlayerRenderer.RendererPlayerStats(player);
        Console.WriteLine();
        PlayerRenderer.RenderPlayerInventory(player);

        Console.WriteLine();
        Console.WriteLine(Messages.YouFoundATreasureChooseABonus);

        var keys = new List<string> { settings.Controls.Choice1, settings.Controls.Choice2, settings.Controls.Choice3 };

        for (int i = 0; i < choices.Count; i++)
        {
            var b = choices[i];

            // Build description + deduce rarity
            ItemRarity? itemRarity = null;
            string desc = b.Type == BonusType.Item
                ? FormatItemDescription((ItemId)b.Value, player, out itemRarity)
                : $"+{b.Value} {Messages.ResourceManager.GetString(b.Type.ToString()) ?? b.Type.ToString()}";

            var rarity = GetRarityForBonus(b.Type, b.Value, player, itemRarity);

            ItemManager.WriteColored($"{keys[i]}. {desc}\n", rarity);
        }

        int chosen = -1;
        while (chosen == -1)
        {
            var key = Console.ReadKey(true).Key.ToString().ToUpperInvariant();
            chosen = keys.FindIndex(k => k.ToUpperInvariant() == key);
        }

        return choices[chosen];
    }

    private static string FormatItemDescription(ItemId itemId, Player player, out ItemRarity? itemRarity)
    {
        var item = ItemFactory.CreateItem(itemId);
        var existing = player.Inventory.FirstOrDefault(i => i.Id == itemId);

        int displayValue = existing != null
            ? existing.Value + existing.UpgradableIncrementValue
            : item.Value;

        itemRarity = existing?.Rarity != null ? existing.Rarity + 1 : item.Rarity;

        return $"{item.Name} : {item.GetEffectDescription(displayValue)}";
    }

    public static string ApplyBonus(Treasure bonus, Player player, GameSettings settings)
    {
        return bonus.Type switch
        {
            BonusType.LifePoint => ApplyLifePointBonus(player, bonus.Value),
            BonusType.MaxLifePoint => ApplyMaxLifePointBonus(player, bonus.Value),
            BonusType.Strength => ApplyStatBonus(player, nameof(player.Strength), bonus.Value),
            BonusType.Armor => ApplyStatBonus(player, nameof(player.Armor), bonus.Value),
            BonusType.Speed => ApplyStatBonus(player, nameof(player.Speed), bonus.Value),
            BonusType.Vision => ApplyStatBonus(player, nameof(player.Vision), bonus.Value),
            BonusType.Item => ApplyItemBonus(player, settings, (ItemId)bonus.Value),
            _ => "Unknown bonus type"
        };
    }

    private static string ApplyLifePointBonus(Player player, int value)
    {
        int healed = Math.Min(player.MaxLifePoint - player.LifePoint, value);
        player.LifePoint += healed;
        return $"+{healed} {Messages.HP}";
    }

    private static string ApplyMaxLifePointBonus(Player player, int value)
    {
        player.MaxLifePoint += value;

        var gameDifficulty = ConfigurationReader.LoadGameSettings().Difficulty;
        if (gameDifficulty != Difficulty.Hell)
            player.LifePoint += value;

        return $"+{value} {Messages.MaxHp}";
    }

    private static string ApplyStatBonus(Player player, string statName, int value)
    {
        switch (statName)
        {
            case nameof(player.Strength): player.Strength += value; break;
            case nameof(player.Armor): player.Armor += value; break;
            case nameof(player.Speed): player.Speed += value; break;
            case nameof(player.Vision): player.Vision += value; break;
        }

        string label = Messages.ResourceManager.GetString(statName) ?? statName; // Translate stat name
        return $"+{value} {label}";
    }

    private static string ApplyItemBonus(Player player, GameSettings settings, ItemId itemId)
    {
        var item = ItemFactory.CreateItem(itemId);
        return InventoryManager.TryAddItem(player, item, settings);
    }

    public static void ResetItemPool() => _selectedItemPool.Clear();
}
