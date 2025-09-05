using Roguelike.Core.Configuration;
using Roguelike.Core.Game.Characters.Players;
using Roguelike.Core.Game.Collectables.Items;

namespace Roguelike.Core.Game.Abstractions;

public interface IInventoryUI
{
    int PromptDropIndex(Player player, Item newItem, GameSettings settings);
}
