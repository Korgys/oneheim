namespace Roguelike.Core.Game.Characters.Moves;

/// <summary>
/// Builds a distance field from the player using BFS, considering static blockers and current enemy occupancy.
/// </summary>
public interface IDistanceField
{
    int[,] Build(int w, int h, int playerX, int playerY, HashSet<int> staticBlocked, HashSet<int> occupiedEnemies);
}
