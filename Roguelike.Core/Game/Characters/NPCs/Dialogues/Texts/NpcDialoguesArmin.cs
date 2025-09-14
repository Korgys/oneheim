using Roguelike.Core.Game.Characters.Enemies.Bosses;
using Roguelike.Core.Game.Characters.NPCs.Services;
using Roguelike.Core.Game.Levels;
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.NPCs.Dialogues.Texts;

public static partial class NpcDialogues
{
    public static void BuildForArmin(Npc npc, LevelManager level)
    {
        var player = level.Player;        
        bool hasBoss = level.Enemies.Any(e => e is Boss);
        int steps = player.Steps;

        // Domain service for all service logic
        var service = new ArminService(level);
        string otherText = service.PickOther();
        var (hub, healNode, repairNode, other) = service.BuildServiceNodes(otherText);

        // Wire shared options onto all intros
        void WireStandard(DialogueNode node)
        {
            node.Options.Add(new DialogueOption { Label = Messages.NeedHealing, Next = healNode });
            node.Options.Add(new DialogueOption { Label = Messages.RepairTheCamp, Next = repairNode });
            node.Options.Add(new DialogueOption { Label = service.PickOther(), Next = other });
            node.Options.Add(new DialogueOption { Label = Messages.Goodbye, Next = null });
        }

        // Intro factory
        static DialogueNode Text(Func<string> text) => new() { Text = text };

        // Intros
        var firstIntro = Text(() => Messages.ArminFirstMeeting);
        var returningIntro = Text(() => Messages.ArminMeeting);
        var firstBossComingIntro = Text(() => Messages.ArminMeetingFirstBossComing);
        var firstBossHereIntro = Text(() => Messages.ArminMeetingFirstBossHere);
        var firstBossDefeatedIntro = Text(() => Messages.ArminMeetingFirstBossDefeated);
        var playerLowLifeIntro = Text(() => Messages.ArminMeetingPlayerLowLife);
        var playerRichIntro = Text(() => Messages.ArminMeetingPlayerRich);
        var baseCampUnderAttackIntro = Text(() => Messages.ArminMeetingBaseCampUnderAttack);
        var intros = new[]
        {
            firstIntro, returningIntro, firstBossComingIntro, firstBossHereIntro,
            firstBossDefeatedIntro, playerLowLifeIntro, playerRichIntro, baseCampUnderAttackIntro
        };
        foreach (var intro in intros)
        {
            WireStandard(intro);
        }

        // Root selection (behavior preserved)
        DialogueNode PickRoot()
        {
            if (!npc.HasMet) return firstIntro;

            if (level.IsBaseCampUnderAttack()) return baseCampUnderAttackIntro;
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
}
