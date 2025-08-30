using Roguelike.Core.Configuration;
using Roguelike.Core.Game.GameLoop;

namespace Roguelike.Core.Abstractions;
public interface IInput
{
    // Return one logical action by tick (MoveUp, Choice1, Quit, etc.)
    PlayerAction ReadAction(GameSettings settings);
}