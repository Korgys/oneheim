namespace Roguelike.Console.Game.Collectables;

using Roguelike.Console.Configuration;
using Roguelike.Console.Game.Characters.Players;
using Roguelike.Console.Game.Collectables.Items;
using Roguelike.Console.Properties.i18n;
using System;

public static class TreasureSelector
{
    private static List<ItemId> _selectedItemPool = new();
    private static readonly Random _random = new();
    private static readonly int _numberOfItemInPool = 9; // Number of items to randomly select from the full item pool

    public static List<Treasure> GenerateBonusChoices(Player player, GameSettings settings)
    {
        EnsureItemPoolInitialized();

        // Bonus types list
        var types = Enum.GetValues<BonusType>().ToList();

        // Avoid LifePoint if player HP ratio > 50%
        double hpRatio = player.MaxLifePoint > 0 ? (double)player.LifePoint / player.MaxLifePoint : 0;
        if (hpRatio > 0.5)
            types.Remove(BonusType.LifePoint);

        // Do not offer Vision once it's already high (cap ~10; offer stops at >=9)
        if (player.Vision >= 9)
            types.Remove(BonusType.Vision);

        var result = new List<Treasure>();
        var usedNonItemTypes = new HashSet<BonusType>();

        // 50% Optional stat-focus pick (one guaranteed slot if a stat is strictly dominant and > 10)
        if (_random.NextDouble() <= 0.5) TryAddStatFocus(player, result, usedNonItemTypes);

        // Fill remaining slots up to 3
        int safety = 50; // prevent infinite loops in edge cases
        while (result.Count < 3 && safety-- > 0)
        {
            // Build available types for this iteration
            var available = types
                .Where(t => !usedNonItemTypes.Contains(t))
                .ToList();

            // If nothing left, build a simple fallback set
            if (available.Count == 0) break;

            // Pick a type
            var type = available[_random.Next(available.Count)];
            if (type != BonusType.Item) usedNonItemTypes.Add(type); // Avoid duplicate type bonus, except Items

            int value = GenerateValueForBonus(type, player, result);
            result.Add(new Treasure { Type = type, Value = value });
        }

        // Last safety: if for some reason we have <3, top up with safe defaults
        while (result.Count < 3)
        {
            result.Add(new Treasure { Type = BonusType.MaxLifePoint, Value = Math.Max(1, player.MaxLifePoint / 10) });
        }

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

    private static int GenerateValueForBonus(BonusType type, Player player, List<Treasure> treasuresSelection = null)
    {
        switch (type)
        {
            case BonusType.Item:
                var validItems = _selectedItemPool
                .Where(itemId =>
                {
                    // Exclude player items already in inventory not upgradable
                    var existing = player.Inventory.FirstOrDefault(i => i.Id == itemId);
                    if (existing != null)
                    {
                        if (existing.UpgradableIncrementValue == 0) return false;
                        if (existing.Rarity >= ItemRarity.Legendary) return false;
                    }

                    // Exclude items already in treasure selection
                    if (treasuresSelection?.Any(t => t.Type == BonusType.Item && (ItemId)t.Value == itemId) == true) return false;

                    return true;
                })
                .ToList();

                if (validItems.Count == 0)
                    return -1; // No valid items to pick

                return (int)validItems[_random.Next(validItems.Count)];
            case BonusType.Vision:
                // hawkEye item logic
                var hawkEye = player.Inventory.FirstOrDefault(i => i.Id == ItemId.HawkEye);
                if (hawkEye != null && _random.NextDouble() <= (float)hawkEye.Value/100)
                {
                    if (_random.NextDouble() <= 0.1) // 10% chance to double the effect
                        return 3;
                    else return 2;
                }
                else return 1;
            case BonusType.LifePoint:
                return (int)((player.MaxLifePoint - player.LifePoint) * 0.6);
            case BonusType.MaxLifePoint:
                return _random.Next(3, 6) * Math.Max(1, player.Steps / 100);
            default: // Strength, Armor, Speed
                return _random.Next(1, 3) + Math.Max(1, player.Steps / 100);
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

    public static Treasure PromptPlayerForBonus(List<Treasure> choices, Player player, GameSettings settings)
    {
        Console.Clear();
        Console.WriteLine($"{Messages.HP}: {player.LifePoint}/{player.MaxLifePoint} | " +
            $"{Messages.Strength}: {player.Strength} | {Messages.Armor}: {player.Armor} | " +
            $"{Messages.Speed}: {player.Speed} | {Messages.Vision}: {player.Vision}");
        Console.WriteLine();

        if (player.Inventory.Any())
        {
            Console.WriteLine($"{Messages.Inventory}:");
            foreach (var item in player.Inventory)
                Console.WriteLine($"- {item.Name} ({item.EffectDescription})");
            Console.WriteLine();
        }

        Console.WriteLine(Messages.YouFoundATreasureChooseABonus);

        var keys = new List<string> { settings.Controls.Choice1, settings.Controls.Choice2, settings.Controls.Choice3 };
        for (int i = 0; i < choices.Count; i++)
        {
            var b = choices[i];
            string desc = b.Type == BonusType.Item
                ? FormatItemDescription((ItemId)b.Value, player)
                : $"+{b.Value} {Messages.ResourceManager.GetString(b.Type.ToString()) ?? b.Type.ToString()}";
            Console.WriteLine($"{keys[i]}. {desc}");
        }

        int chosen = -1;
        while (chosen == -1)
        {
            var key = Console.ReadKey(true).Key.ToString().ToUpperInvariant();
            chosen = keys.FindIndex(k => k.ToUpperInvariant() == key);
        }

        return choices[chosen];
    }

    private static string FormatItemDescription(ItemId itemId, Player player)
    {
        var item = ItemFactory.CreateItem(itemId);
        var existing = player.Inventory.FirstOrDefault(i => i.Id == itemId);
        int displayValue = existing != null
            ? existing.Value + existing.UpgradableIncrementValue
            : item.Value;

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
        {
            player.LifePoint += value;
        }

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
