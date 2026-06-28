using Roguelike.Core.Game.Characters.Enemies;
using Roguelike.Core.Game.Characters.NPCs.Allies;
using Roguelike.Core.Game.Levels;
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Systems.Logics;

public sealed class MercenaryPatrolSystem : ITurnSystem
{
    public TurnPhase Phase => TurnPhase.AfterEnemiesMove;
    public string? LastMessage { get; private set; }

    public void Update(TurnContext ctx)
    {
        LastMessage = null;
        var level = ctx.Level;

        foreach (var mercenary in level.Mercenaries.ToList())
        {
            var enemy = GetAdjacentEnemy(level, mercenary.X, mercenary.Y);
            if (enemy != null)
            {
                int damage = Math.Max(1, mercenary.Strength - enemy.Armor);
                enemy.LifePoint = Math.Max(0, enemy.LifePoint - damage);

                if (enemy.LifePoint <= 0)
                {
                    level.Enemies.Remove(enemy);
                    LastMessage = Messages.Format("MercenaryHasSlainEnemy", mercenary.Name, enemy.Name);
                }
                else
                {
                    int counterDmg = Math.Max(1, enemy.Strength - mercenary.Armor);
                    mercenary.LifePoint = Math.Max(0, mercenary.LifePoint - counterDmg);
                    if (mercenary.LifePoint <= 0)
                    {
                        level.Mercenaries.Remove(mercenary);
                        LastMessage = Messages.Format("MercenaryWasSlainByEnemy", mercenary.Name, enemy.Name);
                    }
                }

                continue;
            }

            var seen = level.Enemies
                .Where(e => Math.Abs(e.X - mercenary.X) <= mercenary.Vision && Math.Abs(e.Y - mercenary.Y) <= mercenary.Vision)
                .OrderBy(e => Math.Abs(e.X - mercenary.X) + Math.Abs(e.Y - mercenary.Y))
                .FirstOrDefault();

            if (seen != null)
            {
                MoveTowards(level, mercenary, seen.X, seen.Y);
                continue;
            }

            PatrolPerimeter(level, mercenary);
        }
    }

    private static Enemy? GetAdjacentEnemy(LevelManager level, int x, int y)
    {
        foreach (var e in level.Enemies)
        {
            int dx = Math.Abs(e.X - x);
            int dy = Math.Abs(e.Y - y);
            if (dx + dy == 1) return e;
        }
        return null;
    }

    private static void MoveTowards(LevelManager level, Mercenary m, int tx, int ty)
    {
        int dx = Math.Sign(tx - m.X);
        int dy = Math.Sign(ty - m.Y);

        int nx = m.X;
        int ny = m.Y;
        if (Math.Abs(tx - m.X) >= Math.Abs(ty - m.Y)) nx += dx;
        else ny += dy;

        if (CanMove(level, nx, ny)) { m.X = nx; m.Y = ny; }
    }

    private static void PatrolPerimeter(LevelManager level, Mercenary m)
    {
        var dirs = new (int dx, int dy)[] { (1, 0), (0, 1), (-1, 0), (0, -1) };
        foreach (var d in dirs)
        {
            int nx = m.X + d.dx;
            int ny = m.Y + d.dy;
            if (CanMove(level, nx, ny))
            {
                m.X = nx; m.Y = ny;
                break;
            }
        }
    }

    private static bool CanMove(LevelManager level, int x, int y)
    {
        if (x <= 0 || x >= LevelManager.GridWidth - 1 || y <= 0 || y >= LevelManager.GridHeight - 1)
            return false;

        if (level.Structures.Any(s => s.IsWall(x, y) && !s.IsSeverelyEndomaged()))
            return false;

        if (level.Treasures.Any(t => t.X == x && t.Y == y)) return false;
        if (level.Mercenaries.Any(m => m.X == x && m.Y == y)) return false;
        if (level.Enemies.Any(e => e.X == x && e.Y == y)) return false;
        if (level.Player.X == x && level.Player.Y == y) return false;

        return true;
    }
}
