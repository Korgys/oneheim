namespace Roguelike.Core.Game.Systems;

public interface ITurnSystem
{
    TurnPhase Phase { get; }
    string? LastMessage { get; }          // expose a short message for UI
    void Update(TurnContext ctx);         // run once per turn (phase)
}
