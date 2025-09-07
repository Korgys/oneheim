namespace Roguelike.Core.Game.GameLoop;

using Roguelike.Core.Configuration;
using Roguelike.Core.Game.Characters.Enemies;
using Roguelike.Core.Game.Characters.NPCs;
using Roguelike.Core.Game.Characters.NPCs.Allies;
using Roguelike.Core.Game.Characters.Players;
using Roguelike.Core.Game.Collectables;
using Roguelike.Core.Game.Levels;
using Roguelike.Core.Game.Structures;
using Roguelike.Core.Game.Systems.Logics;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Immutable snapshot of the current game state for rendering/UI.
/// Built from the LevelManager and current game state.
/// </summary>
public sealed class GameStateView
{
    // Grid info
    public int GridWidth { get; }
    public int GridHeight { get; }

    // Entities
    public Player Player { get; }
    public IReadOnlyList<Npc> Npcs { get; }
    public IReadOnlyList<Enemy> Enemies { get; }
    public IReadOnlyList<Mercenary> Mercenaries { get; }
    public IReadOnlyList<Treasure> Treasures { get; }
    public IReadOnlyList<Structure> Structures { get; }

    // UI flags
    public string? CurrentMessage { get; }
    public bool IsGameEnded { get; }
    public bool HasPlayerUsedAValidKey { get; }
    public bool IsBaseCampUnderAttack { get; }

    // Day and night cycle
    public DayCycle DayCycle { get; init; }
    public double CycleProgress { get; init; } // 0.0 to 1.0

    // Controls mapping (for help display)
    public ControlsSettings Controls { get; }

    private GameStateView(
        int gridWidth, int gridHeight,
        Player player,
        IEnumerable<Npc> npcs,
        IEnumerable<Enemy> enemies,
        IEnumerable<Mercenary> mercenaries,
        IEnumerable<Treasure> treasures,
        IEnumerable<Structure> structures,
        string? currentMessage,
        bool isGameEnded,
        bool hasPlayerUsedAValidKey,
        bool isBaseCampUnderAttack,
        ControlsSettings controls, 
        DayCycle dayCycle, 
        double cycleProgress)
    {
        GridWidth = gridWidth;
        GridHeight = gridHeight;
        Player = player;
        Npcs = npcs.ToList();
        Enemies = enemies.ToList();
        Mercenaries = mercenaries.ToList();
        Treasures = treasures.ToList();
        Structures = structures.ToList();
        CurrentMessage = currentMessage;
        IsGameEnded = isGameEnded;
        HasPlayerUsedAValidKey = hasPlayerUsedAValidKey;
        IsBaseCampUnderAttack = isBaseCampUnderAttack;
        Controls = controls;
        DayCycle = dayCycle;
        CycleProgress = cycleProgress;
    }

    /// <summary>
    /// Builds a GameStateView from the current LevelManager + game flags.
    /// </summary>
    public static GameStateView From(
        LevelManager level,
        GameSettings settings,
        string? currentMessage = null,
        bool isGameEnded = false,
        bool hasUsedKey = false,
        bool isBaseCampUnderAttack = false)
    {
        return new GameStateView(
            LevelManager.GridWidth,
            LevelManager.GridHeight,
            level.Player,
            level.Npcs,
            level.Enemies,
            level.Mercenaries,
            level.Treasures,
            level.Structures,
            currentMessage,
            isGameEnded,
            hasUsedKey,
            isBaseCampUnderAttack,
            settings.Controls,
            level.DayCycle,
            level.StepsForFullCycle > 0
                ? (double)(level.Player.Steps % level.StepsForFullCycle) / level.StepsForFullCycle
                : 0.0
        );
    }
}
