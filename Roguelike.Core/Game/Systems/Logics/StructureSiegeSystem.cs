using Roguelike.Core.Game.Characters.Enemies;
using Roguelike.Core.Game.Characters.Enemies.Bosses;
using Roguelike.Core.Game.Levels;
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Systems.Logics;

public sealed class StructureSiegeSystem : ITurnSystem
{
    public TurnPhase Phase => TurnPhase.BeforeEnemiesMove;
    public string? LastMessage { get; private set; }

    private readonly HashSet<Enemy> _lastAttackers = new();

    public IReadOnlyCollection<Enemy> LastAttackers => _lastAttackers;

    public void Update(TurnContext ctx)
    {
        LastMessage = null;
        _lastAttackers.Clear();

        var level = ctx.Level;
        var structure = level.Structures.FirstOrDefault(s => s.Name == Messages.BaseCamp);
        if (structure == null) return;
        if (level.Player.Steps <= 175) return; // No siege before 175 steps

        // Collect attackers (adjacent to walls, not inside)
        var walls = structure.WallTiles().ToArray();
        foreach (var e in level.Enemies)
        {
            if (structure.IsInterior(e.X, e.Y)) continue;
            if (IsAdjacentToAnyWall((e.X, e.Y), walls))
                _lastAttackers.Add(e);
        }

        // Rule: ≥4 attackers OR any boss
        if (_lastAttackers.Count >= 4 || _lastAttackers.Any(a => a is Boss))
        {
            int damage = Math.Max(1, _lastAttackers.Sum(a => a.Strength) / 8);
            structure.TakeDamage(damage);

            if (structure.Hp <= 0)
            {
                level.Structures.Remove(structure);
                LastMessage = string.Format(Messages.HasBeenDestroy, structure.Name);
                _lastAttackers.Clear();
            }
            else
                LastMessage = string.Format(Messages.StructureUnderAttack, structure.Name, structure.Hp, structure.MaxHp);
        }
    }

    public bool IsPlayerSheltered(LevelManager level) =>
        level.Structures.Any(s => s.IsInterior(level.Player.X, level.Player.Y) && !s.IsSeverelyEndomaged());

    private static bool IsAdjacentToAnyWall((int x, int y) p, (int x, int y)[] walls)
    {
        foreach (var w in walls)
            if (Math.Abs(p.x - w.x) + Math.Abs(p.y - w.y) == 1) return true;
        return false;
    }
}
