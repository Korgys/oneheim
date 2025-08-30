using Roguelike.Core.Game.Abstractions;

namespace Roguelike.Console.Rendering;

public sealed class SystemClock : IClock { public void Delay(int ms) => Thread.Sleep(ms); }
