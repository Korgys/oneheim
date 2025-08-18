using Roguelike.Console.Configuration;
using Roguelike.Console.Game.Characters.Enemies;
using Roguelike.Console.Game.Characters.Enemies.Bosses;
using Roguelike.Console.Game.Characters.NPCs;
using Roguelike.Console.Game.Characters.Players;
using Roguelike.Console.Game.Levels;
using Roguelike.Console.Game.Systems;
using Roguelike.Console.Properties.i18n;
using Roguelike.Console.Rendering;

namespace Roguelike.Console.Game;

public class GameEngine
{
    private GameSettings _gameSettings;
    private LevelManager _levelManager;
    private PlayerController _playerController;
    private EnemyManager _enemyManager;
    private DifficultyManager _difficultyManager;
    private StructureSiegeSystem _siegeSystem;
    private TurnSystemRunner _runner;

    private string _gameMessage = string.Empty;
    private bool _isGameEnded = false;

    public void StartNewGame()
    {
        _gameSettings = ConfigurationReader.LoadGameSettings();
        _levelManager = new LevelManager(_gameSettings);
        _playerController = new PlayerController(_levelManager, _gameSettings);
        _difficultyManager = new DifficultyManager(_gameSettings.Difficulty);

        // Register systems
        _runner = new TurnSystemRunner();
        _siegeSystem = new StructureSiegeSystem();
        _runner.Register(_siegeSystem);
        _runner.Register(new WaveAndFogSystem());

        _enemyManager = new EnemyManager(_levelManager, _siegeSystem); // EnemyManager uses siege info


        while (!_isGameEnded)
        {
            ConsoleRenderer.RenderGrid(_levelManager, _gameSettings, _playerController.HasUsedKey, _gameMessage, _isGameEnded);
            _playerController.ReadAndProcessUserInput();
            _gameMessage = _playerController.GameMessage;

            if (_playerController.IsGameEnded)
            {
                _isGameEnded = true;
                break;
            }

            _enemyManager.MoveEnemies();
            _gameMessage = _enemyManager.CombatMessage ?? _gameMessage;

            ApplyGameEventsIfNeeded();
        }

        ConsoleRenderer.RenderGrid(_levelManager, _gameSettings, true, _gameMessage, _isGameEnded);
    }

    private void ApplyGameEventsIfNeeded()
    {
        // Player death
        if (_levelManager.Player.LifePoint <= 0)
        {
            _isGameEnded = true;
            return;
        }

        if (_levelManager.Player.Steps == 6)
        {
            _levelManager.PlaceEnemies(_difficultyManager.GetEnemiesNumber());
            _gameMessage = Messages.BeCarefullYouAreNotSafeHere;
        }

        // New enemies/boss wave
        if (_levelManager.Player.Steps > 0
            && _levelManager.Player.Steps < 1001
            && _levelManager.Player.Steps % 100 == 0)
        {
            if (_levelManager.Player.Steps % 500 == 0)
            {
                _levelManager.PlaceBoss();
                _levelManager.Player.SetPlayerVisionAfterFogArrival();
                _gameMessage = Messages.ABossArrives;
            }
            else
            {
                _levelManager.PlaceEnemies(_difficultyManager.GetEnemiesNumber());
                _levelManager.PlaceTreasures(_difficultyManager.GetTreasuresNumber());
                _levelManager.Player.SetPlayerVisionAfterFogArrival();
                _gameMessage = Messages.TheFogIntensifies;
            }
        }

        // Spawn Ichem (shop NPC) at 150 steps
        if (_levelManager.Player.Steps == 150 && !_levelManager.Npcs.Any(n => n.Id == NpcId.Ichem) 
            && _levelManager.Structures.Any(s => s.Name == Messages.BaseCamp))
        {
            _levelManager.PlaceNpc(NpcId.Ichem);
            _gameMessage = "A new traveler comes to the base camp";
        }

        // Endgame if all bosses are dead and level steps = 10
        if (_levelManager.Player.Steps > 1000 && !_levelManager.Enemies.Any(e => e is Boss))
        {
            _gameMessage = Messages.YouDefeatedAllBossesThanksForPlaying;
            _isGameEnded = true;
        }
    }
}

