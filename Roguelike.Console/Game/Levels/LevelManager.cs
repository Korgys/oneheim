namespace Roguelike.Console.Game.Levels;

using Roguelike.Console.Configuration;
using Roguelike.Console.Game.Characters.Enemies;
using Roguelike.Console.Game.Characters.Enemies.Bosses;
using Roguelike.Console.Game.Characters.NPCs;
using Roguelike.Console.Game.Characters.Players;
using Roguelike.Console.Game.Collectables;
using Roguelike.Console.Game.Collectables.Items;
using Roguelike.Console.Game.Structures;
using Roguelike.Console.Properties.i18n;
using System;

public class LevelManager
{
    public const int GridWidth = 60;
    public const int GridHeight = 22;
    public char[,] Grid { get; } = new char[GridHeight, GridWidth];

    public Player Player { get; private set; }
    public List<Treasure> Treasures { get; } = new();
    public List<Enemy> Enemies { get; } = new();
    public List<Npc> Npcs { get; } = new();
    public List<Structure> Structures { get; } = new();
    public bool PlayerInCombat { get; set; } = false;

    private readonly DifficultyManager _difficultyManager;
    private readonly Random _random = new();

    public LevelManager(GameSettings settings)
    {
        _difficultyManager = new DifficultyManager(settings.Difficulty);
        InitializeLevel();
    }

    public bool IsBaseCampUnderAttack()
    {
        var baseCamp = Structures.FirstOrDefault(s => s.Name == "Base Camp");
        if (baseCamp == null) return false;

        // Collect walls once
        var walls = baseCamp.WallTiles().ToArray();
        if (walls.Length == 0) return false;

        int attackers = 0;
        bool bossAdjacent = false;

        foreach (var e in Enemies)
        {
            // Ignore enemies that are inside the structure
            if (baseCamp.IsInterior(e.X, e.Y)) continue;

            // Adjacent to any wall tile? (4-neighborhood)
            for (int i = 0; i < walls.Length; i++)
            {
                var w = walls[i];
                if (Math.Abs(e.X - w.x) + Math.Abs(e.Y - w.y) == 1)
                {
                    attackers++;
                    if (e is Boss) bossAdjacent = true;
                    break;
                }
            }
        }

        // Rule: at least 3 attackers or any boss adjacent
        return attackers >= 3 || bossAdjacent;
    }

    public void PlaceNpc(NpcId npcId, int x, int y)
    {
        Npcs.Add(NpcFactory.CreateNpc(npcId, x, y));
    }

    public void PlaceTreasures(int count)
    {
        // ProspectorKey item logic
        var prospectorKey = Player.Inventory.FirstOrDefault(i => i.Id == ItemId.ProspectorKey);
        if (prospectorKey != null) count += prospectorKey.Value;

        for (int i = 0; i < count; i++)
        {
            int attempts = 0;
            bool placed = false;

            while (!placed && attempts < 100)
            {
                int x = _random.Next(1, GridWidth - 1);
                int y = _random.Next(1, GridHeight - 1);

                int px = GridWidth / 2;
                int py = GridHeight / 2;

                // Avoid center spawn zone
                if (Math.Abs(x - px) <= 3 && Math.Abs(y - py) <= 3)
                {
                    attempts++;
                    continue;
                }

                // Avoid spawning inside any structure
                if (Structures.Any(s => s.Contains(x, y)))
                {
                    attempts++;
                    continue;
                }

                // Avoid treasures too close to each other
                bool tooClose = Treasures.Any(t => Math.Abs(t.X - x) <= 3 && Math.Abs(t.Y - y) <= 3);
                if (tooClose)
                {
                    attempts++;
                    continue;
                }

                Treasures.Add(new Treasure { X = x, Y = y });
                placed = true;
            }
        }
    }

    public void PlaceEnemies(int count)
    {
        int level = Math.Max(1, Player.Steps / 100); // Level scaling
        var enemyBagProbability = EnemyBags.GetByLevel(level); // Weighted bag

        for (int i = 0; i < count; i++)
        {
            if (!TryFindEnemySpawnTile(out int x, out int y, false))
                break;

            var enemy = EnemyFactory.CreateFromBag(enemyBagProbability, x, y, level);
            Enemies.Add(enemy);
        }
    }

    public void PlaceBoss()
    {
        int level = Math.Max(1, Player.Steps / 100); // Level scaling
        var bossBagProbability = BossBags.GetByLevel(level);

        if (!TryFindEnemySpawnTile(out int x, out int y, true))
            return;

        var boss = EnemyFactory.CreateFromBag(bossBagProbability, x, y, level);
        Enemies.Add(boss);
    }

    /// <summary>
    /// Initialize the level with grid, structures, npcs, treasures, player, etc.
    /// </summary>
    private void InitializeLevel()
    {
        InitializeGrid();
        PlacePayer();
        PlaceBaseCamp();
        PlaceNpc(NpcId.Armin, GridWidth / 2 + 1, GridHeight / 2 - 2);
        PlaceTreasures(_difficultyManager.GetTreasuresNumber() + 4);
        // Enemies only appears after 6 steps in the game events GameEngine
    }

    private void InitializeGrid()
    {
        for (int y = 0; y < GridHeight; y++)
        {
            for (int x = 0; x < GridWidth; x++)
            {
                Grid[y, x] = '.';
            }
        }

        for (int x = 0; x < GridWidth; x++)
        {
            Grid[0, x] = '=';
            Grid[GridHeight - 1, x] = '=';
        }

        for (int y = 0; y < GridHeight; y++)
        {
            Grid[y, 0] = '=';
            Grid[y, GridWidth - 1] = '=';
        }
    }

    /// <summary>
    /// Place a player a the center
    /// </summary>
    private void PlacePayer()
    {
        Player = PlayerFactory.CreatePlayer(GridWidth / 2, GridHeight / 2);
    }

    private void PlaceBaseCamp()
    {
        // Place base camp somewhere safe (adjust the position to the map)
        var baseCamp = BaseCampFactory.CreateBaseCamp((GridWidth / 2) - 4, (GridHeight / 2) - 3, 1000);
        Structures.Add(baseCamp);
    }

    /// <summary>
    /// Find a valid tile to spawn an enemy respecting safe zone, structures and occupancy.
    /// </summary>
    private bool TryFindEnemySpawnTile(out int x, out int y, bool isEnemyBoss, int maxAttempts = 100)
    {
        for (int attempts = 0; attempts < maxAttempts; attempts++)
        {
            int sx = _random.Next(1, GridWidth - 1);
            int sy = _random.Next(1, GridHeight - 1);

            // Safe area around the *current* player position
            int safeAreaAroundPlayer = isEnemyBoss ? 10 : 4;
            if (Math.Abs(sx - Player.X) <= safeAreaAroundPlayer && Math.Abs(sy - Player.Y) <= safeAreaAroundPlayer)
                continue;

            // No spawn inside structures (walls or interior)
            if (Structures.Any(s => s.Contains(sx, sy)))
                continue;

            // Avoid overlapping with treasures or enemies
            if (Treasures.Any(t => t.X == sx && t.Y == sy) || Enemies.Any(e => e.X == sx && e.Y == sy))
                continue;

            x = sx;
            y = sy;
            return true;
        }

        x = y = -1;
        return false;
    }

}
