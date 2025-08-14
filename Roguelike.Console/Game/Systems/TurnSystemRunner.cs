namespace Roguelike.Console.Game.Systems;

public sealed class TurnSystemRunner
{
    private readonly List<ITurnSystem> _systems = new();

    public void Register(ITurnSystem system) => _systems.Add(system);

    public IEnumerable<string> Run(TurnPhase phase, TurnContext ctx)
    {
        var msgs = new List<string>();
        foreach (var sys in _systems.Where(s => s.Phase == phase))
        {
            sys.Update(ctx);
            if (!string.IsNullOrWhiteSpace(sys.LastMessage))
                msgs.Add(sys.LastMessage!);
        }
        return msgs;
    }
}
