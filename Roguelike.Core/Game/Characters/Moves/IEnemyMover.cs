using Roguelike.Core.Game.Characters.Enemies;
using Roguelike.Core.Game.Characters.Players;

namespace Roguelike.Core.Game.Characters.Moves;

/// <summary>
/// Moves one enemy one step toward the player using the distance field (greedy descent),
/// with target reservations to prevent stacking.
/// </summary>
public interface IEnemyMover
{
    bool TryStepTowardPlayer(
        Enemy enemy,
        Player player,
        int[,] dist,
        int gridWidth,
        int gridHeight,
        HashSet<int> staticBlocked,
        HashSet<int> occupiedNow,
        HashSet<int> reservedTargets,
        List<Enemy> combatQueue);
}
