using Roguelike.Core.Game.Abstractions;

namespace Roguelike.Core.Tests.Fakes;

public sealed class FakeTreasurePicker : ITreasurePicker
{
    private readonly int _toReturn;
    public TreasurePickerContext? LastContext { get; private set; }
    public IReadOnlyList<TreasureOptionView>? LastViews { get; private set; }

    public FakeTreasurePicker(int toReturn) => _toReturn = toReturn;

    public int Pick(TreasurePickerContext context, IReadOnlyList<TreasureOptionView> options)
    {
        LastContext = context;
        LastViews = options;
        return _toReturn;
    }
}
