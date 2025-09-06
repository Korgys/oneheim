using Roguelike.Core.Game.Abstractions;

namespace Roguelike.Core.Tests.Fakes;

public sealed class FakeClock : IClock
{
    public int TotalDelayMs { get; private set; }

    public void Delay(int ms)
    {
        TotalDelayMs += ms;
    }
}