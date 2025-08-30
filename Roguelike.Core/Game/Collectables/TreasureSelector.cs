namespace Roguelike.Core.Game.Collectables;

using Roguelike.Core.Configuration;
using Roguelike.Core.Game.Characters.Players;
using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Properties.i18n;
using System;
using System.Linq;

public static class TreasureSelector
{
    private static List<ItemId> _selectedItemPool = new();
    private static readonly Random _random = new();
    private static readonly int _numberOfItemInPool = 9;

    // ---------- Generation ----------

    public static List<Treasure> GenerateBonusChoices(Player player, GameSettings settings)
    {
        EnsureItemPoolInitialized();

        var types = Enum.GetValues<BonusType>().ToList();

        // Avoid life refill if HP ratio >= 50%
        double hpRatio = player.MaxLifePoint > 0 ? (double)player.LifePoint / player.MaxLifePoint : 0;
        if (hpRatio >= 0.5) types.Remove(BonusType.LifePoint);

        // Stop offering vision when already high
        if (player.Vision >= 9) types.Remove(BonusType.Vision);

        var result = new List<Treasure>();
        var usedNonItemTypes = new HashSet<BonusType>();

        // Optional stat focus (50%) if a stat dominates (>10)
        if (_random.NextDouble() <= 0.5)
            TryAddStatFocus(player, result, usedNonItemTypes);

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

        // Safety top-up
        while (result.Count < 3)
            result.Add(new Treasure { Type = BonusType.MaxLifePoint, Value = Math.Max(2, player.MaxLifePoint / 12) });

        return result;
    }

    private static void TryAddStatFocus(Player p, List<Treasure> res, HashSet<BonusType> used)
    {
        bool armorDom = p.Armor > p.Speed && p.Armor > p.Strength && p.Armor > 10;
        bool strDom = p.Strength > p.Speed && p.Strength > p.Armor && p.Strength > 10;
        bool spdDom = p.Speed > p.Strength && p.Speed > p.Armor && p.Speed > 10;

        if (armorDom)
        {
            res.Add(new Treasure { Type = BonusType.Armor, Value = GenerateValueForBonus(BonusType.Armor, p) });
            used.Add(BonusType.Armor);
        }
        else if (strDom)
        {
            res.Add(new Treasure { Type = BonusType.Strength, Value = GenerateValueForBonus(BonusType.Strength, p) });
            used.Add(BonusType.Strength);
        }
        else if (spdDom)
        {
            res.Add(new Treasure { Type = BonusType.Speed, Value = GenerateValueForBonus(BonusType.Speed, p) });
            used.Add(BonusType.Speed);
        }
    }

