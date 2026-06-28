using Roguelike.Core.Game.Levels;
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.NPCs.Dialogues.Texts;

public static partial class NpcDialogues
{
    public static void BuildForYlva(Npc npc, LevelManager level)
    {
        var player = level.Player;

        int GetCost() => 120 + npc.TimesTalked * 35;

        DialogueNode Node(Func<string> text) => new() { Text = text };

        DialogueNode? mainMenu = null;
        mainMenu = Node(() => npc.HasMet ? Messages.Get("YlvaIntroReturning") : Messages.Get("YlvaIntroFirst"));

        mainMenu.Options.Add(new DialogueOption
        {
            LabelFactory = () => Messages.Format("UpgradeFirstItemForGold", GetCost()),
            Action = () =>
            {
                var item = player.Inventory.FirstOrDefault(i => i.UpgradableIncrementValue > 0);
                if (item == null) return Messages.Get("NoUpgradeableItem");

                int cost = GetCost();
                if (player.Gold < cost) return Messages.YouDoNotHaveEnoughGold;

                player.Gold -= cost;
                item.Value += item.UpgradableIncrementValue;
                return Messages.Format("YlvaUpgradedItem", item.Name, item.Value);
            },
            Next = mainMenu
        });

        mainMenu.Options.Add(new DialogueOption { Label = Messages.Goodbye, Next = null });
        npc.Root = mainMenu;
    }
}
