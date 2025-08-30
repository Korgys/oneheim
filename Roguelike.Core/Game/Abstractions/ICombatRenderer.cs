namespace Roguelike.Core.Game.Abstractions;

using Roguelike.Core.Game.Characters.Enemies;
using Roguelike.Core.Game.Characters.Players;
using System.Collections.Generic;

public interface ICombatRenderer
{
    // Called once when combat starts (e.g., blink effect)
    void OnCombatStart(bool isBoss);

    // Called each “tick”/turn to draw state + append log lines
    void RenderTurn(Enemy enemy, Player player, IReadOnlyCollection<string> logLines);

    // Called once when combat ends (show rewards/end screen text)
    void OnCombatEnd(Enemy enemy, Player player, IReadOnlyCollection<string> finalLogLines);
}

// Safe no-op default you can use in tests or headless builds
public sealed class NullCombatRenderer : ICombatRenderer
{
    public void OnCombatStart(bool isBoss) { }
    public void RenderTurn(Enemy enemy, Player player, IReadOnlyCollection<string> logLines) { }
    public void OnCombatEnd(Enemy enemy, Player player, IReadOnlyCollection<string> finalLogLines) { }
}
