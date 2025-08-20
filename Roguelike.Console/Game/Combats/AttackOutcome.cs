namespace Roguelike.Console.Game.Combats;

/// <summary>
/// Represent the result of a single resolved attack.
/// </summary>
public readonly record struct AttackOutcome(
    bool Dodged,
    int Damage,
    bool Crit,
    int ArmorShredded,
    int LifeStolen,
    int ThornsReflected,
    bool DefenderSavedByTalisman,
    bool TrollMushroomEffect = false)
{
    public static AttackOutcome HasDodged(int restauredLife) => new(true, 0, false, 0, restauredLife, 0, false);
    public static AttackOutcome UnderTrollMushroomEffect() => new(false, 0, false, 0, 0, 0, false, true);
}
