namespace Roguelike.Core.Game.Combat;

using Roguelike.Core.Game.Characters;
using Roguelike.Core.Game.Characters.Players;
using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Game.Combats;

public sealed class CombatResolver
{
    private readonly Random _random = new Random();
    private readonly Dictionary<string, bool> _talismanUsed = new Dictionary<string, bool>();

    public AttackOutcome ExecuteAttack(Character attacker, Character defender, int round)
    {
        var trollMushroom = attacker.Inventory.FirstOrDefault(i => i.Id == ItemId.TrollMushroom);
        if (IsUnderTrollMushroomEffect(trollMushroom, round))
        {
            return AttackOutcome.UnderTrollMushroomEffect();
        }

        if (TryResolveDodge(attacker, defender, out var dodgeOutcome))
        {
            return dodgeOutcome;
        }

        int damage = ComputeBaseDamage(attacker, defender, trollMushroom);

        var critResult = ApplyCriticalEffects(attacker, defender, damage);
        damage = critResult.Damage;
        bool isCrit = critResult.IsCritical;
        int armorShred = critResult.ArmorShred;
        int lifeStolen = critResult.LifeStolen;

        armorShred += ApplyOldGiantWoodenClub(attacker, defender);

        bool savedByTalisman = ApplyDamageAndTalisman(defender, damage);

        lifeStolen += ApplyLifeSteal(attacker, damage);

        int thornsDamage = ApplyThorns(attacker, defender, damage);

        return new AttackOutcome(
            Dodged: false,
            Damage: damage,
            Crit: isCrit,
            ArmorShredded: armorShred,
            LifeStolen: lifeStolen,
            ThornsReflected: thornsDamage,
            DefenderSavedByTalisman: savedByTalisman
        );
    }

    private bool IsUnderTrollMushroomEffect(Item trollMushroom, int round)
    {
        if (trollMushroom == null)
        {
            return false;
        }

        // Cannot attack on odd rounds
        return round % 2 == 1;
    }

    private bool TryResolveDodge(Character attacker, Character defender, out AttackOutcome outcome)
    {
        outcome = default;

        if (!TryDodge(attacker, defender, out double dodgeChance)
            || _random.NextDouble() >= dodgeChance)
        {
            return false;
        }

        int restoredLife = 0;
        var feathersOfHope = defender.Inventory.FirstOrDefault(i => i.Id == ItemId.FeathersOfHope);
        if (feathersOfHope != null)
        {
            var initialLifePoint = defender.LifePoint;
            defender.LifePoint = Math.Min(defender.MaxLifePoint, defender.LifePoint + feathersOfHope.Value);
            restoredLife = defender.LifePoint - initialLifePoint;
        }

        outcome = AttackOutcome.HasDodged(restoredLife);
        return true;
    }

    private static int ComputeBaseDamage(Character attacker, Character defender, Item trollMushroom)
    {
        int baseDamage = Math.Max(0, attacker.Strength - defender.Armor);

        if (trollMushroom == null)
        {
            return Math.Max(1, baseDamage);
        }

        decimal multiplier = trollMushroom.Value / 100m;
        int scaledDamage = (int)Math.Ceiling(baseDamage * multiplier);
        return Math.Max(2, scaledDamage);
    }

    private (int Damage, bool IsCritical, int ArmorShred, int LifeStolen) ApplyCriticalEffects(
        Character attacker,
        Character defender,
        int damage)
    {
        if (attacker.Strength <= defender.Strength)
        {
            return (damage, false, 0, 0);
        }

        var royalGantelet = attacker.Inventory.FirstOrDefault(i => i.Id == ItemId.RoyalGuardGauntlet);
        var royalShield = defender.Inventory.FirstOrDefault(i => i.Id == ItemId.RoyalGuardShield);
        decimal criticalChanceBonus = royalGantelet?.Value / 100m ?? 0;
        criticalChanceBonus -= royalShield?.Value / 100m ?? 0;

        double critChance = 0.15 + (double)criticalChanceBonus; // 15% crit chance by default
        if (_random.NextDouble() > critChance)
        {
            return (damage, false, 0, 0);
        }

        var berserkerNecklace = attacker.Inventory.FirstOrDefault(i => i.Id == ItemId.BerserkerNecklace);
        var paladinNecklace = defender.Inventory.FirstOrDefault(i => i.Id == ItemId.PaladinNecklace);
        decimal criticalDamageBonus = 1.5m; // +50% damage by default
        criticalDamageBonus += berserkerNecklace?.Value / 100m ?? 0;
        criticalDamageBonus -= paladinNecklace?.Value / 100m ?? 0;

        int criticalDamage = (int)Math.Ceiling(damage * criticalDamageBonus);
        int armorShred = Math.Max(1, (int)Math.Round(defender.Armor * 0.05, MidpointRounding.AwayFromZero)); // -5% armor
        defender.Armor = Math.Max(0, defender.Armor - armorShred);

        int lifeStolen = 0;
        var sealOfLivingFlesh = attacker.Inventory.FirstOrDefault(i => i.Id == ItemId.SealOfLivingFlesh);
        if (sealOfLivingFlesh != null)
        {
            var initialLifePoint = attacker.LifePoint;
            attacker.LifePoint = Math.Min(attacker.MaxLifePoint, attacker.LifePoint + sealOfLivingFlesh.Value);
            lifeStolen += attacker.LifePoint - initialLifePoint;
        }

        return (criticalDamage, true, armorShred, lifeStolen);
    }

