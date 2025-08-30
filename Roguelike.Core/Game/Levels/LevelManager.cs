namespace Roguelike.Core.Game.Levels;

using Roguelike.Core.Configuration;
using Roguelike.Core.Game.Characters.Enemies;
using Roguelike.Core.Game.Characters.Enemies.Bosses;
using Roguelike.Core.Game.Characters.NPCs;
using Roguelike.Core.Game.Characters.NPCs.Allies;
using Roguelike.Core.Game.Characters.Players;
using Roguelike.Core.Game.Collectables;
using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Game.Structures;
using Roguelike.Core.Properties.i18n;
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
    public List<Mercenary> Mercenaries { get; } = new();
    public bool PlayerInCombat { get; set; } = false;
    public int ChestPrice { get; set; } = 50;

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

    public void PlaceNpc(NpcId id)
    {
        // Try inside first if a structure exists
        var inside = FindFreeTile(preferInterior: true);
        var (x, y) = inside ?? FindFreeTile(preferInterior: false) ?? (GridWidth / 2, GridHeight / 2);
        Npcs.Add(NpcFactory.CreateNpc(id, x, y));
    }

    private (int x, int y)? FindFreeTile(bool preferInterior)
    {
        var tries = 200;
        var rnd = _random; // assume you already have _random
        var s = Structures.FirstOrDefault();

        while (tries-- > 0)
        {
            int x = rnd.Next(1, GridWidth - 1);
            int y = rnd.Next(1, GridHeight - 1);

            if (s.EntranceTiles.Any(e => e.x == x && e.y == y)) continue; // avoid entrances

            bool inInterior = s != null && s.IsInterior(x, y);

            if (preferInterior && s != null && !inInterior) continue;
            if (!preferInterior && s != null && s.Contains(x, y)) continue; // avoid walls when outside

            bool blocked =
                Enemies.Any(e => e.X == x && e.Y == y) ||
                Treasures.Any(t => t.X == x && t.Y == y) ||
                Npcs.Any(n => n.X == x && n.Y == y) ||
                x <= 0 || x >= GridWidth - 1 || y <= 0 || y >= GridHeight - 1;

            if (!blocked) return (x, y);
        }
        return null;
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

        // GrolMokbarRing and TalismanOfPeace items logic
        int variance = 0;
        var grolMokbarRing = Player.Inventory.FirstOrDefault(i => i.Id == ItemId.GrolMokbarRing);
        var talismanOfPeace = Player.Inventory.FirstOrDefault(i => i.Id == ItemId.TalismanOfPeace);
        if (grolMokbarRing != null) variance += grolMokbarRing.Value;
        if (talismanOfPeace != null) variance = Math.Max(1, variance - talismanOfPeace.Value);

        for (int i = 0; i < count + variance; i++)
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

    public bool TryHireMercenaries(int count, out int hired, out string reason)
    {
        hired = 0;
        reason = string.Empty;

        // Find the base camp
        var baseCamp = Structures.FirstOrDefault(s => s.Name == Messages.BaseCamp);
        if (baseCamp == null)
        {
            reason = "No base camp to defend.";
            return false;
        }

        // Find exterior tiles adjacent to the camp walls for spawn
        var spawnTiles = GetExteriorPerimeterSpots(baseCamp).ToList();
        if (spawnTiles.Count == 0)
        {
            reason = "No available perimeter spots.";
            return false;
        }

        // Limit total mercenaries (optional safety)
        int maxMercs = 12;
        if (Mercenaries.Count >= maxMercs)
        {
            reason = "Too many mercenaries already recruited.";
            return false;
        }

        // Try to place up to 'count' mercs
        foreach (var tile in spawnTiles)
        {
            if (hired >= count) break;
            if (IsOccupied(tile.x, tile.y)) continue;

            var m = Mercenary.Create(tile.x, tile.y, Player.Level);
            Mercenaries.Add(m);
            hired++;
        }

        if (hired == 0)
            reason = "Could not place any mercenary at the perimeter.";

        return true;
    }

    /// <summary>
    /// Return free tiles that are outside the structure but adjacent to its walls
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    private IEnumerable<(int x, int y)> GetExteriorPerimeterSpots(Structure s)
    {
        foreach (var w in s.WallTiles())
        {
            var nbs = new (int x, int y)[] { (w.x + 1, w.y), (w.x - 1, w.y), (w.x, w.y + 1), (w.x, w.y - 1) };
            foreach (var n in nbs)
            {
                // outside
                if (s.IsInterior(n.x, n.y)) continue;
                else if (s.IsWall(n.x, n.y)) continue;
                // inside bounds and free-ish
                //if (n.x <= 0 || n.x >= GridWidth - 1 || n.y <= 0 || n.y >= GridHeight - 1) continue;
                if (!IsOccupied(n.x, n.y))
                    yield return n;
            }
        }
    }

    /// <summary>
    /// Consider treasures, enemies, player, mercenaries as blockers
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private bool IsOccupied(int x, int y)
    {
        if (Player.X == x && Player.Y == y) return true;
        if (Treasures.Any(t => t.X == x && t.Y == y)) return true;
        if (Enemies.Any(e => e.X == x && e.Y == y)) return true;
        if (Mercenaries.Any(m => m.X == x && m.Y == y)) return true;
        // walls are naturally blocked by your movement checks
        return false;
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
