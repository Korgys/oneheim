namespace Roguelike.Console.Game.Characters.Players;

public class PlayerFactory
{
    public static Player CreatePlayer(int x, int y)
    {
        return new Player
        {
            X = x,
            Y = y,
            LifePoint = 10,
            MaxLifePoint = 10,
            Strength = 5,
            Armor = 1,
            Speed = 1,
            Vision = 3
        };
    }
}
