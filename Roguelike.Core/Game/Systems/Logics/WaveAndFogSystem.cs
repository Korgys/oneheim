using Roguelike.Core.Game.Characters.Enemies.Bosses;
using Roguelike.Core.Game.Characters.Players;
using Roguelike.Core.Game.Levels;
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Systems.Logics;

public sealed class WaveAndFogSystem : ITurnSystem
{
    public TurnPhase Phase => TurnPhase.AfterEnemiesMove;
    public string? LastMessage { get; private set; }

    private bool _firstBossPlaced = false;
    private bool _secondBossPlaced = false;
    private bool _thirdBossPlaced = false;

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
            level.PlaceEnemies(ctx.Difficulty.GetEnemiesNumber());
            level.PlaceTreasures(ctx.Difficulty.GetTreasuresNumber());
            LastMessage = Messages.TheFogIntensifies;
        }

        // 1st Boss at 500 steps
        if (player.Steps >= 500 && !_firstBossPlaced)
        {
            PlaceBoss(level, player);
            _firstBossPlaced = true;
        }
        // 2nd Boss at 1000 steps
        else if (player.Steps >= 1000 && !_secondBossPlaced)
        {
            PlaceBoss(level, player);
            _secondBossPlaced = true;
        }
        // 3rd Boss at 1500 steps
        else if (player.Steps >= 1500 && !_thirdBossPlaced)
        {
            PlaceBoss(level, player);
            _thirdBossPlaced = true;
        }

        // Endgame condition
        if (player.Steps > 1500 && !level.Enemies.Any(e => e is Boss))
            LastMessage = Messages.YouDefeatedAllBossesThanksForPlaying;
    }

    /// <summary>
    /// Place a boss on the level and update player vision and last message.
    /// </summary>
    /// <param name="level"></param>
    /// <param name="player"></param>
    private void PlaceBoss(LevelManager level, Player player)
    {
        level.PlaceBoss();
        LastMessage = Messages.ABossArrives;
    }
}
