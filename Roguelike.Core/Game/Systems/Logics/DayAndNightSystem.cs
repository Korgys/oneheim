using Roguelike.Core.Game.Characters.Players;
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Systems.Logics;

public sealed class DayAndNightSystem : ITurnSystem
{
    public TurnPhase Phase => TurnPhase.BeforeEnemiesMove;
    public string? LastMessage { get; private set; }

    // Vision tuning (negative = vision down, positive = vision up)
    public int VisionDeltaSunset { get; set; } = -1;
    public int VisionDeltaNight { get; set; } = -1;
    public int VisionDeltaSunrise { get; set; } = +1;
    public int VisionDeltaDay { get; set; } = +1;

    // Clamps to keep gameplay readable
    public int VisionMin { get; set; } = 1;
    public int VisionMax { get; set; } = 10;

    public void Update(TurnContext ctx)
    {
        LastMessage = null;
        var level = ctx.Level;
        var player = level.Player;

        if (level.Structures.Any(s => s.IsInterior(level.Player.X, level.Player.Y)))
            return; // No day/night cycle effects when indoors

        int stepsForNextCycle = level.StepsForFullCycle;
        int moduloForCycle = stepsForNextCycle == 0 ? 0 : (player.Steps % stepsForNextCycle);

        // Keyframes du cycle (tu peux les déplacer si tu veux des durées différentes)
        if (moduloForCycle == 0)
        {
            level.DayCycle = DayCycle.Sunset;
            LastMessage = Messages.TheSunSets;
            ApplyVisionDelta(player, VisionDeltaSunset);
        }
        else if (moduloForCycle == 15)
        {
            level.DayCycle = DayCycle.Night;
            LastMessage = Messages.TheNightArrives;
            ApplyVisionDelta(player, VisionDeltaNight);
        }
        else if (moduloForCycle == 65)
        {
            level.DayCycle = DayCycle.Sunrise;
            LastMessage = Messages.TheSunRises;
            ApplyVisionDelta(player, VisionDeltaSunrise);
        }
        else if (moduloForCycle == 80)
        {
            level.DayCycle = DayCycle.Day;
            LastMessage = Messages.ANewDayDawns;
            ApplyVisionDelta(player, VisionDeltaDay);
        }
    }

    private void ApplyVisionDelta(Player p, int delta)
    {
        p.Vision = Math.Clamp(p.Vision + delta, VisionMin, VisionMax);
    }
}
