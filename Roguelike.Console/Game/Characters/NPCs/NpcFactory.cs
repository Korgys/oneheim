namespace Roguelike.Console.Game.Characters.NPCs;

public static class NpcFactory
{
    public static Npc CreateNpc(NpcId id, int x, int y)
    {
        return id switch
        {
            NpcId.Armin => new Npc
            {
                Id = id,
                Name = NpcId.Armin.ToString(),
                Character = 'à',
                X = x,
                Y = y
            },
            NpcId.Ichem => new Npc
            {
                Id = id,
                Name = NpcId.Ichem.ToString(),
                Character = 'î',
                X = x,
                Y = y
            },
            _ => throw new ArgumentException("Unknown npc type")
        };
    }
}
