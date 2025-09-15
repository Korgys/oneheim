using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Systems.Logics;

public sealed class DayAndNightSystem : ITurnSystem
{
    public TurnPhase Phase => TurnPhase.BeforeEnemiesMove;
    public string? LastMessage { get; private set; }

    // Vision tuning (negative = vision down, positive = vision up)
    public int VisionDeltaSunset { get; set; } = -1;
    public int VisionDeltaNight { get; set; } = -2;
    public int VisionDeltaSunrise { get; set; } = +1;
    public int VisionDeltaDay { get; set; } = +1;

    // Clamps to keep gameplay readable
    public int VisionMin { get; set; } = 1;
    public int VisionMax { get; set; } = 20;

    private int _lastModuloForCycle = -1;

    public void Update(TurnContext ctx)
    {
        LastMessage = null;
        var level = ctx.Level;
        var player = level.Player;

        if (level.Structures.Any(s => s.IsInterior(level.Player.X, level.Player.Y)))
            return; // No day/night cycle effects when indoors

        int stepsForNextCycle = level.StepsForFullCycle;
        int moduloForCycle = stepsForNextCycle == 0 ? 0 : (player.Steps % stepsForNextCycle);

        if (moduloForCycle == _lastModuloForCycle) return; // No change in cycle

        // Cycle changes at specific steps in the cycle
        if (moduloForCycle == 0)
        {
            level.DayCycle = DayCycle.Sunset;
            LastMessage = Messages.TheSunSets;
            player.SetPlayerVision(Math.Clamp(player.Vision + VisionDeltaSunset, VisionMin, VisionMax));
            _lastModuloForCycle = moduloForCycle;
        }
        else if (moduloForCycle == 15)
        {
            level.DayCycle = DayCycle.Night;
            LastMessage = Messages.TheNightArrives;
            player.SetPlayerVision(Math.Clamp(player.Vision + VisionDeltaNight, VisionMin, VisionMax));
            _lastModuloForCycle = moduloForCycle;
        }
        else if (moduloForCycle == 65)
        {
            level.DayCycle = DayCycle.Sunrise;
            LastMessage = Messages.TheSunRises;
            player.SetPlayerVision(Math.Clamp(player.Vision + VisionDeltaSunrise, VisionMin, VisionMax));
            _lastModuloForCycle = moduloForCycle;
        }
        else if (moduloForCycle == 80)
        {
            level.DayCycle = DayCycle.Day;
            LastMessage = Messages.ANewDayDawns;
            player.SetPlayerVision(Math.Clamp(player.Vision + VisionDeltaDay, VisionMin, VisionMax));
            _lastModuloForCycle = moduloForCycle;
        }
    }

}
