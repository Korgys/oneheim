namespace Roguelike.Console.Game.Characters.Players;

using Roguelike.Console.Configuration;
using Roguelike.Console.Game.Characters.NPCs.Dialogues;
using Roguelike.Console.Game.Collectables;
using Roguelike.Console.Game.Levels;
using System;

public class PlayerController
{
    private readonly LevelManager _level;
    private readonly GameSettings _settings;

    public bool HasUsedKey { get; private set; } = false;
    public bool HasMovedThisTurn { get; private set; } = false;
    public bool IsGameEnded { get; private set; } = false;
    public string GameMessage { get; private set; } = string.Empty;

    public PlayerController(LevelManager level, GameSettings settings)
    {
        _level = level;
        _settings = settings;
    }

    public void ReadAndProcessUserInput()
    {
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

            if (key == _settings.ControlsSettings.MoveUp)
                newY--;
            else if (key == _settings.ControlsSettings.MoveDown)
                newY++;
            else if (key == _settings.ControlsSettings.MoveLeft)
                newX--;
            else if (key == _settings.ControlsSettings.MoveRight)
                newX++;
            else if (key == _settings.ControlsSettings.ExitGame)
            {
                IsGameEnded = true;
                return;
            }
            else
                continue;

            var npc = _level.Npcs.FirstOrDefault(n => n.X == newX && n.Y == newY);
            if (npc != null)
            {
                NpcDialogManager.StartDialogue(npc, _level, _settings);
                GameMessage = $"You talked with {npc.Name}.";
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
            // do NOT increment steps if inside any structure
            if (!insideStructure)
            {
                // increment steps
                _level.Player.Steps++;
            }

            GameMessage = string.Empty;

            CheckTreasureUnderPlayer();
            CheckEnemyUnderPlayer();
        }
    }

    private void CheckTreasureUnderPlayer()
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
    }

    private void CheckEnemyUnderPlayer()
    {
        var player = _level.Player;
        var enemy = _level.Enemies.FirstOrDefault(e => e.X == player.X && e.Y == player.Y);
        if (enemy != null)
        {
            var combat = new Combat.CombatManager(_level);
            combat.StartCombat(enemy);
            GameMessage = "Combat occurred!";
        }
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
