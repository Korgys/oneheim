using Roguelike.Core.Game.Characters.Enemies;
using Roguelike.Core.Game.Characters.NPCs;
using Roguelike.Core.Game.Characters.NPCs.Allies;
using Roguelike.Core.Game.Characters.Players;
using Roguelike.Core.Game.Collectables;
using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Game.Levels;
using Roguelike.Core.Game.Structures;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Roguelike.Core.Game.GameLoop;

//exemple d'utilisation : 
//  // Save
//  var snap = GameSnapshot.Capture(level, rngSeed: 12345);
//  File.WriteAllText(path, snap.ToJson());

//  // Load
//  var loaded = GameSnapshot.FromJson(File.ReadAllText(path));
//  loaded.ApplyTo(level);

/// <summary>
/// Serializable, UI-agnostic save-game snapshot.
/// </summary>
public sealed record GameSnapshot
{
    public int Version { get; init; } = 1;

    // Meta / global
    public int GridWidth { get; init; }
    public int GridHeight { get; init; }
    public int ChestPrice { get; init; }
    public int? RngSeed { get; init; } // optional, useful for deterministic replays

    // Player & world
    public required PlayerSnap Player { get; init; }
    public required List<EnemySnap> Enemies { get; init; } = new();
    public required List<NpcSnap> Npcs { get; init; } = new();
    public required List<MercenarySnap> Mercenaries { get; init; } = new();
    public required List<TreasureSnap> Treasures { get; init; } = new();
    public required List<StructureSnap> Structures { get; init; } = new();

    // -------- Capture / Restore --------

    public static GameSnapshot Capture(LevelManager level, int? rngSeed = null) =>
        new()
        {
            GridWidth = LevelManager.GridWidth,
            GridHeight = LevelManager.GridHeight,
            ChestPrice = level.ChestPrice,
            RngSeed = rngSeed,

            Player = PlayerSnap.From(level.Player),

            Enemies = level.Enemies.Select(EnemySnap.From).ToList(),
            Npcs = level.Npcs.Select(NpcSnap.From).ToList(),
            Mercenaries = level.Mercenaries.Select(MercenarySnap.From).ToList(),
            Treasures = level.Treasures.Select(TreasureSnap.From).ToList(),
            Structures = level.Structures.Select(StructureSnap.From).ToList()
        };

    /// <summary>
    /// Apply this snapshot into an existing LevelManager (clears and refills collections).
    /// </summary>
    public void ApplyTo(LevelManager level)
    {
        // Restore simple globals
        level.ChestPrice = ChestPrice;

        // Player
        Player.ApplyTo(level.Player);

        // Collections
        level.Enemies.Clear();
        level.Enemies.AddRange(Enemies.Select(e => e.ToEnemy()));

        level.Npcs.Clear();
        level.Npcs.AddRange(Npcs.Select(n => n.ToNpc()));

        level.Mercenaries.Clear();
        level.Mercenaries.AddRange(Mercenaries.Select(m => m.ToMercenary()));

        level.Treasures.Clear();
        level.Treasures.AddRange(Treasures.Select(TreasureSnap.ToTreasure));

        level.Structures.Clear();
        //level.Structures.AddRange(Structures.Select(StructureSnap.ToStructure));
    }

    // -------- JSON helpers --------

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public string ToJson() => JsonSerializer.Serialize(this, JsonOpts);

    public static GameSnapshot FromJson(string json) =>
        JsonSerializer.Deserialize<GameSnapshot>(json, JsonOpts)
        ?? throw new InvalidDataException("Invalid save file.");
}

// ============ Snap DTOs ============

public sealed record PlayerSnap
{
    public int X { get; init; }
    public int Y { get; init; }
    public int LifePoint { get; init; }
    public int MaxLifePoint { get; init; }
    public int Strength { get; init; }
    public int Armor { get; init; }
    public int Speed { get; init; }
    public int Vision { get; init; }

    public int Steps { get; init; }
    public int Level { get; init; }
    public int XP { get; init; }
    public int Gold { get; init; }

    public required List<ItemSnap> Inventory { get; init; }

    public static PlayerSnap From(Player p) => new()
    {
        X = p.X,
        Y = p.Y,
        LifePoint = p.LifePoint,
        MaxLifePoint = p.MaxLifePoint,
        Strength = p.Strength,
        Armor = p.Armor,
        Speed = p.Speed,
        Vision = p.Vision,
        Steps = p.Steps,
        Level = p.Level,
        XP = p.XP,
        Gold = p.Gold,
        Inventory = p.Inventory.Select(ItemSnap.From).ToList()
    };

    public void ApplyTo(Player p)
    {
        p.X = X; p.Y = Y;
        p.LifePoint = LifePoint; p.MaxLifePoint = MaxLifePoint;
        p.Strength = Strength; p.Armor = Armor; p.Speed = Speed; p.Vision = Vision;
        p.Steps = Steps; p.Level = Level; p.XP = XP; p.Gold = Gold;

        p.Inventory.Clear();
        p.Inventory.AddRange(Inventory.Select(ItemSnap.ToItem));
    }
}

public sealed record EnemySnap
{
    public required string Name { get; init; }
    public char Character { get; init; }
    public int X { get; init; }
    public int Y { get; init; }

