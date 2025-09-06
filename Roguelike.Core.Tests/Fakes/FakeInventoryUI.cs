using Roguelike.Core.Configuration;
using Roguelike.Core.Game.Abstractions;
using Roguelike.Core.Game.Characters.Players;
using Roguelike.Core.Game.Collectables.Items;

namespace Roguelike.Core.Tests.Fakes;

public sealed class FakeInventoryUI : IInventoryUI
{
    public int PromptDropIndex(Player player, Item newItem, GameSettings settings)
    {
        return 0;
    }

    public void Show(object player) { }
}
