using Roguelike.Core.Game.GameLoop;

namespace Roguelike.Core.Game.Abstractions;

public interface IStorage
{
    Task SaveAsync(string slot, GameSnapshot snapshot);
    Task<GameSnapshot?> LoadAsync(string slot);
}