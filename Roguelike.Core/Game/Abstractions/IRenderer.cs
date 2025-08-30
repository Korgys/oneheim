using Roguelike.Core.Game.GameLoop;

namespace Roguelike.Core.Game.Abstractions;

public interface IRenderer
{
    void RenderFrame(GameStateView view);
    void ShowMessages(IEnumerable<string> messages);
}
