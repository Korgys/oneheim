using Roguelike.Console.Configuration;
using Roguelike.Console.Game.Characters.Enemies.Bosses;
using Roguelike.Console.Game.Collectables;
using Roguelike.Console.Game.Collectables.Items;
using Roguelike.Console.Game.Levels;
using Roguelike.Console.Properties.i18n;

namespace Roguelike.Console.Game.Characters.NPCs.Dialogues;

public static class NpcDialogues
{
    private static Random _random = new();

    public static void BuildForArmin(Npc npc, LevelManager level)
    {
        var player = level.Player;
        var baseCamp = level.Structures.FirstOrDefault();
        bool hasBoss = level.Enemies.Any(e => e is Boss);
        int steps = player.Steps;

        // Cost/amount helpers (re-evaluated at runtime for dynamic values)
        int GetHealAmount()
        {
            return player.Gold > player.MaxLifePoint - player.LifePoint
                ? player.MaxLifePoint - player.LifePoint
                : player.Gold;
        }
        int GetHealCost() => GetHealAmount();
        int GetRepairAmount() => baseCamp != null ? Math.Min(player.Gold, baseCamp.MaxHp - baseCamp.Hp) : 0;
        int GetRepairCost() => GetRepairAmount();

        DialogueNode Intro(Func<string> textFactory) => new DialogueNode { Text = textFactory };

        void AddStandardOptions(DialogueNode node, DialogueNode heal, DialogueNode repair, DialogueNode? goodbyeNext = null)
        {
            node.Options.Add(new DialogueOption { Label = Messages.NeedHealing, Next = heal });
            node.Options.Add(new DialogueOption { Label = Messages.RepairTheCamp, Next = repair });
            node.Options.Add(new DialogueOption { Label = Messages.Goodbye, Next = goodbyeNext });
        }

        // ---- Build all intro nodes (never mutate their Text later)
        var firstIntro = Intro(() => Messages.ArminFirstMeeting);
        var returningIntro = Intro(() => Messages.ArminMeeting);
        var firstBossComingIntro = Intro(() => Messages.ArminMeetingFirstBossComing);
        var firstBossHereIntro = Intro(() => Messages.ArminMeetingFirstBossHere);
        var firstBossDefeatedIntro = Intro(() => Messages.ArminMeetingFirstBossDefeated);
        var playerLowLifeIntro = Intro(() => Messages.ArminMeetingPlayerLowLife);
        var playerRichIntro = Intro(() => Messages.ArminMeetingPlayerRich);
        var baseCampUnderAttackIntro = Intro(() => Messages.ArminMeetingBaseCampUnderAttack);

        // ---- Shared "after service" node that asks "what else?"
        var afterService = new DialogueNode { Text = () => Messages.WhatElseCanIDo };

        // ---- Heal node (no recursion; wire Next = afterService)
        var healNode = new DialogueNode
        {
            Text = () => player.LifePoint != player.MaxLifePoint
                ? string.Format(Messages.ArminHealPitch, GetHealCost(), GetHealAmount())
                : Messages.ArminHealPitchPlayerFullLife
        };
        if (player.LifePoint != player.MaxLifePoint)
        {
            healNode.Options.Add(new DialogueOption
            {
                Label = Messages.PayAndHeal,
                Action = () =>
                {
                    if (player.Gold < GetHealCost()) return Messages.NotEnoughGold;
                    if (player.LifePoint >= player.MaxLifePoint) return Messages.AlreadyFullHealth;

                    int healedCost = GetHealCost();
                    player.Gold -= healedCost;
                    int before = player.LifePoint;
                    player.LifePoint = Math.Min(player.MaxLifePoint, player.LifePoint + GetHealAmount());
                    int healed = player.LifePoint - before;
                    return string.Format(Messages.HealedHpForGold, healed, healedCost);
                },
                Next = afterService
            });
        }
        healNode.Options.Add(new DialogueOption { Label = Messages.MaybeLater, Next = afterService });

        // ---- Repair node (no recursion; wire Next = afterService)
        var repairNode = new DialogueNode
        {
            Text = () =>
            {
                if (baseCamp is null) return Messages.NothingToRepair;
                int missing = baseCamp.MaxHp - baseCamp.Hp;
                return missing == 0
                    ? Messages.TheCampIsAlreadyFullyRepair
                    : string.Format(Messages.ArminRepairPitch, GetRepairCost(), GetRepairAmount());
            }
        };
        if (baseCamp != null && baseCamp.MaxHp - baseCamp.Hp != 0)
        {
            repairNode.Options.Add(new DialogueOption
            {
                Label = Messages.PayAndRepair,
                Action = () =>
                {
                    if (baseCamp is null) return Messages.NoCampToRepair;
                    if (baseCamp.Hp >= baseCamp.MaxHp) return Messages.NoCampToRepair;
                    if (player.Gold < GetRepairCost()) return Messages.NotEnoughGold;

                    int repairCost = GetRepairCost();
                    player.Gold -= repairCost;
                    int before = baseCamp.Hp;
                    baseCamp.Hp = Math.Min(baseCamp.MaxHp, baseCamp.Hp + GetRepairAmount());
                    int repaired = baseCamp.Hp - before;
                    return string.Format(Messages.RepairedCampForHpCost, repaired, repairCost);
                },
                Next = afterService
            });
        }
        repairNode.Options.Add(new DialogueOption { Label = Messages.MaybeLater, Next = afterService });

        // ---- Wire "afterService" to standard menu, returning to returningIntro on "Goodbye"
        AddStandardOptions(afterService, healNode, repairNode, null);

        // ---- Add standard options to every intro screen (do NOT change their Text)
        AddStandardOptions(firstIntro, healNode, repairNode, returningIntro);
        AddStandardOptions(returningIntro, healNode, repairNode, returningIntro);
        AddStandardOptions(firstBossComingIntro, healNode, repairNode, returningIntro);
        AddStandardOptions(firstBossHereIntro, healNode, repairNode, returningIntro);
        AddStandardOptions(firstBossDefeatedIntro, healNode, repairNode, returningIntro);
        AddStandardOptions(playerLowLifeIntro, healNode, repairNode, returningIntro);
        AddStandardOptions(playerRichIntro, healNode, repairNode, returningIntro);
        AddStandardOptions(baseCampUnderAttackIntro, healNode, repairNode, returningIntro);

        // ---- Pick the intro based on context
        DialogueNode PickRoot()
        {
            if (!npc.HasMet) return firstIntro;

            if (baseCamp != null && level.IsBaseCampUnderAttack()) return baseCampUnderAttackIntro;
            if (player.GetLifeRatio() <= 0.20) return playerLowLifeIntro;
            if (player.Gold > 1000) return playerRichIntro;

            if (steps <= 370) return returningIntro;
            if (steps is > 370 and < 500) return firstBossComingIntro;
            if (steps >= 500 && hasBoss) return firstBossHereIntro;
            if (steps >= 500 && !hasBoss) return firstBossDefeatedIntro;

            return returningIntro;
        }

        npc.Root = PickRoot();
    }

