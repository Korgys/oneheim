
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.NPCs.Allies;

public class Mercenary : Character
{
    public override string Name { get; set; } = Messages.OneheimGuard;
    public char Character { get; set; } = 'é';
    public int StepsPerTurn { get; set; } = 1;

    public static Mercenary Create(int x, int y, int playerLevel)
    {
        return new Mercenary
        {
            X = x,
            Y = y,
            MaxLifePoint = 30 + playerLevel * 2,
            LifePoint = 30 + playerLevel * 2,
            Strength = 10 + playerLevel * 2,
            Armor = 15 + playerLevel * 2,
            Speed = 10 + playerLevel,
            Vision = 3
        };
    }
}
