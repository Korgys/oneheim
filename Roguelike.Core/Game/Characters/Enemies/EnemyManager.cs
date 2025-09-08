using Roguelike.Core.Game.Abstractions;
using Roguelike.Core.Game.Characters.Moves;
using Roguelike.Core.Game.Characters.Players;
using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Game.Combat;
using Roguelike.Core.Game.Levels;
using Roguelike.Core.Game.Systems.Logics;
using Roguelike.Core.Properties.i18n;
using System.Runtime.CompilerServices;

namespace Roguelike.Core.Game.Characters.Enemies;

public sealed class EnemyManager
{
    private readonly LevelManager _level;
    private readonly StructureSiegeSystem _siege;
    private readonly ICombatRenderer _combatUi;
    private readonly IWalkability _walk;
    private readonly IDistanceField _path;
    private readonly IEnemyMover _mover;
    private readonly Random _rng = new();

    public string? CombatMessage { get; private set; }

    public EnemyManager(
        LevelManager level,
        StructureSiegeSystem siege,
        ICombatRenderer combatUi)
    {
        _level = level;
        _siege = siege;
        _combatUi = combatUi ?? new NullCombatRenderer();

        // default collaborators
        _walk = new DefaultWalkability(level);
        _path = new BfsDistanceField();
        _mover = new GreedyMover();
    }

    public void MoveEnemies()
    {
        CombatMessage = null;

        if (_siege.IsPlayerSheltered(_level))
            return;

        var player = _level.Player;

        // Copy enemies once to avoid collection mutation surprises during the loop
        var enemies = _level.Enemies as IList<Enemy> ?? _level.Enemies.ToList();
        if (!enemies.Any()) return;

        // Visibility (Cape)
        var invisibility = player.Inventory.FirstOrDefault(i => i.Id == ItemId.CapeOfInvisibility);
        var visionReduction = invisibility?.Value ?? 0;

        // Occupancy (enemies only) for this tick
        var occupied = new HashSet<int>(enemies.Count);
        foreach (var e in enemies) occupied.Add(Key(e.X, e.Y));

        // Prebuild static blockers (walls/interiors/treasures/mercs) snapshot for this turn
        var staticBlocked = _walk.BuildStaticBlockedSet();

        // Distance field from player using static blockers + current enemy occupancy
        var dist = _path.Build(LevelManager.GridWidth, LevelManager.GridHeight, player.X, player.Y,
                               staticBlocked, occupied);

        var reserved = new HashSet<int>();        // target reservations this tick
        var toFight = new List<Enemy>(capacity: 8);

        foreach (var enemy in enemies)
        {
            if (_siege.LastAttackers.Contains(enemy))
                continue;

            int steps = Math.Max(0, enemy.StepsPerTurn);

            for (int s = 0; s < steps; s++)
            {
                bool inRange =
                    Math.Abs(enemy.X - player.X) <= Math.Max(enemy.Vision - visionReduction, 1) &&
                    Math.Abs(enemy.Y - player.Y) <= Math.Max(enemy.Vision - visionReduction, 1);

                if (inRange)
                {
                    if (!_mover.TryStepTowardPlayer(enemy, player, dist,
                        LevelManager.GridWidth, LevelManager.GridHeight,
                        staticBlocked, occupied, reserved, toFight))
                        break;
                }
                else
                {
                    if (!TryStepRandom(enemy, occupied, staticBlocked, toFight))
                        break;
                }
            }
        }

        // Resolve queued combats
        foreach (var enemy in toFight.Distinct())
        {
            if (player.LifePoint > 0)
            {
                var combat = new CombatManager(_level, _combatUi);
                combat.StartCombat(enemy);
            }

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

    private bool TryStepRandom(Enemy enemy, HashSet<int> occupied, HashSet<int> staticBlocked, List<Enemy> combatQueue)
    {
        int dir = _rng.Next(4);
        int nx = enemy.X, ny = enemy.Y;
        switch (dir)
        {
            case 0: ny--; break;
            case 1: ny++; break;
            case 2: nx--; break;
            case 3: nx++; break;
        }

        // engage if bumping the player
        if (nx == _level.Player.X && ny == _level.Player.Y)
        {
            combatQueue.Add(enemy);
            return false;
        }

        // cannot step into static blockers or occupied by other enemies
        int key = Key(nx, ny);
        if (!IsInside(nx, ny)) return false;
        if (staticBlocked.Contains(key)) return false;
        if (occupied.Contains(key)) return false;

        // move and update occupancy
        occupied.Remove(Key(enemy.X, enemy.Y));
        occupied.Add(key);
        enemy.X = nx; enemy.Y = ny;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Key(int x, int y) => (y << 16) | (x & 0xFFFF);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsInside(int x, int y)
        => x > 0 && y > 0; // caller already knows grid bounds
}