    private int ApplyOldGiantWoodenClub(Character attacker, Character defender)
    {
        var oldGiantWoodenClub = attacker.Inventory.FirstOrDefault(i => i.Id == ItemId.OldGiantWoodenClub);
        if (oldGiantWoodenClub == null || attacker.Strength >= defender.Armor)
        {
            return 0;
        }

        var defenderArmorBeforeBreak = defender.Armor;
        defender.Armor = Math.Max(attacker.Strength, defender.Armor - oldGiantWoodenClub.Value);
        return defenderArmorBeforeBreak - defender.Armor;
    }

    private bool ApplyDamageAndTalisman(Character defender, int damage)
    {
        defender.LifePoint = Math.Max(0, defender.LifePoint - damage);

        if (defender.LifePoint > 0
            || !defender.Inventory.Any(i => i.Id == ItemId.TalismanOfTheLastBreath)
            || _talismanUsed.ContainsKey(defender.Name))
        {
            return false;
        }

        var talisman = defender.Inventory.First(i => i.Id == ItemId.TalismanOfTheLastBreath);
        defender.LifePoint = Math.Min(defender.MaxLifePoint, talisman.Value);
        _talismanUsed.Add(defender.Name, true);
        return true;
    }

    private int ApplyLifeSteal(Character attacker, int damage)
    {
        var dagger = attacker.Inventory.FirstOrDefault(i => i.Id == ItemId.DaggerLifeSteal);
        if (dagger == null || damage <= 0 || _random.NextDouble() > 0.6)
        {
            return 0;
        }

        int steal = Math.Min(damage, dagger.Value);
        int before = attacker.LifePoint;
        attacker.LifePoint = Math.Min(attacker.MaxLifePoint, attacker.LifePoint + steal);
        return attacker.LifePoint - before;
    }

    private int ApplyThorns(Character attacker, Character defender, int damage)
    {
        var breastplate = defender.Inventory.FirstOrDefault(i => i.Id == ItemId.ThornBreastplate);
        if (breastplate == null || damage <= 0 || _random.NextDouble() > 0.6)
        {
            return 0;
        }

        int thornsDamage = Math.Max(0, breastplate.Value);
        if (thornsDamage > 0)
        {
            attacker.LifePoint = Math.Max(0, attacker.LifePoint - thornsDamage);
        }

        return thornsDamage;
    }

    /// <summary>
    /// Compute defender dodge chance based on speed diff and boots; return true if dodge can occur.
    /// </summary>
    private bool TryDodge(Character attacker, Character defender, out double chance)
    {
        int speedDiff = defender.Speed - attacker.Speed;
        if (speedDiff <= 0)
        {
            chance = 0;
            return false;
        }

        // Tune slopes/caps depending on who defends (keep your previous values)
        double slope = 0.025;
        double max = 0.85; // Max chances of dodging the attack

        // Boots add a flat percentage to player's dodge chance while defending
        double bootsBonus = 0;
        if (defender is Player)
        {
            var boots = defender.Inventory.FirstOrDefault(i => i.Id == ItemId.BootsOfEchoStep);
            if (boots != null) bootsBonus += boots.Value / 100.0;
        }

        chance = Math.Min(max, slope * speedDiff + bootsBonus);
        return chance > 0;
    }
}
