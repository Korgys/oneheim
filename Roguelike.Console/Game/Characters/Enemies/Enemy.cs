namespace Roguelike.Console.Game.Characters.Enemies;

public class Enemy : Character
{
    public Enemy() { }

    public Enemy(int x, int y, int level)
    {
        X = x;
        Y = y;
        Level = level;
        Character = (char)('E' + level - 1);
    }

    public EnemyType Category { get; set; }
    public char Character { get; set; } = 'E'; // Default enemy character
    public string Name { get; set; }
    public int StepsPerTurn { get; set; } = 1; // 0 = static, 1 = normal, 2 = boss/fast
}
