using Roguelike.Console.Configuration;
using Roguelike.Console.Game.Characters.Enemies.Bosses;
using Roguelike.Console.Game.Levels;
using Roguelike.Console.Properties.i18n;

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

        // Intro nodes
        var firstIntro = new DialogueNode
        {
            Text = () => Messages.ArminFirstMeeting
        };
        var returningIntro = new DialogueNode
        {
            Text = () => Messages.ArminMeeting
        };
        var firstBossComingIntro = new DialogueNode
        {
            Text = () => Messages.ArminMeetingFirstBossComing
        };
        var firstBossHereIntro = new DialogueNode
        {
            Text = () => Messages.ArminMeetingFirstBossHere
        };
        var firstBossDefeatedIntro = new DialogueNode
        {
            Text = () => Messages.ArminMeetingFirstBossDefeated
        };
        var playerLowLifeIntro = new DialogueNode
        {
            Text = () => Messages.ArminMeetingPlayerLowLife
        };
        var playerRichIntro = new DialogueNode
        {
            Text = () => Messages.ArminMeetingPlayerRich
        };
        var baseCampUnderAttackIntro = new DialogueNode
        {
            Text = () => Messages.ArminMeetingBaseCampUnderAttack
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
                if (baseCamp == null) return Messages.NothingToRepair;
                int missing = baseCamp.MaxHp - baseCamp.Hp;
                return missing <= 0
                    ? Messages.TheCampIsAlreadyFullyRepair
                    : $"I can repair the camp for {repairCost} gold (+{repairAmount} HP).";
            }
        };
        repairNode.Options.Add(new DialogueOption
        {
            Label = Messages.PayAndRepair,
            Action = () =>
            {
                if (baseCamp == null) return Messages.NoCampToRepair;
                if (baseCamp.Hp >= baseCamp.MaxHp) return Messages.NoCampToRepair;
                if (player.Gold < repairCost) return Messages.NotEnoughGold;

                player.Gold -= repairCost;
                int before = baseCamp.Hp;
                baseCamp.Hp += Math.Min(baseCamp.MaxHp, repairAmount); // negative damage = repair
                int repaired = baseCamp.Hp - before;
                return $"Repaired camp for +{repaired} HP (cost {repairCost} gold).";
            },
            Next = returningIntro
        });
        repairNode.Options.Add(new DialogueOption { Label = Messages.MaybeLater, Next = returningIntro });

        // First intro options
        firstIntro.Options.Add(new DialogueOption { Label = Messages.CanYouHealMe, Next = healNode });
        firstIntro.Options.Add(new DialogueOption { Label = Messages.CanYouRepairTheCamp, Next = repairNode });
        firstIntro.Options.Add(new DialogueOption { Label = Messages.Goodbye, Next = null });

        // Returning intro options (slightly different flavor)
        returningIntro.Options.Add(new DialogueOption { Label = Messages.NeedHealing, Next = healNode });
        returningIntro.Options.Add(new DialogueOption { Label = Messages.RepairTheCamp, Next = repairNode });
        returningIntro.Options.Add(new DialogueOption { Label = Messages.Goodbye, Next = null });

        // Choose the proper root depending on NPC memory
        if (npc.HasMet)
        {
            if (player.GetLifeRatio() <= 0.2)
            {
                npc.Root = playerLowLifeIntro;
            }
            else if (player.Gold > 1000)
            {
                npc.Root = playerRichIntro;
            }
            else if (player.Steps <= 370)
            {
                npc.Root = returningIntro;
            }
            else if (player.Steps > 370 && player.Steps < 500)
            {
                npc.Root = firstBossComingIntro;
            }
            else if (player.Steps >= 500 && level.Enemies.Any(e => e is Boss))
            {
                npc.Root = firstBossHereIntro;
            }
            else if (player.Steps >= 500 && !level.Enemies.Any(e => e is Boss))
            {
                npc.Root = firstBossDefeatedIntro;
            }
            else
            {
                npc.Root = returningIntro;
            }
        }
        else // First time meeting
        {
            npc.Root = firstIntro;
        }
    }
}

