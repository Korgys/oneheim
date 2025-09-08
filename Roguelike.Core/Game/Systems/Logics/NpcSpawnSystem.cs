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

        // Only spawn NPCs if the base camp exists
        if (!level.Structures.Any(s => s.Name == Messages.BaseCamp))
            return;

        // Spawn Ichem (shop NPC) at 150 steps
        if (steps == 66 && !level.Npcs.Any(n => n.Id == NpcId.Ichem))
        {
            level.PlaceNpc(NpcId.Ichem);
            LastMessage = Messages.ANewTravelerComesToTheBaseCamp;
        }
        // Spawn Eber (mercenary NPC) at 250 steps
        if (steps == 166 && !level.Npcs.Any(n => n.Id == NpcId.Eber))
        {
            PlaceNpc(NpcId.Eber, level);
        }
    }

    private void PlaceNpc(NpcId npcId, LevelManager level)
    {
        level.PlaceNpc(npcId);
        LastMessage = Messages.ANewTravelerComesToTheBaseCamp;
    }
}
