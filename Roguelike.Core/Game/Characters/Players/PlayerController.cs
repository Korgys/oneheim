using Roguelike.Core.Configuration;
using Roguelike.Core.Game.Abstractions;
using Roguelike.Core.Game.Collectables;
using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Game.Levels;
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Players
{
    public class PlayerController
    {
        private readonly LevelManager _level;
        private readonly GameSettings _settings;
        private readonly ICombatRenderer _combatUi;
        private readonly IDialogueRenderer _dialogueUi;
        private readonly ITreasurePicker _treasurePicker;
        private readonly IInventoryUI _inventoryUi;

        private bool _stopWatchUsed = false;

        public bool HasUsedKey { get; private set; } = false;
        public bool HasMovedThisTurn { get; private set; } = false;
        public bool IsGameEnded { get; private set; } = false;
        public string GameMessage { get; private set; } = string.Empty;

        public PlayerController(
            LevelManager level,
            GameSettings settings,
            ICombatRenderer combatUi,
            IDialogueRenderer dialogueUi,
            ITreasurePicker treasurePicker,
            IInventoryUI inventoryUi)
        {
            _level = level ?? throw new ArgumentNullException(nameof(level));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _combatUi = combatUi ?? new NullCombatRenderer();        // no-op renderer is fine
            _dialogueUi = dialogueUi ?? throw new ArgumentNullException(nameof(dialogueUi));
            _treasurePicker = treasurePicker ?? throw new ArgumentNullException(nameof(treasurePicker));
            _inventoryUi = inventoryUi;
        }

        public void ReadAndProcessUserInput()
        {
            // Ignore inputs while a combat is occurring
            if (_level.PlayerInCombat) return;

            if (_level.Player.LifePoint <= 0)
            {
                IsGameEnded = true;
                return;
            }

            bool playerMoved = false;
            bool keyRecognized = false;

            while (!keyRecognized)
            {
                var key = Console.ReadKey(true).Key.ToString().ToUpperInvariant();
                var player = _level.Player;
                var grid = _level.Grid;

                int newX = player.X;
                int newY = player.Y;

                if (key == _settings.Controls.MoveUp) newY--;
                else if (key == _settings.Controls.MoveDown) newY++;
                else if (key == _settings.Controls.MoveLeft) newX--;
                else if (key == _settings.Controls.MoveRight) newX++;
                else if (key == _settings.Controls.Exit)
                {
                    IsGameEnded = true;
                    return;
                }
                else
                {
#if DEBUG
                    if (key == "M")
                    {
                        player.Armor = 200;
                        player.Strength = 200;
                        player.Speed = 200;
                        player.Gold = 2000;
                        player.MaxLifePoint = 100;
                        player.LifePoint = player.MaxLifePoint;
                        player.Vision = 7;
                    }
#endif
                    continue; // unrecognized key, keep waiting
                }

                // NPC interaction (do not enter tile; trigger dialogue instead)
                var npc = _level.Npcs.FirstOrDefault(n => n.X == newX && n.Y == newY);
                if (npc != null)
                {
                    NpcDialogManager.StartDialogue(npc, _level, _settings, _dialogueUi, _treasurePicker, _inventoryUi);
                    GameMessage = string.Format(Messages.YouTalkedWith, npc.Name);
                    keyRecognized = true;
                    continue;
                }

                // Ally/mercenary interaction (currently just a message)
                if (_level.Mercenaries.Any(m => m.X == newX && m.Y == newY))
                {
                    var ally = _level.Mercenaries.First(m => m.X == newX && m.Y == newY);
                    GameMessage = string.Format(Messages.YouTalkedWith, ally.Name);
                    keyRecognized = true;
                    continue;
                }

                // Movement
                if (CanMoveTo(newX, newY, allowMoveOnTreasure: true, allowMoveOnEnemy: true, allowMoveOnNpc: false))
                {
                    grid[player.Y, player.X] = ' ';
                    player.X = newX;
                    player.Y = newY;
                    playerMoved = true;
                }

                keyRecognized = true;
            }

            bool insideStructure = _level.Structures.Any(s => s.IsInterior(_level.Player.X, _level.Player.Y));

            HasUsedKey = true;
            HasMovedThisTurn = playerMoved;

            if (playerMoved)
            {
                GameMessage = string.Empty;

                bool lootedChest = CheckTreasureUnderPlayer(); // may set GameMessage
                bool foughtEnemy = CheckEnemyUnderPlayer();     // may set GameMessage

                // Do not increment steps if inside any structure or if chest/ combat consumed the turn
                if (!insideStructure && !lootedChest && !foughtEnemy)
                {
                    // StopWatch item: skip increment on given frequency
                    var stopWatch = _level.Player.Inventory.FirstOrDefault(i => i.Id == ItemId.StopWatch);
                    if (stopWatch != null! && stopWatch.Value > 0 && _level.Player.Steps % stopWatch.Value == 0)
                    {
                        if (!_stopWatchUsed)
                        {
                            _stopWatchUsed = true;
                            return; // skip step increment this turn
                        }
                        else
                        {
                            _stopWatchUsed = false;
                        }                        
                    }

                    _level.Player.Steps++;
                }
            }
        }

        private bool CheckTreasureUnderPlayer()
        {
            var player = _level.Player;
            var treasure = _level.Treasures.FirstOrDefault(t => t.X == player.X && t.Y == player.Y);
            if (treasure == null) return false;

            // Generate & pick using the UI-agnostic picker
            var selected = TreasureSelector.ChooseWithPicker(player, _settings, _treasurePicker);

            // Apply the bonus and set feedback
            GameMessage = TreasureSelector.ApplyBonus(selected, player, _settings, _inventoryUi);

            // Remove the chest
            _level.Treasures.Remove(treasure);
            return true;
        }

        private bool CheckEnemyUnderPlayer()
        {
            var player = _level.Player;
            var enemy = _level.Enemies.FirstOrDefault(e => e.X == player.X && e.Y == player.Y);
            if (enemy == null) return false;

            var combat = new Combat.CombatManager(_level, _combatUi);
            combat.StartCombat(enemy);
            GameMessage = Messages.CombatOccurred;
            return true;
        }

        private bool CanMoveTo(int x, int y, bool allowMoveOnTreasure, bool allowMoveOnEnemy, bool allowMoveOnNpc)
        {
            // Keep within the world bounds
            if (x <= 0 || x >= LevelManager.GridWidth - 1 || y <= 0 || y >= LevelManager.GridHeight - 1)
                return false;

            // Block structure walls
            if (_level.Structures.Any(s => s.IsWall(x, y)))
                return false;

            // Tile-based occupancy checks
            if (!allowMoveOnTreasure && _level.Treasures.Any(t => t.X == x && t.Y == y))
                return false;

            if (!allowMoveOnEnemy && _level.Enemies.Any(e => e.X == x && e.Y == y))
                return false;

            if (!allowMoveOnNpc && _level.Npcs.Any(n => n.X == x && n.Y == y))
                return false;

            return true;
        }
    }
}
