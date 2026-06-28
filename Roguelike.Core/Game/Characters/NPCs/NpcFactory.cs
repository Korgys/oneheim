using Roguelike.Core.Properties.i18n;

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
                Name = Messages.Get("Omana"),
                Character = 'ô',
                MaxLifePoint = 500,
                LifePoint = 500,
                Armor = 40,
                Strength = 20,
                Speed = 60,
                X = x,
                Y = y
            },
            NpcId.Urd => new Npc
            {
                Id = id,
                Name = Messages.Get("Urd"),
                Character = 'ù',
                MaxLifePoint = 450,
                LifePoint = 450,
                Armor = 30,
                Strength = 30,
                Speed = 50,
                X = x,
                Y = y
            },
            NpcId.Ylva => new Npc
            {
                Id = id,
                Name = Messages.Get("Ylva"),
                Character = 'ÿ',
                MaxLifePoint = 500,
                LifePoint = 500,
                Armor = 45,
                Strength = 45,
                Speed = 45,
                X = x,
                Y = y
            },
            _ => throw new ArgumentException(Messages.Get("UnknownNpcType"))
        };
    }
}
