namespace Roguelike.Core.Game.Abstractions;

using Roguelike.Core.Game.Characters.Players;
using Roguelike.Core.Game.Collectables;
using Roguelike.Core.Game.Collectables.Items;

public interface ITreasurePicker
{
    /// <summary>
    /// Ask the presentation layer to let the user pick one treasure.
    /// Return the chosen index in options (0..n-1). Must be a valid index.
    /// </summary>
    int Pick(TreasurePickerContext context, IReadOnlyList<TreasureOptionView> options);
}

/// <summary>
/// Minimal context for renderers (you can extend later).
/// </summary>
public sealed class TreasurePickerContext
{
    public required Player Player { get; init; }
    public string? Title { get; init; } // e.g. "You found a treasure!"
}

/// <summary>
/// A UI-ready description for a treasure choice (no colors, no Console).
/// </summary>
public sealed class TreasureOptionView
{
    public required Treasure Treasure { get; init; }
    public required string Description { get; init; }
    public required ItemRarity Rarity { get; init; }
}
