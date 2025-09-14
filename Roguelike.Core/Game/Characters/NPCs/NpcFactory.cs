namespace Roguelike.Core.Game.Characters.NPCs;

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
                MaxLifePoint = 300,
                LifePoint = 300,
                Armor = 30,
                Strength = 30,
                Speed = 50,
                X = x,
                Y = y
            },
            NpcId.Ichem => new Npc
            {
                Id = id,
                Name = NpcId.Ichem.ToString(),
                Character = 'î',
                MaxLifePoint = 600,
                LifePoint = 600,
                Armor = 60,
                Strength = 40,
                Speed = 40,
                X = x,
                Y = y
            },
            NpcId.Eber => new Npc
            {
                Id = id,
                Name = NpcId.Eber.ToString(),
                Character = 'ê',
                MaxLifePoint = 750,
                LifePoint = 750,
                Armor = 60,
                Strength = 80,
                Speed = 60,
                X = x,
                Y = y
            },
            NpcId.Omana => new Npc
            {
                Id = id,
                Name = NpcId.Omana.ToString(),
                Character = 'ô',
                X = x,
                Y = y
            },
            _ => throw new ArgumentException("Unknown npc type")
        };
    }
}
