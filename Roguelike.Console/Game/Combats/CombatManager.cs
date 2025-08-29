namespace Roguelike.Console.Game.Combat;

using Roguelike.Console.Game.Characters;
using Roguelike.Console.Game.Characters.Enemies;
using Roguelike.Console.Game.Characters.Enemies.Bosses;
using Roguelike.Console.Game.Characters.Players;
using Roguelike.Console.Game.Collectables.Items;
using Roguelike.Console.Game.Combats;
using Roguelike.Console.Game.Levels;
using Roguelike.Console.Game.Rendering;
using Roguelike.Console.Properties.i18n;
using System;

public class CombatManager
{
    private readonly LevelManager _level;
    private readonly CombatResolver _resolver = new();
    private Queue<string> _fightLog = new();
    private Random _random = new Random();
    private readonly float _initialWaitTimeInTurn = 1000f;

    public CombatManager(LevelManager level)
    {
        _level = level;
    }

    public void StartCombat(Enemy enemy)
    {
        _level.PlayerInCombat = true;
        CombatRenderer.BlinkConsole(enemy is Boss);

        var player = _level.Player;

        // Setup combat state
        ApplyPreCombatModifiers(player, enemy);              // items anti-Undead/Wild/Boss
        ApplyStartOfCombatBonuses(player);                   // Ring of Endurance

        // Advantage in turn : player vs enemy speed
        bool playerTurn = DetermineFirstTurn(player.Speed, enemy.Speed, _random);

        // Render pacing
        float waitTimeInTurn = _initialWaitTimeInTurn;
        if (enemy is Boss) waitTimeInTurn *= 2.5f; // Bosses have slower pacing

        int turn = 1;   // n° de tour (une action = 1 tour)
        int round = 1;  // n° de manche (2 tours = 1 manche : joueur+ennemi)

        _fightLog.Clear();

        // Combat loop
        while (player.LifePoint > 0 && enemy.LifePoint > 0)
        {
            // Start of round: periodic effects (every 2 turns)
            if (IsRoundStart(turn)) OnRoundStart(player, enemy, round);

            // Rendering fight state
            CombatRenderer.RenderFight(enemy, player);
            Console.WriteLine(string.Join("\n", _fightLog));
            Thread.Sleep((int)waitTimeInTurn);

            // Who's attacking?
            var attacker = playerTurn ? (Character)player : enemy;
            var defender = playerTurn ? (Character)enemy : player;

            // Hook start of turn
            if (OnTurnStart(attacker, defender)) // true = continue, false = skip action (stun, etc.)
            {
                var outcome = _resolver.ExecuteAttack(attacker, defender, round);
                AddLog(playerTurn ? FormatPlayerAttack(outcome) : FormatEnemyAttack(outcome));

                // Special effect
                if (!playerTurn)
                {
                    if (outcome.DefenderSavedByTalisman)
                        AddLog(Messages.YouSurvivedAFatalBlowWithYourTalisman);
                    else if (player.LifePoint <= 0)
                    {
                        AddLog(Messages.YouDied);
                        break;
                    }
                }

                if (enemy.LifePoint <= 0)
                {
                    AddLog(Messages.EnemyDefeated);
                    _level.Enemies.Remove(enemy);
                    break;
                }
            }

            // End turn hooks
            OnTurnEnd(attacker, defender);

            // Pacing : wait time between turns accelerates
            if (waitTimeInTurn > _initialWaitTimeInTurn / 4f) waitTimeInTurn *= 0.98f;

            // Alterning + counters
            playerTurn = !playerTurn;
            turn++;
            if (turn % 2 == 1) round++; // each 2 actions is a new round
        }

        CombatRenderer.RenderEndFight(enemy, player, _fightLog);
        _level.PlayerInCombat = false;
    }

    private static bool DetermineFirstTurn(int playerSpeed, int enemySpeed, Random rng)
    {
        if (playerSpeed == enemySpeed) return rng.NextDouble() >= 0.5; // pile ou face propre
        return playerSpeed > enemySpeed;
    }

