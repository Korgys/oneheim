using Roguelike.Core.Game.Abstractions;
using Roguelike.Core.Game.GameLoop;

namespace Roguelike.Core.Tests.Fakes;

public sealed class FakeRenderer : IRenderer
{
    public List<GameStateView> RenderFrameCalls { get; } = new();

    public void RenderFrame(GameStateView view)
    {
        RenderFrameCalls.Add(view);
    }

    public void ShowMessages(IEnumerable<string> messages)
    {
    }
}
