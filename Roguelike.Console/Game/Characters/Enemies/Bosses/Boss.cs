namespace Roguelike.Console.Game.Characters.Enemies.Bosses;

public class Boss : Enemy
{
    public Boss(int x, int y, int level) : base(x, y, level)
    {
        Level = level;
        X = x;
        Y = y;
        Character = 'B';
        StepsPerTurn = 2; // boss can move 2 cases per turn
    }
}
