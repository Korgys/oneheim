namespace Roguelike.Core.Game.Combat;

using Roguelike.Core.Game.Abstractions;
using Roguelike.Core.Game.Characters;
using Roguelike.Core.Game.Characters.Enemies;
using Roguelike.Core.Game.Characters.Enemies.Bosses;
using Roguelike.Core.Game.Characters.Players;
using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Game.Combats;          // AttackOutcome, CombatResolver
using Roguelike.Core.Game.Levels;
using Roguelike.Core.Properties.i18n;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Pure core combat coordinator. No rendering, no sleeps.
/// Produces a report the presentation layer can display.
/// </summary>
public sealed class CombatManager
{
    private readonly LevelManager _level;
    private readonly CombatResolver _resolver;
    private readonly Queue<string> _fightLog = new();
    private readonly Random _rng = new();
    private readonly float _initialWaitTimeInTurn = 1000f;
    private readonly ICombatRenderer _ui; // <-- injected UI

    public CombatManager(LevelManager level, ICombatRenderer ui)
    {
        _level = level;
        _resolver = new CombatResolver();
        _ui = ui ?? new NullCombatRenderer();
    }

    /// <summary>
    /// Run a blocking (core-side) combat simulation and return an immutable report.
    /// The caller (UI/adapter) is responsible for rendering the report.
    /// </summary>
    public CombatReport StartCombat(Enemy enemy)
    {
        _level.PlayerInCombat = true;
        _fightLog.Clear();
        _ui.OnCombatStart(enemy is Boss);

        var player = _level.Player;

        // Initial wait time (longer for bosses)
        float waitTimeInTurn = _initialWaitTimeInTurn;
        if (enemy is Boss) waitTimeInTurn *= 2.5f;

        // Pre-combat hooks
        ApplyPreCombatModifiers(player, enemy);   // anti-Undead / Wild / Boss
        ApplyStartOfCombatBonuses(player);        // Ring of Endurance

        // Determine initiative from Speed (coin flip on tie)
        bool playerTurn = DetermineFirstTurn(player.Speed, enemy.Speed, _rng);

        int turn = 1;   // each action
        int round = 1;  // every two actions (player + enemy) => 1 round

        // Main loop
        while (player.LifePoint > 0 && enemy.LifePoint > 0)
        {
            if (IsRoundStart(turn))
                OnRoundStart(player, enemy, round);

            _ui.RenderTurn(enemy, player, _fightLog.ToArray());
            Thread.Sleep((int)waitTimeInTurn);

            var attacker = playerTurn ? (Character)player : enemy;
            var defender = playerTurn ? (Character)enemy : player;

            if (OnTurnStart(attacker, defender)) // return false to skip (e.g., stun)
            {
                var outcome = _resolver.ExecuteAttack(attacker, defender, round);
                _fightLog.Enqueue(playerTurn ? FormatPlayerAttack(outcome) : FormatEnemyAttack(outcome));
                TrimLog();

                // Player death handling (including talisman message already set in outcome)
                if (!playerTurn)
                {
                    if (outcome.DefenderSavedByTalisman)
                    {
                        _fightLog.Enqueue(Messages.YouSurvivedAFatalBlowWithYourTalisman);
                        TrimLog();
                    }
                    else if (player.LifePoint <= 0)
                    {
                        _fightLog.Enqueue(Messages.YouDied);
                        TrimLog();
                        break;
                    }
                }

                // Enemy dead
                if (enemy.LifePoint <= 0)
                {
                    _fightLog.Enqueue(Messages.EnemyDefeated);
                    TrimLog();
                    _level.Enemies.Remove(enemy);
                    break;
                }
            }

            OnTurnEnd(attacker, defender);

            // Alternate, advance counters
            playerTurn = !playerTurn;
            turn++;
            if (turn % 2 == 1) round++;
        }

        // Build report (reward if player survived)
        int gold = 0, xp = 0;
        if (player.LifePoint > 0)
        {
            gold = enemy.GetGoldValue();
            xp = enemy.GetXpValue();
            player.Gold += gold;

            // Apply XP and let the UI show stat changes if needed
            int beforeLevel = player.Level;
            int beforeMax = player.MaxLifePoint;
            int beforeStr = player.Strength;
            int beforeArm = player.Armor;
            int beforeSpd = player.Speed;

            player.GainXP(xp);

            if (player.Level > beforeLevel)
            {
                _fightLog.Enqueue(string.Format(Messages.YouLeveledUp, player.Level));
                TrimLog();

                if (player.MaxLifePoint > beforeMax)
                    _fightLog.Enqueue($"- MaxLifePoint: {beforeMax} -> {player.MaxLifePoint}");
                if (player.Strength > beforeStr)
                    _fightLog.Enqueue($"- Strength: {beforeStr} -> {player.Strength}");
                if (player.Armor > beforeArm)
                    _fightLog.Enqueue($"- Armor: {beforeArm} -> {player.Armor}");
                if (player.Speed > beforeSpd)
                    _fightLog.Enqueue($"- Speed: {beforeSpd} -> {player.Speed}");
                TrimLog();
            }
        }

        var combatReport = new CombatReport(
            EnemyName: enemy.Name,
            PlayerDied: player.LifePoint <= 0,
            EnemyDied: enemy.LifePoint <= 0,
            Gold: gold,
            Xp: xp,
            Log: _fightLog.ToArray()
        );

        _ui.OnCombatEnd(enemy, player, _fightLog.ToArray(), combatReport);
        _level.PlayerInCombat = false;

        return combatReport;
    }

    // ---------- Hooks & helpers ----------

