using Roguelike.Core.Game.Characters.Enemies;
using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Game.Levels;
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.NPCs.Dialogues.Texts;

public static partial class NpcDialogues
{
    public static void BuildForOmana(Npc npc, LevelManager level)
    {
        DialogueNode Node(Func<string> text) => new() { Text = text };

        var mainMenu = Node(() =>
        {
            var boss = level.PeekNextBoss();
            var family = Messages.Get($"EnemyType{boss.Category}");
            var weakness = GetWeaknessText(boss.Category);
            return Messages.Format("OmanaProphecy", boss.Name, family, weakness);
        });

        mainMenu.Options.Add(new DialogueOption { Label = Messages.Ok, Next = null });
        npc.Root = mainMenu;
    }

    private static string GetWeaknessText(EnemyType enemyType)
    {
        var itemNames = ItemIdHelper.GetItemIdsSpecificByEnemyType(enemyType)
            .Select(id => ItemFactory.CreateItem(id).Name)
            .Take(2)
            .ToList();

        return itemNames.Count == 0
            ? Messages.Get("NoKnownWeakness")
            : string.Join(", ", itemNames);
    }
}
