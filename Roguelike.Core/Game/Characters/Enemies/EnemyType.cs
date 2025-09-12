namespace Roguelike.Core.Game.Characters.Enemies;

/// <summary>
/// Represents the various types of enemies encountered in the game.
/// </summary>
/// <remarks>This enumeration categorizes enemies into distinct types, which can be used to determine their
/// behavior, strengths, weaknesses, or other gameplay mechanics.</remarks>
public enum EnemyType
{
    Unknown, // Default value, should not be used in practice.
    Undead,
    Wild,
    Outlaws,
    Cultist,
    Demon
}
