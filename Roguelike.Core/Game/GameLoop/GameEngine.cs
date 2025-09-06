using Roguelike.Core.Configuration;
using Roguelike.Core.Game.Abstractions;
using Roguelike.Core.Game.Characters.Enemies;
using Roguelike.Core.Game.Characters.NPCs;
using Roguelike.Core.Game.Characters.Players;
using Roguelike.Core.Game.Levels;
using Roguelike.Core.Game.Systems;
using Roguelike.Core.Game.Systems.Logics;
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.GameLoop;

public sealed class GameEngine
{
    private readonly IRenderer _renderer;
    private readonly IClock _clock;

    private readonly TurnSystemRunner _runner;
    private readonly LevelManager _level;
    private readonly PlayerController _playerController;
    private readonly EnemyManager _enemyManager;
    private readonly DifficultyManager _difficultyManager;
    private readonly GameSettings _settings;

    private string _gameMessage = string.Empty;
    private bool _isGameEnded = false;

    public GameEngine(
        GameSettings settings, 
        IRenderer renderer, 
        IClock clock, 
        ICombatRenderer combatRenderer, 
        IDialogueRenderer dialogueRenderer,
        ITreasurePicker treasurePicker,
        IInventoryUI inventoryUI)
    {
        _renderer = renderer;
        _clock = clock;

        _settings = settings;

        _level = new LevelManager(settings);
        _playerController = new PlayerController(_level, settings, combatRenderer, dialogueRenderer, treasurePicker, inventoryUI);
        _difficultyManager = new DifficultyManager(settings.Difficulty);

        var siegeSystem = new StructureSiegeSystem();
        _runner = new TurnSystemRunner();
        _runner.Register(siegeSystem);
        _runner.Register(new WaveAndFogSystem());
        _runner.Register(new MercenaryPatrolSystem());

        _enemyManager = new EnemyManager(_level, siegeSystem, combatRenderer);
    }

    public void Run()
    {
        while (!_isGameEnded)
        {
            // 1) Render current frame (include current message + end state + input hint)
            var view = GameStateView.From(
                _level,
                _settings,
                currentMessage: _gameMessage,
                isGameEnded: _isGameEnded,
                hasUsedKey: _playerController.HasUsedKey
            );
            _renderer.RenderFrame(view);

            // 2) Read/handle player action via controller
            _playerController.ReadAndProcessUserInput();
            _gameMessage = _playerController.GameMessage ?? string.Empty;

            if (_playerController.IsGameEnded)
            {
                _isGameEnded = true;
                break;
            }

            // 3) Run "before enemies move" systems
            var ctx = new TurnContext(_level, _settings, _difficultyManager);
            var beforeMsgs = _runner.Run(TurnPhase.BeforeEnemiesMove, ctx);
            if (beforeMsgs.Any())
                _gameMessage = string.Join("\n", beforeMsgs);

            // 4) Enemy movement + eventual combats
            _enemyManager.MoveEnemies();
            if (!string.IsNullOrWhiteSpace(_enemyManager.CombatMessage))
                _gameMessage = _enemyManager.CombatMessage!;

            // 5) Run "after enemies move" systems
            var afterMsgs = _runner.Run(TurnPhase.AfterEnemiesMove, ctx);
            if (afterMsgs.Any())
                _gameMessage = string.Join("\n", afterMsgs);

            // 6) Time-based / step-based events (NPC spawns, etc.)
            ApplyGameEventsIfNeeded();

            // 7) Let the renderer surface any consolidated message line(s)
            if (!string.IsNullOrWhiteSpace(_gameMessage))
                _renderer.ShowMessages(new[] { _gameMessage });

            // 8) Small tick pacing (UI can override/ignore)
            _clock.Delay(1);
        }

        // Final frame (typically game over / end screen)
        var endView = GameStateView.From(
            _level,
            _settings,
            currentMessage: _gameMessage,
            isGameEnded: _isGameEnded,
            hasUsedKey: true  // at end we don't need the controls helper anymore
        );
        _renderer.RenderFrame(endView);
    }

    private void ApplyGameEventsIfNeeded()
    {
        // Spawn Ichem (shop NPC) at 150 steps
        if (_level.Player.Steps == 150 &&
            !_level.Npcs.Any(n => n.Id == NpcId.Ichem) &&
            _level.Structures.Any(s => s.Name == Messages.BaseCamp))
        {
            _level.PlaceNpc(NpcId.Ichem);
            _gameMessage = Messages.ANewTravelerComesToTheBaseCamp;
        }

        // Spawn Eber (mercenary NPC) at 250 steps
        if (_level.Player.Steps == 250 &&
            !_level.Npcs.Any(n => n.Id == NpcId.Eber) &&
            _level.Structures.Any(s => s.Name == Messages.BaseCamp))
        {
            _level.PlaceNpc(NpcId.Eber);
            _gameMessage = Messages.ANewTravelerComesToTheBaseCamp;
        }
    }
}
