using Roguelike.Console.Configuration;
using Roguelike.Console.Game.Levels;

namespace Roguelike.Console.Game.Characters.NPCs.Dialogues;

public static class NpcDialogues
{
    // Build (or rebuild) the dialogue tree for a given NPC and current game context
    public static void BuildForArmin(Npc npc, LevelManager level, GameSettings settings)
    {
        var player = level.Player;

        // Costs and effects
        const int healCost = 15;
        const int healAmount = 5;

        const int repairCost = 100;
        const int repairAmount = 100;

        // Find your first base (or null if none)
        var baseCamp = level.Structures.FirstOrDefault();

        // Intro nodes (first time vs returning)
        var firstIntro = new DialogueNode
        {
            Text = () => "Greetings, traveler. You made it to the camp. How can I help?"
        };

        var returningIntro = new DialogueNode
        {
            Text = () => "Back again? The wilds look harsh today."
        };

        // Action: heal player for gold
        var healNode = new DialogueNode
        {
            Text = () => $"I can patch you up for {healCost} gold. You will recover up to +{healAmount} HP."
        };
        healNode.Options.Add(new DialogueOption
        {
            Label = "Pay and heal",
            Action = () =>
            {
                if (player.Gold < healCost) return "You do not have enough gold.";
                if (player.LifePoint >= player.MaxLifePoint) return "You are already at full health.";

                player.Gold -= healCost;
                int before = player.LifePoint;
                player.LifePoint = Math.Min(player.MaxLifePoint, player.LifePoint + healAmount);
                int healed = player.LifePoint - before;
                return $"Healed {healed} HP for {healCost} gold.";
            },
            Next = returningIntro
        });
        healNode.Options.Add(new DialogueOption { Label = "Maybe later.", Next = returningIntro });

        // Action: repair base for gold
        var repairNode = new DialogueNode
        {
            Text = () =>
            {
                if (baseCamp == null) return "There is nothing to repair here.";
                int missing = baseCamp.MaxHp - baseCamp.Hp;
                return missing <= 0
                    ? "The camp is already fully repaired."
                    : $"I can repair the camp for {repairCost} gold (+{repairAmount} HP).";
            }
        };
        repairNode.Options.Add(new DialogueOption
        {
            Label = "Pay and repair",
            Action = () =>
            {
                if (baseCamp == null) return "No camp to repair.";
                if (baseCamp.Hp >= baseCamp.MaxHp) return "The camp is already in perfect shape.";
                if (player.Gold < repairCost) return "You do not have enough gold.";

                player.Gold -= repairCost;
                int before = baseCamp.Hp;
                baseCamp.Hp += Math.Min(baseCamp.MaxHp, repairAmount); // negative damage = repair
                int repaired = baseCamp.Hp - before;
                return $"Repaired camp for +{repaired} HP (cost {repairCost} gold).";
            },
            Next = returningIntro
        });
        repairNode.Options.Add(new DialogueOption { Label = "Maybe later.", Next = returningIntro });

        // First intro options
        firstIntro.Options.Add(new DialogueOption { Label = "Can you heal me?", Next = healNode });
        firstIntro.Options.Add(new DialogueOption { Label = "Can you repair the camp?", Next = repairNode });
        firstIntro.Options.Add(new DialogueOption { Label = "Goodbye.", Next = null });

        // Returning intro options (slightly different flavor)
        returningIntro.Options.Add(new DialogueOption { Label = "I need healing.", Next = healNode });
        returningIntro.Options.Add(new DialogueOption { Label = "Repair the camp.", Next = repairNode });
        returningIntro.Options.Add(new DialogueOption { Label = "Goodbye.", Next = null });

        // Choose the proper root depending on NPC memory
        npc.Root = npc.HasMet ? returningIntro : firstIntro;
    }
}

