using Roguelike.Core.Game.Abstractions;

namespace Roguelike.Console.Rendering;

public sealed class DefaultRng : IRng
{
    private readonly Random _r = new();
    public int Next(int min, int max) => _r.Next(min, max);
    public double NextDouble() => _r.NextDouble();
}
