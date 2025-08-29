namespace Roguelike.Console.Game.Structures;

public class Structure
{
    public string Name { get; }
    public int X { get; }          // top-left grid coord
    public int Y { get; }          // top-left grid coord
    public int Width { get; }
    public int Height { get; }
    public int MaxHp { get; }
    public int Hp { get; set; }

    // Optional explicit entrance tiles (outside side)
    public readonly HashSet<(int x, int y)> EntranceTiles = new();

    // Precomputed absolute coordinates for fast checks
    private readonly HashSet<(int x, int y)> _walls = new();
    private readonly HashSet<(int x, int y)> _interior = new();

    public Structure(string name, int x, int y, string[] layout, HashSet<(int x, int y)> entranceTiles, int baseHp = 1000)
    {
        Name = name;
        X = x; Y = y;
        Height = layout.Length;
        Width = layout[0].Length;
        MaxHp = baseHp;
        Hp = baseHp;

        // Parse layout: 'X' = wall (blocked), ' ' = interior
        for (int row = 0; row < layout.Length; row++)
        {
            var line = layout[row];
            for (int col = 0; col < line.Length; col++)
            {
                char c = line[col];
                int gx = X + col;
                int gy = Y + row;
                if (c == 'X') _walls.Add((gx, gy));
                else _interior.Add((gx, gy)); // only spaces should be in layout
            }
        }

        EntranceTiles = entranceTiles;
    }

    public bool IsWall(int gx, int gy) => _walls.Contains((gx, gy));
    public bool IsInterior(int gx, int gy) => _interior.Contains((gx, gy));
    public bool Contains(int gx, int gy) => IsWall(gx, gy) || IsInterior(gx, gy);
    public void TakeDamage(int amount) => Hp = Math.Max(0, Hp - Math.Max(0, amount));
    public bool IsSeverelyEndomaged() => (Hp * 100) / MaxHp <= 10;
    public IEnumerable<(int x, int y)> WallTiles() => _walls;
}

