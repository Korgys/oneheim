using Roguelike.Core.Game.Characters.NPCs.Dialogues;
using Roguelike.Core.Game.Characters.Players;
using Roguelike.Core.Game.Levels;
using Roguelike.Core.Game.Structures;
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.NPCs.Services;

/// <summary>
/// Domain service that encapsulates Armin's healing and camp repair logic.
/// It exposes side-effecting actions (deduct gold, modify HP) as Func<string>
/// to plug directly into DialogueOption.Action.
/// </summary>
public sealed class ArminService
{
    private readonly LevelManager _level;

    public ArminService(LevelManager level) => _level = level;

    private Player Player => _level.Player;
    private Structure? BaseCamp => _level.Structures.FirstOrDefault();

    // ----- Heal rules -----

    public bool CanHealNow =>
        Player.LifePoint < Player.MaxLifePoint &&
        HealAmount > 0;

    public int HealAmount
    {
        get
        {
            int missing = Math.Max(0, Player.MaxLifePoint - Player.LifePoint);
            return Math.Min(Player.Gold, missing);
        }
    }

    public int HealCost => HealAmount; // 1 gold = 1 HP

    public string HealAction()
    {
        if (Player.LifePoint >= Player.MaxLifePoint) return Messages.AlreadyFullHealth;
        if (!CanHealNow) return Messages.NotEnoughGold;

        int cost = HealCost;
        int amount = HealAmount;

        int before = Player.LifePoint;
        Player.LifePoint = Math.Min(Player.MaxLifePoint, Player.LifePoint + amount);
        Player.Gold -= cost;

        int healed = Player.LifePoint - before;
        return string.Format(Messages.HealedHpForGold, healed, cost);
    }

    public string HealPitchText() =>
        Player.LifePoint != Player.MaxLifePoint
            ? string.Format(Messages.ArminHealPitch, HealCost, HealAmount)
            : Messages.ArminHealPitchPlayerFullLife;

    // ----- Repair rules -----

    private int MissingCampHp => BaseCamp is null ? 0 : Math.Max(0, BaseCamp.MaxHp - BaseCamp.Hp);

    public bool CanRepairNow =>
        BaseCamp is not null &&
        BaseCamp.Hp < BaseCamp.MaxHp &&
        RepairAmount > 0;

    public int RepairAmount =>
        BaseCamp is null ? 0 : Math.Min(Player.Gold, MissingCampHp);

    public int RepairCost => RepairAmount; // 1 gold = 1 HP

    public string RepairAction()
    {
        if (BaseCamp is null) return Messages.NoCampToRepair;
        if (BaseCamp.Hp >= BaseCamp.MaxHp) return Messages.NoCampToRepair;
        if (!CanRepairNow) return Messages.NotEnoughGold;

        int cost = RepairCost;
        int amount = RepairAmount;

        int before = BaseCamp.Hp;
        BaseCamp.Hp = Math.Min(BaseCamp.MaxHp, BaseCamp.Hp + amount);
        Player.Gold -= cost;

        int repaired = BaseCamp.Hp - before;
        return string.Format(Messages.RepairedCampForHpCost, repaired, cost);
    }

    public string RepairPitchText()
    {
        if (BaseCamp is null) return Messages.NothingToRepair;
        int missing = MissingCampHp;
        return missing == 0
            ? Messages.TheCampIsAlreadyFullyRepair
            : string.Format(Messages.ArminRepairPitch, RepairCost, RepairAmount);
    }

    // ----- Other -----

    public string PickOther()
    {
        return Messages.WhereAreWe;

        //return Messages.JustTalking;
    }

    // ----- Node factory -----

    public (DialogueNode Hub, DialogueNode Heal, DialogueNode Repair, DialogueNode Other) BuildServiceNodes(string otherText)
    {
        var hub = new DialogueNode { Text = () => Messages.WhatElseCanIDo };

        var healNode = new DialogueNode { Text = HealPitchText };
        if (Player.LifePoint != Player.MaxLifePoint)
        {
            healNode.Options.Add(new DialogueOption
            {
                Label = Messages.PayAndHeal,
                Action = HealAction,
                Next = hub
            });
        }
        healNode.Options.Add(new DialogueOption { Label = Messages.MaybeLater, Next = hub });

        var repairNode = new DialogueNode { Text = RepairPitchText };
        if (BaseCamp is not null && BaseCamp.Hp < BaseCamp.MaxHp)
        {
            repairNode.Options.Add(new DialogueOption
            {
                Label = Messages.PayAndRepair,
                Action = RepairAction,
                Next = hub
            });
        }
        repairNode.Options.Add(new DialogueOption { Label = Messages.MaybeLater, Next = hub });

        var other = new DialogueNode 
        { 
            Text = () =>
            {
                if (otherText == Messages.WhereAreWe)
                {
                    if (ArminInteractions.HasExplainedWhereWeAre) return Messages.ArminAreYouSerious;
                    else
                    {
                        ArminInteractions.HasExplainedWhereWeAre = true;
                        return Messages.ArminOneheimWasASmallVillage;
                    }
                }
                else return "";
            }
        };
        other.Options.Add(new DialogueOption { Label = Messages.Ok, Next = hub });

        // Hub gets the standard entries; caller may add more
        AddStandardOptions(hub, healNode, repairNode, other);

        return (hub, healNode, repairNode, other);
    }

    private void AddStandardOptions(DialogueNode node, DialogueNode heal, DialogueNode repair, DialogueNode other)
    {
        node.Options.Add(new DialogueOption { Label = Messages.NeedHealing, Next = heal });
        node.Options.Add(new DialogueOption { Label = Messages.RepairTheCamp, Next = repair });
        node.Options.Add(new DialogueOption { Label = PickOther(), Next = other });
        node.Options.Add(new DialogueOption { Label = Messages.Goodbye, Next = null });        
    }
}
