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
        // TrollMushroom item logic 
        var trollMushroom = attacker.Inventory.FirstOrDefault(i => i.Id == ItemId.TrollMushroom);
        if (trollMushroom != null && round % 2 == 1)
        {
            // Cannot attack on odd rounds
            return AttackOutcome.UnderTrollMushroomEffect();
        }

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
                defender.LifePoint = Math.Min(defender.MaxLifePoint, defender.LifePoint + feathersOfHope.Value);
                restoredLife = defender.LifePoint - initialLifePoint;
            }
            return AttackOutcome.HasDodged(restoredLife);
        }

        // 2) Compute base damage (after armor). Min damage is 1.
        int baseDamage = Math.Max(0, attacker.Strength - defender.Armor);
        int minDamage = 1;
        decimal multiplier = 1m;
        if (trollMushroom != null)
        {
            multiplier *= trollMushroom.Value / 100m;
            minDamage = 2;
        }

        int damage = Math.Max(minDamage, (int)Math.Ceiling(baseDamage * multiplier));

        // 3) Roll crit + armor break (only if attacker’s Strength > defender’s Strength)
        bool isCrit = false;
        int armorShred = 0;
        int lifeStolen = 0;
        if (attacker.Strength > defender.Strength)
        {
            // RoyalGuardGauntlet and RoyalGuardShield logic
            var royalGantelet = attacker.Inventory.FirstOrDefault(i => i.Id == ItemId.RoyalGuardGauntlet);
            var royalShield = defender.Inventory.FirstOrDefault(i => i.Id == ItemId.RoyalGuardShield);
            decimal criticalChanceBonus = royalGantelet?.Value / 100m ?? 0;
            criticalChanceBonus -= royalShield?.Value / 100m ?? 0;

            if (_random.NextDouble() <= 0.15 + (double)criticalChanceBonus) // 15% crit chance by default
            {
                // BerserkerNecklace and PaladinNecklace logic
                var berserkerNecklace = attacker.Inventory.FirstOrDefault(i => i.Id == ItemId.BerserkerNecklace);
                var paladinNecklace = defender.Inventory.FirstOrDefault(i => i.Id == ItemId.PaladinNecklace);
                decimal criticalDamageBonus = 1.5m; // +50% damage by default
                criticalDamageBonus += berserkerNecklace?.Value / 100m ?? 0;
                criticalDamageBonus -= paladinNecklace?.Value / 100m ?? 0;

                isCrit = true;
                damage = (int)Math.Ceiling(damage * criticalDamageBonus); 
                armorShred = Math.Max(1, (int)Math.Round(defender.Armor * 0.05, MidpointRounding.AwayFromZero)); // -5% armor
                defender.Armor = Math.Max(0, defender.Armor - armorShred);

                // SealOfLivingFlesh item logic
                var sealOfLivingFlesh = attacker.Inventory.FirstOrDefault(i => i.Id == ItemId.SealOfLivingFlesh);
                if (sealOfLivingFlesh != null)
                {
                    var initialLifePoint = attacker.LifePoint;
                    attacker.LifePoint = Math.Min(attacker.MaxLifePoint, attacker.LifePoint + sealOfLivingFlesh.Value);
                    lifeStolen += attacker.LifePoint - initialLifePoint;
                }
            }
        }

        // OldGiantWoodenClub item logic
        var oldGiantWoodenClub = attacker.Inventory.FirstOrDefault(i => i.Id == ItemId.OldGiantWoodenClub);
        if (oldGiantWoodenClub != null)
        {
            // Breaks armor when less strength than opponent's armor
            if (attacker.Strength < defender.Armor)
            {
                var defenderArmorBeforeBreak = defender.Armor;
                defender.Armor = Math.Max(attacker.Strength, defender.Armor - oldGiantWoodenClub.Value);
                armorShred += defenderArmorBeforeBreak - defender.Armor;
            }
        }

        // 4) Apply damage to defender (with talisman safety if equiped)
        defender.LifePoint = Math.Max(0, defender.LifePoint - damage);

        // Talisman logic
        bool savedByTalisman = false;
        if (defender.LifePoint <= 0
            && defender.Inventory.Any(i => i.Id == ItemId.TalismanOfTheLastBreath)
            && !_talismanUsed.ContainsKey(defender.Name))
        {
            var talisman = defender.Inventory.First(i => i.Id == ItemId.TalismanOfTheLastBreath);
            defender.LifePoint = Math.Min(defender.MaxLifePoint, talisman.Value);
            savedByTalisman = true;
            _talismanUsed.Add(defender.Name, savedByTalisman);
        }

        // 5) Apply on-hit lifesteal for attacker (dagger)
        var dagger = attacker.Inventory.FirstOrDefault(i => i.Id == ItemId.DaggerLifeSteal);
        if (dagger != null && damage > 0 && _random.NextDouble() <= 0.6 )
        {
            int steal = Math.Min(damage, dagger.Value);
            int before = attacker.LifePoint;
            attacker.LifePoint = Math.Min(attacker.MaxLifePoint, attacker.LifePoint + steal);
            lifeStolen += attacker.LifePoint - before;
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
