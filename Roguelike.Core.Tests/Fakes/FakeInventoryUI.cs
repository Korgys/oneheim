using Roguelike.Core.Configuration;
using Roguelike.Core.Game.Abstractions;
using Roguelike.Core.Game.Characters.Players;
using Roguelike.Core.Game.Collectables.Items;

namespace Roguelike.Core.Tests.Fakes;

public sealed class FakeInventoryUI : IInventoryUI
{
    public void Show(object player) { }

    private readonly int _dropIndex;
    public FakeInventoryUI(int dropIndex) => _dropIndex = dropIndex;
    public int PromptDropIndex(Player player, Item newItem, GameSettings settings) => _dropIndex;
}
