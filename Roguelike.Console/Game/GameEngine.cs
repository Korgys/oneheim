using Roguelike.Console.Configuration;
using Roguelike.Console.Game.Characters.Enemies;
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
        _runner.Register(new MercenaryPatrolSystem());

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

            // Inside game loop, after player input
            var ctx = new TurnContext(_levelManager, _gameSettings, _difficultyManager);

            // Run systems before enemies move
            var beforeMsgs = _runner.Run(TurnPhase.BeforeEnemiesMove, ctx);
            if (beforeMsgs.Any())
                _gameMessage = string.Join("\n", beforeMsgs);

            // Enemy movement & combat
            _enemyManager.MoveEnemies();
            _gameMessage = _enemyManager.CombatMessage ?? _gameMessage;

            // Run systems after enemies move
            var afterMsgs = _runner.Run(TurnPhase.AfterEnemiesMove, ctx);
            if (afterMsgs.Any())
                _gameMessage = string.Join("\n", afterMsgs);

            ApplyGameEventsIfNeeded();
        }

        ConsoleRenderer.RenderGrid(_levelManager, _gameSettings, true, _gameMessage, _isGameEnded);
    }

    private void ApplyGameEventsIfNeeded()
    {
        // Spawn Ichem (shop NPC) at 150 steps
        if (_levelManager.Player.Steps == 150 && !_levelManager.Npcs.Any(n => n.Id == NpcId.Ichem) 
            && _levelManager.Structures.Any(s => s.Name == Messages.BaseCamp))
        {
            _levelManager.PlaceNpc(NpcId.Ichem);
            _gameMessage = Messages.ANewTravelerComesToTheBaseCamp;
        }

        // Spawn Eber (mercenary, hire guards) at 250 steps
        if (_levelManager.Player.Steps == 250 && !_levelManager.Npcs.Any(n => n.Id == NpcId.Eber)
            && _levelManager.Structures.Any(s => s.Name == Messages.BaseCamp))
        {
            _levelManager.PlaceNpc(NpcId.Eber);
            _gameMessage = Messages.ANewTravelerComesToTheBaseCamp;
        }
    }
}

