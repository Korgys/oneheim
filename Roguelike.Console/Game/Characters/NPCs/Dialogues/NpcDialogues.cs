using Roguelike.Console.Configuration;
using Roguelike.Console.Game.Characters.Enemies.Bosses;
using Roguelike.Console.Game.Collectables;
using Roguelike.Console.Game.Collectables.Items;
using Roguelike.Console.Game.Levels;
using Roguelike.Console.Properties.i18n;
using System;

namespace Roguelike.Console.Game.Characters.NPCs.Dialogues;

public static class NpcDialogues
{
    private static Random _random = new Random();

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

    public static void BuildForIchem(Npc npc, LevelManager level, GameSettings settings)
    {
        var player = level.Player;
        int price = 50; // chest price

        // Fidelity card item logic
        var fidelityCard = player.Inventory.FirstOrDefault(i => i.Id == ItemId.FidelityCard);
        if (fidelityCard != null)
        {
            float discountRate = fidelityCard.Value / 100f;
            price = (int)MathF.Round(price * (1 - discountRate));
        }

        // Helpers
        DialogueNode Node(Func<string> text) => new DialogueNode { Text = text };

        // Small talks
        string[] smallTalkLines =
        {
            "These lands change you. Sometimes for the better.",
            "Gold comes and goes. Choices linger.",
            "I once sold a boon that saved a kingdom. Or so they say.",
            "Storm’s coming. You can feel it in the stone.",
            "Power is a weight; spend it wisely."
        };
        var talk = Node(() => smallTalkLines[_random.Next(smallTalkLines.Length)]);
        talk.Options.Add(new DialogueOption { Label = "Back", Next = null });

        DialogueNode? mainMenu = null;

        // --- Menu principal (3 options : acheter / discuter / quitter) ---
        mainMenu = Node(() =>
        {
            var intro = npc.HasMet
                ? "Back again, wanderer?"
                : "Greetings, wanderer.";
            return $"{intro} Care to buy a boon for {price} gold?";
        });

        mainMenu.Options.Add(new DialogueOption
        {
            Label = $"Buy a boon ({price} gold)",
            Action = () =>
            {
                // Check if player has enough gold
                if (player.Gold < price) return "You do not have enough gold.";

                // 3 choices like treasure chest
                var choices = TreasureSelector.GenerateBonusChoices(player, settings);
                var chosen = TreasureSelector.PromptPlayerForBonus(choices, player, settings);

                // Pay and apply bonus
                player.Gold -= price;
                var msg = TreasureSelector.ApplyBonus(chosen, player, settings);
                return $"Purchased: {msg}";
            },
            Next = mainMenu // go back to main menu
        });
        mainMenu.Options.Add(new DialogueOption
        {
            Label = "Just chat",
            Next = talk
        });
        mainMenu.Options.Add(new DialogueOption
        {
            Label = "Goodbye",
            Next = null
        });

        // Root directement sur le menu principal
        npc.Root = mainMenu;
    }
}