    private static int GenerateValueForBonus(BonusType type, Player player, List<Treasure>? currentSelection = null)
    {
        switch (type)
        {
            case BonusType.Item:
                {
                    var validItems = _selectedItemPool
                        .Where(id =>
                        {
                            var existing = player.Inventory.FirstOrDefault(i => i.Id == id);
                            if (existing != null)
                            {
                                if (existing.UpgradableIncrementValue == 0) return false;           // owned & non-upgradable
                                if (existing.Rarity >= ItemRarity.Legendary) return false;         // maxed rarity
                            }
                            if (currentSelection?.Any(t => t.Type == BonusType.Item && (ItemId)t.Value == id) == true)
                                return false; // avoid duplicates among the 3 choices
                            return true;
                        })
                        .ToList();

                    if (validItems.Count == 0) return -1; // no valid item right now
                    return (int)validItems[_random.Next(validItems.Count)];
                }

            case BonusType.Vision:
                {
                    // Base +1 (Common). HawkEye synergy may upgrade roll.
                    int v = 1;
                    var hawkEye = player.Inventory.FirstOrDefault(i => i.Id == ItemId.HawkEye);
                    if (hawkEye != null && _random.NextDouble() <= hawkEye.Value / 100.0)
                        v = _random.NextDouble() <= 0.10 ? 3 : 2; // 10% chance for +3, else +2
                    return v;
                }

            case BonusType.LifePoint:
                {
                    // ~70% of missing HP
                    return (int)((player.MaxLifePoint - player.LifePoint) * 0.7);
                }

            case BonusType.MaxLifePoint:
                {
                    // Calibrate by steps and soft-cap by 20% of MaxHP
                    int steps = Math.Max(0, player.Steps);
                    int baseMin, baseMax;
                    if (steps < 200) baseMin = 2;
                    else if (steps < 500) baseMin = 3;
                    else if (steps < 800) baseMin = 4; else baseMin = 5;

                    if (steps < 200) baseMax = 4;
                    else if (steps < 500) baseMax = 6;
                    else if (steps < 800) baseMax = 7; else baseMax = 8;

                    int buildNudge = Math.Max(1, player.LifePoint / 16);
                    int roll = _random.Next(baseMin + buildNudge, baseMax + buildNudge + 1);
                    int cap = Math.Max(3, (int)Math.Round(player.MaxLifePoint * 0.20));
                    return Math.Min(roll, cap);
                }

            default: // Strength, Armor, Speed
                {
                    int s;
                    if (player.Steps < 200) s = _random.Next(1, 3);
                    else if (player.Steps < 600) s = _random.Next(2, 4);
                    else s = _random.Next(3, 5);

                    // LuckyMillorLeftHand: +1 proc
                    var lucky = player.Inventory.FirstOrDefault(i => i.Id == ItemId.LuckyMillorLeftHand);
                    if (lucky != null && _random.NextDouble() <= (double)lucky.Value / 100.0) s += 1;
                    return s;
                }
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

    // ---------- Rarity evaluation (for display only) ----------

    private static ItemRarity GetRarityForBonus(BonusType type, int value, Player player, ItemRarity? itemRarityFromItem)
    {
        if (type == BonusType.Item && itemRarityFromItem.HasValue) return itemRarityFromItem.Value;

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

            _ => value switch   // STR / ARM / SPD
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
            int missing = Math.Max(0, p.MaxLifePoint - p.LifePoint);
            double healPct = missing > 0 ? (double)Math.Min(amount, missing) / missing : 0;
            if (healPct < 0.25) return ItemRarity.Broken;
            if (healPct < 0.50) return ItemRarity.Common;
            if (healPct < 0.75) return ItemRarity.Uncommon;
            if (healPct < 0.95) return ItemRarity.Rare;
            return ItemRarity.Epic;
        }
    }

    // ---------- Prompt (Core-only; no color helpers) ----------

    public static Treasure PromptPlayerForBonus(List<Treasure> choices, Player player, GameSettings settings)
    {
        Console.Clear();

        // Minimal stats + inventory (Core-only)
        Console.WriteLine($"Steps: {player.Steps} | LVL: {player.Level} | XP: {player.XP} | Gold: {player.Gold}");
        Console.WriteLine($"HP: {player.LifePoint}/{player.MaxLifePoint} | STR: {player.Strength} | ARM: {player.Armor} | SPD: {player.Speed} | VIS: {player.Vision}");
        if (player.Inventory.Any())
        {
            Console.WriteLine("Inventory:");
            foreach (var it in player.Inventory)
                Console.WriteLine($"- {it.Name} ({it.EffectDescription}) x{it.Value}");
        }

        Console.WriteLine();
        Console.WriteLine(Messages.YouFoundATreasureChooseABonus);

        var keys = new List<string> { settings.Controls.Choice1, settings.Controls.Choice2, settings.Controls.Choice3 };

        for (int i = 0; i < choices.Count; i++)
        {
            var b = choices[i];

            ItemRarity? fromItem = null;
            string desc = b.Type == BonusType.Item
                ? FormatItemDescription((ItemId)b.Value, player, out fromItem)
                : $"+{b.Value} {Messages.ResourceManager.GetString(b.Type.ToString()) ?? b.Type.ToString()}";

            var rarity = GetRarityForBonus(b.Type, b.Value, player, fromItem);
            Console.WriteLine($"{keys[i]}. [{rarity}] {desc}");
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

        // Use actual rarity (don’t offset it)
        itemRarity = existing != null ? existing.Rarity : item.Rarity;

        return $"{item.Name} : {item.GetEffectDescription(displayValue)}";
    }

    // ---------- Apply ----------

    public static string ApplyBonus(Treasure bonus, Player player, GameSettings settings) =>
        bonus.Type switch
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

    private static string ApplyItemBonus(Player p, GameSettings settings, ItemId itemId)
    {
        var item = ItemFactory.CreateItem(itemId);
        return InventoryManager.TryAddItem(p, item, settings);
    }

    public static void ResetItemPool() => _selectedItemPool.Clear();
}