    public static void BuildForIchem(Npc npc, LevelManager level, GameSettings settings)
    {
        var player = level.Player;

        int GetCurrentPrice()
        {
            int basePrice = level.ChestPrice;
            var fidelityCard = player.Inventory.FirstOrDefault(i => i.Id == ItemId.FidelityCard);
            if (fidelityCard == null) return basePrice;

            float discountRate = Math.Clamp(fidelityCard.Value / 100f, 0f, 0.95f);
            int discounted = (int)MathF.Round(basePrice * (1 - discountRate));
            return Math.Max(1, discounted);
        }

        DialogueNode Node(Func<string> text) => new DialogueNode { Text = text };

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

        mainMenu = Node(() =>
        {
            var intro = npc.HasMet ? "Back again, wanderer?" : "Greetings, wanderer.";
            return $"{intro} Care to buy a boon for {GetCurrentPrice()} gold?";
        });

        // Shop
        mainMenu.Options.Add(new DialogueOption
        {
            LabelFactory = () => $"Buy a boon ({GetCurrentPrice()} gold)",
            Action = () =>
            {
                int price = GetCurrentPrice();

                if (player.Gold < price) return "You do not have enough gold.";

                var choices = TreasureSelector.GenerateBonusChoices(player, settings);
                var chosen = TreasureSelector.PromptPlayerForBonus(choices, player, settings);

                player.Gold -= price;
                level.ChestPrice += 2; // inflation anti-exploit
                var msg = TreasureSelector.ApplyBonus(chosen, player, settings);

                return $"Purchased: {msg}\nChest price has increased.";
            },
            Next = mainMenu
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

        npc.Root = mainMenu;
    }
}