namespace Roguelike.Console.Game.Combat;

using Roguelike.Console.Game.Characters.Enemies;
using Roguelike.Console.Game.Characters.Enemies.Bosses;
using Roguelike.Console.Game.Collectables.Items;
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
        CombatRendering.BlinkConsole(enemy is Boss);

        var player = _level.Player;
        bool playerTurn = player.Speed == enemy.Speed
            ? _random.NextDouble() >= 0.5
            : player.Speed > enemy.Speed;

        float waitTimeInTurn = _initialWaitTimeInTurn; // 1s
        if (enemy is Boss) waitTimeInTurn *= 2.5f; // Bosses have a longer wait time

        // items logic before combat
        if (enemy.Category == EnemyType.Undead)
        {
            var holyBible = player.Inventory.FirstOrDefault(i => i.Id == ItemId.HolyBible);
            if (holyBible != null)
            {
                enemy.Strength = Math.Max(0, enemy.Strength - holyBible.Value);
            }

            var sacredCrucifix = player.Inventory.FirstOrDefault(i => i.Id == ItemId.SacredCrucifix);
            if (sacredCrucifix != null)
            {
                enemy.Armor = Math.Max(0, enemy.Armor - sacredCrucifix.Value);
            }
        }

        while (player.LifePoint > 0 && enemy.LifePoint > 0)
        {
            CombatRendering.RenderFight(enemy, player);
            Console.WriteLine(string.Join("\n", _fightLog));
            Thread.Sleep((int)waitTimeInTurn);

            if (playerTurn) // Player turn
            {
                var outcome = _resolver.ExecuteAttack(player, enemy);

                AddLog(FormatPlayerAttack(outcome));
            }
            else // Enemy turn
            {
                var outcome = _resolver.ExecuteAttack(enemy, player);

                AddLog(FormatEnemyAttack(outcome));

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

            if (waitTimeInTurn > (float)_initialWaitTimeInTurn / 4) waitTimeInTurn *= 0.98f;

            playerTurn = !playerTurn;
        }

        CombatRendering.RenderEndFight(enemy, player, _fightLog);
        _level.PlayerInCombat = false;
    }

    private static string FormatPlayerAttack(AttackOutcome r)
    {
        if (r.Dodged) return Messages.TheEnemyDodgedYourAttack;

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
