using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Game.Levels;
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.NPCs.Dialogues.Texts;

public static partial class NpcDialogues
{
    public static void BuildForUrd(Npc npc, LevelManager level)
    {
        const int cost = 80;
        var player = level.Player;

        DialogueNode Node(Func<string> text) => new() { Text = text };

        DialogueNode? mainMenu = null;
        mainMenu = Node(() => npc.HasMet ? Messages.Get("UrdIntroReturning") : Messages.Get("UrdIntroFirst"));

        mainMenu.Options.Add(new DialogueOption
        {
            LabelFactory = () => Messages.Format("GambleForGold", cost),
            Action = () =>
            {
                if (player.Gold < cost) return Messages.YouDoNotHaveEnoughGold;

                player.Gold -= cost;
                if (_random.Next(100) < 45)
                    return Messages.Get("GambleLost");

                if (player.Inventory.Count >= player.MaxInventorySize)
                    return Messages.InventoryFull;

                var ids = Enum.GetValues<ItemId>();
                var item = ItemFactory.CreateItem(ids[_random.Next(ids.Length)]);
                player.Inventory.Add(item);
                return Messages.Format("GambleWonItem", item.Name);
            },
            Next = mainMenu
        });

        mainMenu.Options.Add(new DialogueOption { Label = Messages.Goodbye, Next = null });
        npc.Root = mainMenu;
    }
}
