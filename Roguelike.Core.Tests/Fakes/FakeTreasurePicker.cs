using Roguelike.Core.Game.Abstractions;

namespace Roguelike.Core.Tests.Fakes;

public sealed class FakeTreasurePicker : ITreasurePicker
{
    public int Pick(TreasurePickerContext context, IReadOnlyList<TreasureOptionView> options)
    {
        return 0;
    }
}
