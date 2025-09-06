using Roguelike.Core.Game.Abstractions;
using Roguelike.Core.Game.Characters.Enemies;
using Roguelike.Core.Game.Characters.Players;
using Roguelike.Core.Game.Combat;

namespace Roguelike.Core.Tests.Fakes;

public sealed class FakeCombatRenderer : ICombatRenderer
{
    public void BlinkConsole(bool isBoss = false) { }
    public void RenderFight(object enemy, object player) { }
    public void RenderEndFight(object enemy, object player, IEnumerable<string> log) { }

    public void OnCombatStart(bool isBoss)
    {
    }

    public void RenderTurn(Enemy enemy, Player player, IReadOnlyCollection<string> logLines)
    {
    }

    public void OnCombatEnd(Enemy enemy, Player player, IReadOnlyCollection<string> finalLogLines, CombatReport combatReport)
    {
    }
}