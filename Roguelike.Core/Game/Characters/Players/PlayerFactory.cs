namespace Roguelike.Core.Game.Characters.Players;

public class PlayerFactory
{
    public static Player CreatePlayer(int x, int y)
    {
        return new Player
        {
            X = x,
            Y = y,
            LifePoint = 12,
            MaxLifePoint = 12,
            Strength = 5,
            Armor = 1,
            Speed = 1,
            Vision = 4
        };
    }
}