    public int LifePoint { get; init; }
    public int MaxLifePoint { get; init; }
    public int Strength { get; init; }
    public int Armor { get; init; }
    public int Speed { get; init; }
    public int Vision { get; init; }

    public bool IsBoss { get; init; } // simple flag, si utile
    public int StepsPerTurn { get; init; } // si tu l’utilises

    public static EnemySnap From(Enemy e) => new()
    {
        Name = e.Name,
        Character = e.Character,
        X = e.X,
        Y = e.Y,
        LifePoint = e.LifePoint,
        MaxLifePoint = e.MaxLifePoint,
        Strength = e.Strength,
        Armor = e.Armor,
        Speed = e.Speed,
        Vision = e.Vision,
        IsBoss = e is Characters.Enemies.Bosses.Boss,
        StepsPerTurn = e.StepsPerTurn
    };

    public Enemy ToEnemy()
    {
        var e = new Enemy
        {
            Name = Name,
            Character = Character,
            X = X,
            Y = Y,
            LifePoint = LifePoint,
            MaxLifePoint = MaxLifePoint,
            Strength = Strength,
            Armor = Armor,
            Speed = Speed,
            Vision = Vision,
            StepsPerTurn = StepsPerTurn
        };
        return e;
    }
}

public sealed record MercenarySnap
{
    public required string Name { get; init; }
    public char Character { get; init; }
    public int X { get; init; }
    public int Y { get; init; }

    public int LifePoint { get; init; }
    public int MaxLifePoint { get; init; }
    public int Strength { get; init; }
    public int Armor { get; init; }
    public int Speed { get; init; }
    public int Vision { get; init; }

    public static MercenarySnap From(Mercenary m) => new()
    {
        Name = m.Name,
        Character = m.Character,
        X = m.X,
        Y = m.Y,
        LifePoint = m.LifePoint,
        MaxLifePoint = m.MaxLifePoint,
        Strength = m.Strength,
        Armor = m.Armor,
        Speed = m.Speed,
        Vision = m.Vision
    };

    public Mercenary ToMercenary() => new()
    {
        Name = Name,
        Character = Character,
        X = X,
        Y = Y,
        LifePoint = LifePoint,
        MaxLifePoint = MaxLifePoint,
        Strength = Strength,
        Armor = Armor,
        Speed = Speed,
        Vision = Vision
    };
}

public sealed record NpcSnap
{
    public required string Name { get; init; }
    public char Character { get; init; }
    public int X { get; init; }
    public int Y { get; init; }
    public bool HasMet { get; init; }
    public string Id { get; init; } = ""; // si tu as un NpcId, tu peux stocker string/int

    public static NpcSnap From(Npc n) => new()
    {
        Name = n.Name,
        Character = n.Character,
        X = n.X,
        Y = n.Y,
        HasMet = n.HasMet,
        Id = n.Id.ToString()
    };

    public Npc ToNpc()
    {
        var npc = new Npc
        {
            Name = Name,
            Character = Character,
            X = X,
            Y = Y,
            HasMet = HasMet
        };
        // si tu as un enum NpcId, reparse-le ici si besoin
        return npc;
    }
}

public sealed record TreasureSnap
{
    public int X { get; init; }
    public int Y { get; init; }
    public BonusType Type { get; init; }
    public int Value { get; init; }

    public static TreasureSnap From(Treasure t) => new()
    {
        X = t.X,
        Y = t.Y,
        Type = t.Type,
        Value = t.Value
    };

    public static Treasure ToTreasure(TreasureSnap s) => new()
    {
        X = s.X,
        Y = s.Y,
        Type = s.Type,
        Value = s.Value
    };
}

public sealed record StructureSnap
{
    public required string Name { get; init; }
    public int Hp { get; init; }
    public int MaxHp { get; init; }
    public int Left { get; init; }
    public int Top { get; init; }
    public int Right { get; init; }
    public int Bottom { get; init; }

    public static StructureSnap From(Structure s) => new()
    {
        Name = s.Name,
        Hp = s.Hp,
        MaxHp = s.MaxHp
        //Left = s.Left,
        //Top = s.Top,
        //Right = s.Right,
        //Bottom = s.Bottom
    };

    //public static Structure ToStructure(StructureSnap snap) => new()
    //{
    //    Name = snap.Name,
    //    Hp = snap.Hp,
    //    MaxHp = snap.MaxHp
    //    //Left = snap.Left,
    //    //Top = snap.Top,
    //    //Right = snap.Right,
    //    //Bottom = snap.Bottom
    //};
}

public sealed record ItemSnap
{
    public ItemId Id { get; init; }
    public string Name { get; init; } = "";
    public int Value { get; init; }
    public ItemRarity Rarity { get; init; }
    public int UpgradableIncrementValue { get; init; }
    public string EffectDescription { get; init; } = "";

    public static ItemSnap From(Item i) => new()
    {
        Id = i.Id,
        Name = i.Name,
        Value = i.Value,
        Rarity = i.Rarity,
        UpgradableIncrementValue = i.UpgradableIncrementValue,
        EffectDescription = i.EffectDescription
    };

    public static Item ToItem(ItemSnap s)
    {
        var item = ItemFactory.CreateItem(s.Id);
        // overwrite dynamic fields
        item.Value = s.Value;
        item.Rarity = s.Rarity;
        // si ton item possède d’autres champs runtime, applique-les ici
        return item;
    }
}
