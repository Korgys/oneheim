namespace Roguelike.Core.Game.GameLoop;

/// <summary>
/// Logical input action independent from any device.
/// </summary>
public enum PlayerActionType
{
    None,
    Move,
    Choice,     // Choice1/2/3 in dialogues/shops
    Interact,   // Talk / Use
    Wait,
    Quit
}
