namespace Roguelike.Console.Game.Combat;

using Roguelike.Console.Game.Characters;
using Roguelike.Console.Game.Characters.Players;
using Roguelike.Console.Game.Collectables.Items;

public sealed class CombatResolver
{
    private readonly Random _random = new Random();

    public AttackOutcome ExecuteAttack(Character attacker, Character defender)
    {
        // 1) Compute defender dodge chance (with boots if any)
        if (TryDodge(attacker, defender, out double dodgeChance)
            && _random.NextDouble() < dodgeChance)
        {
            // FeathersOfHope logic
            int restoredLife = 0;
            var feathersOfHope = defender.Inventory.FirstOrDefault(i => i.Id == ItemId.FeathersOfHope);
            if (feathersOfHope != null)
            {
                var initialLifePoint = defender.LifePoint;
                var stealPoints = feathersOfHope.Value * defender.MaxLifePoint / 100;
                defender.LifePoint = Math.Min(defender.MaxLifePoint, defender.LifePoint + stealPoints);
                restoredLife = defender.LifePoint - initialLifePoint;
            }
            return AttackOutcome.HasDodged(restoredLife);
        }

        // 2) Compute base damage (after armor). Min damage is 1.
        int damage = Math.Max(1, attacker.Strength - defender.Armor);

        // 3) Roll crit + armor break (only if attacker’s Strength > defender’s Strength)
        bool isCrit = false;
        int armorShred = 0;
        if (attacker.Strength > defender.Strength)
        {
            // RoyalGuardGauntlet and RoyalGuardShield logic
            var royalGantelet = attacker.Inventory.FirstOrDefault(i => i.Id == ItemId.RoyalGuardGauntlet);
            var royalShield = defender.Inventory.FirstOrDefault(i => i.Id == ItemId.RoyalGuardShield);
            var criticalChanceBonus = royalGantelet?.Value ?? 0;
            criticalChanceBonus -= royalShield?.Value ?? 0;
            criticalChanceBonus = Math.Min(0, Math.Max(1, criticalChanceBonus));

            if (_random.NextDouble() <= 0.15 + criticalChanceBonus) // 15% crit chance by default
            {
                // BerserkerNecklace and PaladinNecklace logic
                var berserkerNecklace = attacker.Inventory.FirstOrDefault(i => i.Id == ItemId.BerserkerNecklace);
                var paladinNecklace = defender.Inventory.FirstOrDefault(i => i.Id == ItemId.PaladinNecklace);
                var criticalDamageBonus = 1.5; // +50% damage by default
                criticalDamageBonus += berserkerNecklace?.Value ?? 0;
                criticalDamageBonus -= paladinNecklace?.Value ?? 0;

                isCrit = true;
                damage = (int)Math.Ceiling(damage * criticalDamageBonus); 
                armorShred = Math.Max(1, (int)Math.Round(defender.Armor * 0.10, MidpointRounding.AwayFromZero)); // -5% armor
                defender.Armor = Math.Max(0, defender.Armor - armorShred);
            }
        }

        // 4) Apply damage to defender (with talisman safety if equiped)
        defender.LifePoint = Math.Max(0, defender.LifePoint - damage);

        // Talisman logic
        bool savedByTalisman = false;
        if (defender.LifePoint <= 0 && defender.Inventory.Any(i => i.Id == ItemId.TalismanOfTheLastBreath))
        {
            var talisman = defender.Inventory.First(i => i.Id == ItemId.TalismanOfTheLastBreath);
            defender.LifePoint = Math.Min(defender.MaxLifePoint, talisman.Value);
            savedByTalisman = true;
            // Consume talisman → remove from inventory          
            defender.Inventory.Remove(talisman);
        }

        // 5) Apply on-hit lifesteal for attacker (dagger)
        int lifeStolen = 0;
        var dagger = attacker.Inventory.FirstOrDefault(i => i.Id == ItemId.DaggerLifeSteal);
        if (dagger != null && damage > 0 && _random.NextDouble() <= 0.6 )
        {
            int steal = Math.Min(damage, dagger.Value);
            int before = attacker.LifePoint;
            attacker.LifePoint = Math.Min(attacker.MaxLifePoint, attacker.LifePoint + steal);
            lifeStolen = attacker.LifePoint - before;
        }

        // 6) Apply thorns on attacker if defender has breastplate and got hit
        int thornsDamage = 0;
        var breastplate = defender.Inventory.FirstOrDefault(i => i.Id == ItemId.ThornBreastplate);
        if (breastplate != null && damage > 0 && _random.NextDouble() <= 0.6)
        {
            thornsDamage = Math.Max(0, breastplate.Value);
            if (thornsDamage > 0)
            {
                attacker.LifePoint = Math.Max(0, attacker.LifePoint - thornsDamage);
            }
        }

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
        double slope = 0.02;
        double max = 0.75;

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

/// <summary>
/// Represent the result of a single resolved attack.
/// </summary>
public readonly record struct AttackOutcome(
    bool Dodged,
    int Damage,
    bool Crit,
    int ArmorShredded,
    int LifeStolen,
    int ThornsReflected,
    bool DefenderSavedByTalisman
)
{
    public static AttackOutcome HasDodged(int restauredLife) => new(true, 0, false, 0, restauredLife, 0, false);
}
