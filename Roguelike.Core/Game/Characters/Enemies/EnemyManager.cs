using Roguelike.Core.Game.Abstractions;
using Roguelike.Core.Game.Characters.Players;
using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Game.Combat;
using Roguelike.Core.Game.Levels;
using Roguelike.Core.Game.Systems.Logics;
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Enemies;

public class EnemyManager
{
    private readonly LevelManager _level;
    private readonly StructureSiegeSystem _siege;
    private readonly ICombatRenderer _combatUi;
    private readonly Random _random = new();
    public string? CombatMessage { get; private set; }

    public EnemyManager(LevelManager level, StructureSiegeSystem siege, ICombatRenderer combatUi)
    {
        _level = level;
        _siege = siege;
        _combatUi = combatUi ?? new NullCombatRenderer();
    }

    public void MoveEnemies()
    {
        CombatMessage = null;

        // 1) If player is sheltered, do not move enemies (they can still siege via _siege.Update)
        if (_siege.IsPlayerSheltered(_level))
            return;

        var player = _level.Player;
        var invisibility = player.Inventory.FirstOrDefault(i => i.Id == ItemId.CapeOfInvisibility);
        var visionReduction = invisibility?.Value ?? 0;

        var enemiesToFight = new List<Enemy>();

        // 3) Move all enemies with their per-turn steps
        foreach (var enemy in _level.Enemies.ToList())
        {
            // Do not move enemies currently contributing to siege (they keep attacking walls)
            if (_siege.LastAttackers.Contains(enemy))
                continue;

            int steps = Math.Max(0, enemy.StepsPerTurn);
            for (int s = 0; s < steps; s++)
            {
                bool playerInRange =
                    Math.Abs(enemy.X - player.X) <= Math.Max(enemy.Vision - visionReduction, 2) &&
                    Math.Abs(enemy.Y - player.Y) <= Math.Max(enemy.Vision - visionReduction, 2);

                if (playerInRange)
                {
                    if (!TryStepTowardPlayer(enemy, player, enemiesToFight))
                        break; // stop if blocked or engaged
                }
                else
                {
                    if (!TryStepRandom(enemy, enemiesToFight))
                        break;
                }
            }
        }

        // 4) Resolve queued combats
        foreach (var enemy in enemiesToFight.Distinct())
        {
            // Only fight if player is alive
            if (player.LifePoint > 0)
            {
                var combat = new CombatManager(_level, _combatUi);
                combat.StartCombat(enemy);
            }

            // Combat result
            if (player.LifePoint <= 0)
            {
                CombatMessage = string.Format(Messages.YouWereKilledBy, enemy.Name);
                break;
            }
            else
            {
                CombatMessage = Messages.YouWereAttackedAndDefeatedYourEnemy;
            }
        }
    }

    private bool TryStepTowardPlayer(Enemy enemy, Player player, List<Enemy> combatQueue)
    {
        int dx = player.X - enemy.X;
        int dy = player.Y - enemy.Y;
        int stepX = dx == 0 ? 0 : dx / Math.Abs(dx);
        int stepY = dy == 0 ? 0 : dy / Math.Abs(dy);

        int newX = enemy.X;
        int newY = enemy.Y;

        if (Math.Abs(dx) > Math.Abs(dy)) newX += stepX;
        else if (dy != 0) newY += stepY;

        if (newX == player.X && newY == player.Y)
        {
            combatQueue.Add(enemy);
            return false;
        }

        if (CanMoveTo(newX, newY))
        {
            enemy.X = newX;
            enemy.Y = newY;
            return true;
        }

        return false;
    }

    private bool TryStepRandom(Enemy enemy, List<Enemy> combatQueue)
    {
        int direction = _random.Next(4);
        int newX = enemy.X;
        int newY = enemy.Y;

        switch (direction)
        {
            case 0: newY--; break;
            case 1: newY++; break;
            case 2: newX--; break;
            case 3: newX++; break;
        }

        if (newX == _level.Player.X && newY == _level.Player.Y)
        {
            combatQueue.Add(enemy);
            return false;
        }

        // Prevent enemies to move in walls or inside the structure 
        if (CanMoveTo(newX, newY) 
            && _level.Structures.Any(s => !s.IsSeverelyEndomaged() && !s.IsInterior(newX, newY)))
        {
            enemy.X = newX;
            enemy.Y = newY;
            return true;
        }

        return false;
    }

    private bool CanMoveTo(int x, int y)
    {
        if (x <= 0 || x >= LevelManager.GridWidth - 1 || y <= 0 || y >= LevelManager.GridHeight - 1)
            return false;

        // walls are blocked until they are at 10% HP
        if (_level.Structures.Any(s => s.IsWall(x, y) && ((double)s.Hp / (double)s.MaxHp) >= 0.1))
            return false;

        // do not move onto treasures, enemies, or mercenaries
        return !_level.Treasures.Any(t => t.X == x && t.Y == y)
            && !_level.Enemies.Any(e => e.X == x && e.Y == y)
            && !_level.Mercenaries.Any(e => e.X == x && e.Y == y);
    }
}
