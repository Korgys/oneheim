namespace Roguelike.Core.Game.GameLoop;

/// <summary>
/// Immutable input packet. Use the static helpers to build instances.
/// </summary>
public readonly record struct PlayerAction(
    PlayerActionType Type,
    int Dx = 0,
    int Dy = 0,
    int ChoiceIndex = -1)
{
    // Factories
    public static PlayerAction Move(int dx, int dy) => new(PlayerActionType.Move, dx, dy);
    public static PlayerAction Up() => Move(0, -1);
    public static PlayerAction Down() => Move(0, 1);
    public static PlayerAction Left() => Move(-1, 0);
    public static PlayerAction Right() => Move(1, 0);

    public static PlayerAction Choose(int index) => new(PlayerActionType.Choice, 0, 0, index);
    public static PlayerAction Interact() => new(PlayerActionType.Interact);
    public static PlayerAction Wait() => new(PlayerActionType.Wait);
    public static PlayerAction Quit() => new(PlayerActionType.Quit);
    public static PlayerAction None() => new(PlayerActionType.None);
}
