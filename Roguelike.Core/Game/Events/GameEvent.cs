namespace Roguelike.Core.Game.Events;

using Roguelike.Core.Game.Characters.Enemies;

public abstract record GameEvent;

public record TextMessage(string Message) : GameEvent;
public record EntityMoved(int Id, int X, int Y) : GameEvent;
public record CombatStarted(Enemy Enemy) : GameEvent;
public record StructureDamaged(string Name, int Hp, int MaxHp) : GameEvent;

public interface IEventSink { void Publish(GameEvent e); }

public sealed class EventBus : IEventSink
{
    private readonly List<GameEvent> _buffer = new();
    public void Publish(GameEvent e) => _buffer.Add(e);
    public IReadOnlyList<GameEvent> Drain() { var copy = _buffer.ToList(); _buffer.Clear(); return copy; }
}

