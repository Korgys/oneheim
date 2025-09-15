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
    private int _newWaveStartedAt = 0;

    private PlayerController _playerController;

    public WaveAndFogSystem(PlayerController playerController)
    {
        _playerController = playerController;
    }

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

        // New wave every 100 steps up to 1515
        if (player.Steps > 0 && player.Steps < 1515 && player.Steps % 100 == 0 && _newWaveStartedAt != player.Steps)
        {
            _newWaveStartedAt = player.Steps;

            // Remove weak enemies (level 3 below the wave level)
            if (player.Steps >= 400)
            {
                var weakEnemies = level.Enemies.Where(e => e.Level <= (player.Steps / 100) - 3);
                foreach (var weakEnemy in weakEnemies.ToList())
                {
                    level.Enemies.Remove(weakEnemy);
                }
            }

            // Place new enemies and treasures
            level.PlaceEnemies(ctx.Difficulty.GetEnemiesNumber());
            level.PlaceTreasures(ctx.Difficulty.GetTreasuresNumber());
            LastMessage = Messages.TheFogIntensifies;
        }

        // 1st Boss at 515 steps
        if (player.Steps >= 515 && !_firstBossPlaced)
        {
            PlaceBoss(level, player);
            _firstBossPlaced = true;
        }
        // 2nd Boss at 1015 steps
        else if (player.Steps >= 1015 && !_secondBossPlaced)
        {
            PlaceBoss(level, player);
            _secondBossPlaced = true;
        }
        // 3rd Boss at 1515 steps
        else if (player.Steps >= 1515 && !_thirdBossPlaced)
        {
            PlaceBoss(level, player);
            _thirdBossPlaced = true;
        }

        // Endgame condition
        if (player.Steps > 1515 && !level.Enemies.Any(e => e is Boss))
        {
            LastMessage = Messages.YouDefeatedAllBossesThanksForPlaying;
            _playerController.IsGameEnded = true;
        }
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
