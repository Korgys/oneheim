namespace Roguelike.Console.Game.Systems;

using Roguelike.Console.Game.Characters.Enemies.Bosses;
using Roguelike.Console.Properties.i18n;

public sealed class WaveAndFogSystem : ITurnSystem
{
    public TurnPhase Phase => TurnPhase.AfterEnemiesMove;
    public string? LastMessage { get; private set; }

    public void Update(TurnContext ctx)
    {
        LastMessage = null;
        var level = ctx.Level;
        var player = level.Player;

        // Early death check (let engine end game)
        if (player.LifePoint <= 0) return;

        // First trigger
        if (player.Steps == 8)
        {
            level.PlaceEnemies(ctx.Difficulty.GetEnemiesNumber());
            LastMessage = Messages.BeCarefullYouAreNotSafeHere;
            return;
        }

        // Every 100 steps up to 1000
        if (player.Steps > 0 && player.Steps < 1001 && player.Steps % 100 == 0)
        {
            if (player.Steps % 500 == 0)
            {
                level.PlaceBoss();
                player.SetPlayerVisionAfterFogArrival();
                LastMessage = Messages.ABossArrives;
            }
            else
            {
                level.PlaceEnemies(ctx.Difficulty.GetEnemiesNumber());
                level.PlaceTreasures(ctx.Difficulty.GetTreasuresNumber());
                player.SetPlayerVisionAfterFogArrival();
                LastMessage = Messages.TheFogIntensifies;
            }
        }

        // Endgame condition
        if (player.Steps > 1000 && !level.Enemies.Any(e => e is Boss))
        {
            LastMessage = Messages.YouDefeatedAllBossesThanksForPlaying;
        }
    }
}
