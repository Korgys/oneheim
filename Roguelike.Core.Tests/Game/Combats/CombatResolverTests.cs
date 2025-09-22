using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Roguelike.Core.Game.Characters;
using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Game.Combat;

namespace Roguelike.Core.Tests.Game.Combats;

[TestClass]
public class CombatResolverTests
{
    [TestMethod]
    public void ExecuteAttack_ReturnsTrollMushroomEffect_OnOddRound()
    {
        var resolver = CreateResolver(1.0);
        var attacker = CreateCharacter("Attacker", life: 20, maxLife: 20, strength: 12, armor: 2, speed: 6);
        attacker.Inventory.Add(new Item { Id = ItemId.TrollMushroom, Value = 150 });
        var defender = CreateCharacter("Defender", life: 18, maxLife: 18, strength: 8, armor: 4, speed: 4);

        var outcome = resolver.ExecuteAttack(attacker, defender, round: 1);

        Assert.IsTrue(outcome.TrollMushroomEffect);
        Assert.AreEqual(0, outcome.Damage);
        Assert.AreEqual(18, defender.LifePoint);
    }

    [TestMethod]
    public void ExecuteAttack_DealsBaseDamageWithoutCritOrSpecials()
    {
        var resolver = CreateResolver(1.0);
        var attacker = CreateCharacter("Attacker", life: 22, maxLife: 22, strength: 10, armor: 2, speed: 5);
        var defender = CreateCharacter("Defender", life: 30, maxLife: 30, strength: 12, armor: 3, speed: 4);

        var outcome = resolver.ExecuteAttack(attacker, defender, round: 2);

        Assert.IsFalse(outcome.Crit);
        Assert.AreEqual(7, outcome.Damage);
        Assert.AreEqual(23, defender.LifePoint);
        Assert.AreEqual(0, outcome.ArmorShredded);
        Assert.AreEqual(0, outcome.LifeStolen);
        Assert.AreEqual(0, outcome.ThornsReflected);
        Assert.IsFalse(outcome.DefenderSavedByTalisman);
    }

    [TestMethod]
    public void ExecuteAttack_TalismanTriggersOnlyOnceWhenDefenderWouldDie()
    {
        var resolver = CreateResolver(1.0, 1.0);
        var attacker = CreateCharacter("Attacker", life: 25, maxLife: 25, strength: 40, armor: 2, speed: 5);
        var defender = CreateCharacter("Defender", life: 20, maxLife: 40, strength: 12, armor: 5, speed: 4);
        defender.Inventory.Add(new Item { Id = ItemId.TalismanOfTheLastBreath, Value = 15 });

        var firstOutcome = resolver.ExecuteAttack(attacker, defender, round: 2);
        var secondOutcome = resolver.ExecuteAttack(attacker, defender, round: 2);

        Assert.IsTrue(firstOutcome.DefenderSavedByTalisman);
        Assert.AreEqual(15, defender.LifePoint);
        Assert.IsFalse(secondOutcome.DefenderSavedByTalisman);
        Assert.AreEqual(0, defender.LifePoint);
    }

    [TestMethod]
    public void ExecuteAttack_AppliesOldGiantWoodenClubArmorShred()
    {
        var resolver = CreateResolver(1.0);
        var attacker = CreateCharacter("Attacker", life: 18, maxLife: 18, strength: 5, armor: 2, speed: 5);
        attacker.Inventory.Add(new Item { Id = ItemId.OldGiantWoodenClub, Value = 4 });
        var defender = CreateCharacter("Defender", life: 25, maxLife: 25, strength: 10, armor: 10, speed: 4);

        var outcome = resolver.ExecuteAttack(attacker, defender, round: 2);

        Assert.AreEqual(1, outcome.Damage);
        Assert.AreEqual(4, outcome.ArmorShredded);
        Assert.AreEqual(6, defender.Armor);
    }

    [TestMethod]
    public void ExecuteAttack_LifeStealRestoresHealthWhenChanceSucceeds()
    {
        var resolver = CreateResolver(0.0);
        var attacker = CreateCharacter("Attacker", life: 5, maxLife: 20, strength: 12, armor: 2, speed: 5);
        attacker.Inventory.Add(new Item { Id = ItemId.DaggerLifeSteal, Value = 3 });
        var defender = CreateCharacter("Defender", life: 18, maxLife: 18, strength: 15, armor: 4, speed: 4);

        var outcome = resolver.ExecuteAttack(attacker, defender, round: 2);

        Assert.AreEqual(8, outcome.Damage);
        Assert.AreEqual(3, outcome.LifeStolen);
        Assert.AreEqual(8, attacker.LifePoint);
    }

    [TestMethod]
    public void ExecuteAttack_ThornsReflectDamageWhenChanceSucceeds()
    {
        var resolver = CreateResolver(0.0);
        var attacker = CreateCharacter("Attacker", life: 20, maxLife: 20, strength: 10, armor: 2, speed: 5);
        var defender = CreateCharacter("Defender", life: 18, maxLife: 18, strength: 15, armor: 4, speed: 4);
        defender.Inventory.Add(new Item { Id = ItemId.ThornBreastplate, Value = 4 });

        var outcome = resolver.ExecuteAttack(attacker, defender, round: 2);

        Assert.AreEqual(6, outcome.Damage);
        Assert.AreEqual(4, outcome.ThornsReflected);
        Assert.AreEqual(16, attacker.LifePoint);
    }

    [TestMethod]
    public void ExecuteAttack_DodgeRestoresHealthWithFeathersOfHope()
    {
        var resolver = CreateResolver(0.0);
        var attacker = CreateCharacter("Attacker", life: 20, maxLife: 20, strength: 8, armor: 2, speed: 5);
        var defender = CreateCharacter("Defender", life: 10, maxLife: 15, strength: 8, armor: 4, speed: 12);
        defender.Inventory.Add(new Item { Id = ItemId.FeathersOfHope, Value = 3 });

        var outcome = resolver.ExecuteAttack(attacker, defender, round: 2);

        Assert.IsTrue(outcome.Dodged);
        Assert.AreEqual(0, outcome.Damage);
        Assert.AreEqual(13, defender.LifePoint);
        Assert.AreEqual(3, outcome.LifeStolen);
    }

    private static CombatResolver CreateResolver(params double[] randomSequence)
    {
        var resolver = new CombatResolver();
        var sequence = new SequenceRandom(randomSequence);
        var randomField = typeof(CombatResolver).GetField("_random", BindingFlags.Instance | BindingFlags.NonPublic);
        randomField!.SetValue(resolver, sequence);
        return resolver;
    }

    private static TestCharacter CreateCharacter(string name, int life, int maxLife, int strength, int armor, int speed)
    {
        return new TestCharacter
        {
            Name = name,
            LifePoint = life,
            MaxLifePoint = maxLife,
            Strength = strength,
            Armor = armor,
            Speed = speed,
        };
    }

    private sealed class TestCharacter : Character
    {
        private string _name = string.Empty;

        public override string Name
        {
            get => _name;
            set => _name = value;
        }
    }

    private sealed class SequenceRandom : Random
    {
        private readonly Queue<double> _values;

        public SequenceRandom(params double[] values)
        {
            _values = new Queue<double>(values.Length == 0 ? new[] { 1.0 } : values);
        }

        protected override double Sample()
        {
            return _values.Count > 0 ? _values.Dequeue() : 1.0;
        }
    }
}