    private static bool DetermineFirstTurn(int playerSpeed, int enemySpeed, Random rng)
        => playerSpeed == enemySpeed ? rng.NextDouble() >= 0.5 : playerSpeed > enemySpeed;

    /// <summary>Apply player item modifiers vs enemy category (pre-combat).</summary>
    private void ApplyPreCombatModifiers(Player player, Enemy enemy)
    {
        // Undead
        if (enemy.Category == EnemyType.Undead)
        {
            var holyBible = player.Inventory.FirstOrDefault(i => i.Id == ItemId.HolyBible);
            if (holyBible != null) enemy.Strength = Math.Max(0, enemy.Strength - holyBible.Value);

            var sacredCrucifix = player.Inventory.FirstOrDefault(i => i.Id == ItemId.SacredCrucifix);
            if (sacredCrucifix != null) enemy.Armor = Math.Max(0, enemy.Armor - sacredCrucifix.Value);
        }
        // Wild
        else if (enemy.Category == EnemyType.Wild)
        {
            var flute = player.Inventory.FirstOrDefault(i => i.Id == ItemId.FluteOfHunter);
            if (flute != null) enemy.Strength = Math.Max(0, enemy.Strength - flute.Value);

            var fangs = player.Inventory.FirstOrDefault(i => i.Id == ItemId.EngravedFangs);
            if (fangs != null) enemy.Armor = Math.Max(0, enemy.Armor - fangs.Value);
        }

        // Boss
        if (enemy is Boss)
        {
            var blade = player.Inventory.FirstOrDefault(i => i.Id == ItemId.BladeOfHeroes);
            if (blade != null) enemy.Strength = Math.Max(0, enemy.Strength - blade.Value);

            var shield = player.Inventory.FirstOrDefault(i => i.Id == ItemId.ShieldOfChampion);
            if (shield != null) enemy.Armor = Math.Max(0, enemy.Armor - shield.Value);
        }
    }

    /// <summary>Apply start-of-combat one-shots (e.g., Ring of Endurance).</summary>
    private void ApplyStartOfCombatBonuses(Player player)
    {
        var ring = player.Inventory.FirstOrDefault(i => i.Id == ItemId.RingOfEndurance);
        if (ring != null)
        {
            int before = player.LifePoint;
            player.LifePoint = Math.Min(player.MaxLifePoint, player.LifePoint + ring.Value);
            int healed = player.LifePoint - before;
            if (healed > 0)
            {
                _fightLog.Enqueue(string.Format(Messages.YouHealWithYourRingOfEndurance, healed));
                TrimLog();
            }
        }
    }

    /// <summary>Called at the start of each attacker’s turn. Return false to skip the action (e.g., stunned).</summary>
    private bool OnTurnStart(Character attacker, Character defender)
    {
        // TODO: Apply poison/burn ticks, check stun or silence; return false to skip.
        return true;
    }

    /// <summary>Called at the end of each turn.</summary>
    private void OnTurnEnd(Character attacker, Character defender)
    {
        // TODO: Decay temporary effects, handle end-of-turn item hooks, etc.
    }

    /// <summary>Called at the start of each round (every two turns).</summary>
    private void OnRoundStart(Player player, Enemy enemy, int round)
    {
        // TODO: Round-based effects (e.g., ramping aura, periodic buffs).
    }

    private static bool IsRoundStart(int turn) => turn % 2 == 1;

    private static string FormatPlayerAttack(AttackOutcome r)
    {
        if (r.Dodged) return Messages.TheEnemyDodgedYourAttack;
        if (r.TrollMushroomEffect) return Messages.YouAreUnderTrollMushroomEffect;

        var parts = new List<string> { string.Format(Messages.YouDealDamage, r.Damage) };
        if (r.Crit) parts[0] += " (crit)";
        if (r.ArmorShredded > 0) parts.Add(string.Format(Messages.ShredArmor, r.ArmorShredded));
        if (r.LifeStolen > 0) parts.Add(string.Format(Messages.StealHp, r.LifeStolen));
        if (r.ThornsReflected > 0) parts.Add(string.Format(Messages.AndTakeThornsDamage, r.ThornsReflected));
        return string.Join($" {Messages.And} ", parts) + ".";
    }

    private static string FormatEnemyAttack(AttackOutcome r)
    {
        if (r.Dodged)
            return r.LifeStolen > 0
                ? string.Format(Messages.YouDodgedTheEnemyAttackAndGainHp, r.LifeStolen)
                : Messages.YouDodgedTheEnemyAttack;

        if (r.TrollMushroomEffect) return Messages.EnemyIsUnderTrollMushroomEffect;

        var parts = new List<string> { string.Format(Messages.TheEnemyDealDamageToYou, r.Damage) };
        if (r.Crit) parts[0] += " (crit)";
        if (r.ArmorShredded > 0) parts.Add(string.Format(Messages.ShredArmor, r.ArmorShredded));
        if (r.LifeStolen > 0) parts.Add(string.Format(Messages.StealHp, r.LifeStolen));
        if (r.ThornsReflected > 0) parts.Add(string.Format(Messages.AndTakeThornsDamage, r.ThornsReflected));
        return string.Join($" {Messages.And} ", parts) + ".";
    }

    private void TrimLog()
    {
        while (_fightLog.Count > 20) _fightLog.Dequeue();
    }
}

/// <summary>
/// Immutable result of a combat simulation.
/// </summary>
public readonly record struct CombatReport(
    string EnemyName,
    bool PlayerDied,
    bool EnemyDied,
    int Gold,
    int Xp,
    IReadOnlyList<string> Log);
