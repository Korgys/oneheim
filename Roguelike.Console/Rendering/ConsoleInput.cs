namespace Roguelike.Console.Rendering;

using Roguelike.Core.Abstractions;
using Roguelike.Core.Configuration;
using Roguelike.Core.Game.GameLoop;
using System;

public sealed class ConsoleInput : IInput
{
    public PlayerAction ReadAction(GameSettings settings)
    {
        var k = Console.ReadKey(true).Key.ToString().ToUpperInvariant();
        // map keys -> PlayerAction (MoveUp, MoveDown, Choice1,…)
        // …

        return PlayerAction.Up();
    }
}