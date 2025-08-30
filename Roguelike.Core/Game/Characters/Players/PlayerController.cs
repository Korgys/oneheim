namespace Roguelike.Core.Game.Characters.Players;

using Roguelike.Core.Configuration;
using Roguelike.Core.Game.Abstractions;
using Roguelike.Core.Game.Characters.NPCs.Dialogues;
using Roguelike.Core.Game.Collectables;
using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Game.Levels;
using Roguelike.Core.Properties.i18n;
using System;

public class PlayerController
{
    private readonly LevelManager _level;
    private readonly GameSettings _settings;
    private readonly ICombatRenderer _combatUi;

    public bool HasUsedKey { get; private set; } = false;
    public bool HasMovedThisTurn { get; private set; } = false;
    public bool IsGameEnded { get; private set; } = false;
    public string GameMessage { get; private set; } = string.Empty;

    public PlayerController(LevelManager level, GameSettings settings, ICombatRenderer combatUi)
    {
        _level = level;
        _settings = settings;
        _combatUi = combatUi ?? new NullCombatRenderer();
    }

    public void ReadAndProcessUserInput()
    {
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

            if (key == _settings.Controls.MoveUp)
                newY--;
            else if (key == _settings.Controls.MoveDown)
                newY++;
            else if (key == _settings.Controls.MoveLeft)
                newX--;
            else if (key == _settings.Controls.MoveRight)
                newX++;
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
                    player.Steps = 149;
                    player.Armor = 200;
                    player.Strength = 200;
                    player.Speed = 200;
                    player.Gold = 2000;
                    player.MaxLifePoint = 100;
                    player.LifePoint = player.MaxLifePoint;
                    player.Vision = 20;
                }
#endif
                continue;
            }

            if (_level.Npcs.Any(n => n.X == newX && n.Y == newY))
            {
                var npc = _level.Npcs.First(n => n.X == newX && n.Y == newY);
                NpcDialogManager.StartDialogue(npc, _level, _settings);
                GameMessage = string.Format(Messages.YouTalkedWith, npc.Name);
                keyRecognized = true;
                continue;
            }
            else if (_level.Mercenaries.Any(m => m.X == newX && m.Y == newY))
            {
                var allie = _level.Mercenaries.First(m => m.X == newX && m.Y == newY);
                GameMessage = string.Format(Messages.YouTalkedWith, allie.Name);
                keyRecognized = true;
                continue;
            }
            else if (CanMoveTo(newX, newY, true, true, false))
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

            bool hasLootedChest = CheckTreasureUnderPlayer();
            bool hasFightEnemy = CheckEnemyUnderPlayer();

            // do NOT increment steps if inside any structure or if he just looted chest or entered in combat
            if (!insideStructure && !hasLootedChest && !hasFightEnemy)
            {
                // StopWatch logic
                var stopWatch = _level.Player.Inventory.FirstOrDefault(i => i.Id == ItemId.StopWatch);
                if (stopWatch != null && _level.Player.Steps % stopWatch.Value == 0)
                {
                    return; // skip incrementing steps this turn
                }

                // increment steps
                _level.Player.Steps++;
            }
        }
    }

    private bool CheckTreasureUnderPlayer()
    {
        var player = _level.Player;
        var treasure = _level.Treasures.FirstOrDefault(t => t.X == player.X && t.Y == player.Y);
        if (treasure != null)
        {
            var loot = TreasureSelector.GenerateBonusChoices(player, _settings);
            var selected = TreasureSelector.PromptPlayerForBonus(loot, player, _settings);
            GameMessage = TreasureSelector.ApplyBonus(selected, player, _settings);
            _level.Treasures.Remove(treasure);
        }
        return treasure != null;
    }

    private bool CheckEnemyUnderPlayer()
    {
        var player = _level.Player;
        var enemy = _level.Enemies.FirstOrDefault(e => e.X == player.X && e.Y == player.Y);
        if (enemy != null)
        {
            var combat = new Combat.CombatManager(_level, _combatUi);
            combat.StartCombat(enemy);
            GameMessage = Messages.CombatOccurred;
        }
        return enemy != null;
    }

    private bool CanMoveTo(int x, int y, bool allowMoveOnTreasure, bool allowMoveOnEnemy, bool allowMoveOnNpc)
    {
        if (x <= 0 || x >= LevelManager.GridWidth - 1 || y <= 0 || y >= LevelManager.GridHeight - 1)
            return false;

        // Block structure walls
        if (_level.Structures.Any(s => s.IsWall(x, y)))
            return false;

        if (!allowMoveOnTreasure && _level.Treasures.Any(t => t.X == x && t.Y == y))
            return false;

        if (!allowMoveOnEnemy && _level.Enemies.Any(e => e.X == x && e.Y == y))
            return false;

        if (!allowMoveOnNpc && _level.Npcs.Any(n => n.X == x && n.Y == y))
            return false;

        return true;
    }
}
