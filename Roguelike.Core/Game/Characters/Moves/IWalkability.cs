namespace Roguelike.Core.Game.Characters.Moves;

/// <summary>
/// Decides which tiles are statically blocked for enemies in this turn (walls, interiors, treasures, mercs).
/// Produces a snapshot HashSet<int> for O(1) lookups during BFS and movement.
/// </summary>
public interface IWalkability
{
    HashSet<int> BuildStaticBlockedSet();
}