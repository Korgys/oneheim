using Roguelike.Console.Configuration;
using Roguelike.Console.Game.Characters.Enemies.Bosses;
using Roguelike.Console.Game.Levels;
using Roguelike.Console.Properties.i18n;

namespace Roguelike.Console.Game.Characters.NPCs.Dialogues;

public static class NpcDialogues
{
    public static void BuildForArmin(Npc npc, LevelManager level, GameSettings settings)
    {
        var player = level.Player;

        // Costs and effects
        const int healCost = 15;
        const int healAmount = 5;
        const int repairCost = 100;
        const int repairAmount = 100;

        var baseCamp = level.Structures.FirstOrDefault();
        bool hasBoss = level.Enemies.Any(e => e is Boss);
        int steps = player.Steps;


        DialogueNode Intro(Func<string> textFactory) => new DialogueNode { Text = textFactory };

        void AddStandardOptions(DialogueNode node, DialogueNode heal, DialogueNode repair, DialogueNode? goodbyeNext = null)
        {
            node.Options.Add(new DialogueOption { Label = Messages.NeedHealing, Next = heal });
            node.Options.Add(new DialogueOption { Label = Messages.RepairTheCamp, Next = repair });
            node.Options.Add(new DialogueOption { Label = Messages.Goodbye, Next = goodbyeNext });
        }

        DialogueNode MakeHealNode(DialogueNode backNode)
        {
            var healNode = new DialogueNode
            {
                Text = () => string.Format(Messages.ArminHealPitch, healCost, healAmount)
            };

            healNode.Options.Add(new DialogueOption
            {
                Label = Messages.PayAndHeal,
                Action = () =>
                {
                    if (player.Gold < healCost) return Messages.NotEnoughGold;
                    if (player.LifePoint >= player.MaxLifePoint) return Messages.AlreadyFullHealth;

                    player.Gold -= healCost;
                    int before = player.LifePoint;
                    player.LifePoint = Math.Min(player.MaxLifePoint, player.LifePoint + healAmount);
                    int healed = player.LifePoint - before;
                    return string.Format(Messages.HealedHpForGold, healed, healCost);
                },
                Next = backNode
            });

            healNode.Options.Add(new DialogueOption { Label = Messages.MaybeLater, Next = backNode });
            return healNode;
        }

        DialogueNode MakeRepairNode(DialogueNode backNode)
        {
            var repairNode = new DialogueNode
            {
                Text = () =>
                {
                    if (baseCamp is null) return Messages.NothingToRepair;
                    int missing = baseCamp.MaxHp - baseCamp.Hp;
                    return missing <= 0
                        ? Messages.TheCampIsAlreadyFullyRepair
                        : string.Format(Messages.ArminRepairPitch, repairCost, repairAmount);
                }
            };

            repairNode.Options.Add(new DialogueOption
            {
                Label = Messages.PayAndRepair,
                Action = () =>
                {
                    if (baseCamp is null) return Messages.NoCampToRepair;
                    if (baseCamp.Hp >= baseCamp.MaxHp) return Messages.NoCampToRepair;
                    if (player.Gold < repairCost) return Messages.NotEnoughGold;

                    player.Gold -= repairCost;
                    int before = baseCamp.Hp;
                    baseCamp.Hp = Math.Min(baseCamp.MaxHp, baseCamp.Hp + repairAmount); // clamp correct
                    int repaired = baseCamp.Hp - before;
                    return string.Format(Messages.RepairedCampForHpCost, repaired, repairCost);
                },
                Next = backNode
            });

            repairNode.Options.Add(new DialogueOption { Label = Messages.MaybeLater, Next = backNode });
            return repairNode;
        }

        // Nodes
        var firstIntro = Intro(() => Messages.ArminFirstMeeting);
        var returningIntro = Intro(() => Messages.ArminMeeting);
        var firstBossComingIntro = Intro(() => Messages.ArminMeetingFirstBossComing);
        var firstBossHereIntro = Intro(() => Messages.ArminMeetingFirstBossHere);
        var firstBossDefeatedIntro = Intro(() => Messages.ArminMeetingFirstBossDefeated);
        var playerLowLifeIntro = Intro(() => Messages.ArminMeetingPlayerLowLife);
        var playerRichIntro = Intro(() => Messages.ArminMeetingPlayerRich);
        var baseCampUnderAttackIntro = Intro(() => Messages.ArminMeetingBaseCampUnderAttack);

        DialogueNode PickRoot()
        {
            if (!npc.HasMet) return firstIntro;

            // Priorités de contexte (ex. low life > rich > boss states > default)
            if (baseCamp != null && level.IsBaseCampUnderAttack()) return baseCampUnderAttackIntro;
            if (player.GetLifeRatio() <= 0.20) return playerLowLifeIntro;
            if (player.Gold > 1000) return playerRichIntro;

            // Étapes/boss
            if (steps <= 370) return returningIntro;
            if (steps is > 370 and < 500) return firstBossComingIntro;
            if (steps >= 500 && hasBoss) return firstBossHereIntro;
            if (steps >= 500 && !hasBoss) return firstBossDefeatedIntro;

            return returningIntro;
        }

        // Les nœuds d’action doivent renvoyer vers leur "écran" d’origine
        // On fabrique d’abord les intros, puis on crée les nœuds d’action en les pointant vers returningIntro
        var healNode = MakeHealNode(returningIntro);
        var repairNode = MakeRepairNode(returningIntro);

        // Ajout des 3 mêmes options à tous les écrans d’intro
        AddStandardOptions(firstIntro, healNode, repairNode);
        AddStandardOptions(returningIntro, healNode, repairNode);
        AddStandardOptions(firstBossComingIntro, healNode, repairNode);
        AddStandardOptions(firstBossHereIntro, healNode, repairNode);
        AddStandardOptions(firstBossDefeatedIntro, healNode, repairNode);
        AddStandardOptions(playerLowLifeIntro, healNode, repairNode);
        AddStandardOptions(playerRichIntro, healNode, repairNode);
        AddStandardOptions(baseCampUnderAttackIntro, healNode, repairNode);

        // Finale selection
        npc.Root = PickRoot();
    }
}