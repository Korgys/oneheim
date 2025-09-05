using Roguelike.Core.Game.Characters.NPCs;

namespace Roguelike.Console.Rendering.Characters;

public static class ConsoleRendererCharacter
{
    public const char Player = '@';
    public const char Treasure = '$';
        
    public const char Enemy = 'E';
    public const char Boss = 'B';

    public const char Mercenary = 'é';

    public static char GetNpcCharacter(NpcId npcId)
    {
        return npcId switch
        {
            NpcId.Armin => 'â',
            NpcId.Ichem => 'î',
            NpcId.Eber => 'ê',
            _ => 'ù'
        };
    }
}
