using Roguelike.Core.Game.Characters.Enemies.Bosses;
using Roguelike.Core.Game.Characters.NPCs;
using Roguelike.Core.Game.Levels;
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Systems.Logics;

public sealed class NpcSpawnSystem : ITurnSystem
{
    public TurnPhase Phase => TurnPhase.AfterEnemiesMove;
    public string? LastMessage { get; private set; }

    public void Update(TurnContext ctx)
    {
        LastMessage = null;
        var level = ctx.Level;
        var steps = level.Player.Steps;

        if (!level.Structures.Any(s => s.Name == Messages.BaseCamp))
            return;

        if (steps == 66 && !level.Npcs.Any(n => n.Id == NpcId.Ichem))
        {
            level.PlaceNpc(NpcId.Ichem);
            LastMessage = Messages.ANewTravelerComesToTheBaseCamp;
        }

        if (steps == 166 && !level.Npcs.Any(n => n.Id == NpcId.Eber))
            PlaceNpc(NpcId.Eber, level);

        if (steps >= 350 && !level.Npcs.Any(n => n.Id == NpcId.Omana))
            PlaceNpc(NpcId.Omana, level);

        if (steps >= 450 && !level.Npcs.Any(n => n.Id == NpcId.Urd))
            PlaceNpc(NpcId.Urd, level);

        if (steps >= 550 && !level.Npcs.Any(n => n.Id == NpcId.Ylva) && !level.Enemies.Any(e => e is Boss))
            PlaceNpc(NpcId.Ylva, level);
    }

    private void PlaceNpc(NpcId npcId, LevelManager level)
    {
        level.PlaceNpc(npcId);
        LastMessage = Messages.ANewTravelerComesToTheBaseCamp;
    }
}