    /// <summary>
    /// Applies pre-combat modifiers to the specified enemy based on the player's inventory and the enemy's category.
    /// </summary>
    /// <param name="player">The player whose inventory is used to apply modifiers.</param>
    /// <param name="enemy">The enemy whose attributes are modified based on the player's inventory and the enemy's type.</param>
    private void ApplyPreCombatModifiers(Player player, Enemy enemy)
    {
        // Catégorie : Undead
        if (enemy.Category == EnemyType.Undead)
        {
            var holyBible = player.Inventory.FirstOrDefault(i => i.Id == ItemId.HolyBible);
            if (holyBible != null) enemy.Strength = Math.Max(0, enemy.Strength - holyBible.Value);

            var sacredCrucifix = player.Inventory.FirstOrDefault(i => i.Id == ItemId.SacredCrucifix);
            if (sacredCrucifix != null) enemy.Armor = Math.Max(0, enemy.Armor - sacredCrucifix.Value);
        }
        // Catégorie : Wild
        else if (enemy.Category == EnemyType.Wild)
        {
            var fluteOfHunter = player.Inventory.FirstOrDefault(i => i.Id == ItemId.FluteOfHunter);
            if (fluteOfHunter != null) enemy.Strength = Math.Max(0, enemy.Strength - fluteOfHunter.Value);

            var engravedFangs = player.Inventory.FirstOrDefault(i => i.Id == ItemId.EngravedFangs);
            if (engravedFangs != null) enemy.Armor = Math.Max(0, enemy.Armor - engravedFangs.Value);
        }

        // Boss
        if (enemy is Boss)
        {
            var bladeOfHeroes = player.Inventory.FirstOrDefault(i => i.Id == ItemId.BladeOfHeroes);
            if (bladeOfHeroes != null) enemy.Strength = Math.Max(0, enemy.Strength - bladeOfHeroes.Value);

            var shieldOfChampion = player.Inventory.FirstOrDefault(i => i.Id == ItemId.ShieldOfChampion);
            if (shieldOfChampion != null) enemy.Armor = Math.Max(0, enemy.Armor - shieldOfChampion.Value);
        }
    }

    private void ApplyStartOfCombatBonuses(Player player)
    {
        // Anneau d’endurance : +PV au début du combat
        var ringOfEndurance = player.Inventory.FirstOrDefault(i => i.Id == ItemId.RingOfEndurance);
        if (ringOfEndurance != null)
        {
            int before = player.LifePoint;
            player.LifePoint = Math.Min(player.MaxLifePoint, player.LifePoint + ringOfEndurance.Value);
            if (player.LifePoint > before)
                AddLog(string.Format(Messages.YouHealWithYourRingOfEndurance, player.LifePoint - before));
        }
    }

    private bool OnTurnStart(Character attacker, Character defender)
    {
        // TODO: Apply poison/burn damage at the start of the attacker's turn
        // TODO: Manage stun => return false to skip the action
        return true;
    }

    private void OnTurnEnd(Character attacker, Character defender)
    {
        // Will be called at the end of each turn
    }

    private void OnRoundStart(Player player, Enemy enemy, int round)
    {
        // Will be called at the start of each round (every 2 turns)
    }

    private static bool IsRoundStart(int turn)
    {
        // Start of a new round every 2 turns (1 for player, 1 for enemy)
        return turn % 2 == 1;
    }

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
        {
            if (r.LifeStolen > 0) return string.Format(Messages.YouDodgedTheEnemyAttackAndGainHp, r.LifeStolen);
            else return Messages.YouDodgedTheEnemyAttack;
        }

        if (r.TrollMushroomEffect) return Messages.EnemyIsUnderTrollMushroomEffect;

        var parts = new List<string> { string.Format(Messages.TheEnemyDealDamageToYou, r.Damage) };
        if (r.Crit) parts[0] += " (crit)";
        if (r.ArmorShredded > 0) parts.Add(string.Format(Messages.ShredArmor, r.ArmorShredded));
        if (r.LifeStolen > 0) parts.Add(string.Format(Messages.StealHp, r.LifeStolen));
        if (r.ThornsReflected > 0) parts.Add(string.Format(Messages.AndTakeThornsDamage, r.ThornsReflected));
        return string.Join($" {Messages.And} ", parts) + ".";
    }

    private void AddLog(string line)
    {
        _fightLog.Enqueue(line);
        while (_fightLog.Count > 10)
            _fightLog.Dequeue();
    }
}
