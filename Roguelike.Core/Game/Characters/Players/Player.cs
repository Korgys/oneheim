using Roguelike.Core.Game.Collectables.Items;
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Game.Characters.Players;

public class Player : Character
{
    public override string Name { get; set; } = Messages.Player;
    public static char Character { get; set; } = '@'; // Default player character
    public int MaxInventorySize { get; set; } = 3; // Default maximum inventory size
    public int Steps { get; set; } = 0;

    /// <summary>
    /// Calculates the experience points required to reach the next level.
    /// </summary>
    /// <returns>The total experience points required to advance to the next level, based on the current level.</returns>
    public int GetNextLevelXP()
    {
        // Simple formula: 20 * Level^2
        return 20 * Level * Level;
    }

    /// <summary>
    /// Adds experience points to the character and checks for level-up conditions.
    /// </summary>
    /// <param name="amount"></param>
    public void GainXP(int amount)
    {
        XP += amount;
        if (XP >= GetNextLevelXP())
        {
            LevelUp();
        }
    }

    /// <summary>
    /// Increases the character's level and improves their attributes based on the highest current stat.
    /// </summary>
    /// <remarks>This method increments the character's level and adjusts their attributes accordingly: - The
    /// highest stat among Armor, Speed, and Strength is increased by a calculated amount. - Maximum life points and
    /// current life points are also increased proportionally.</remarks>
    public void LevelUp()
    {
        Level++;
        int gain = 1 + Level / 4;

        if (Armor >= Strength && Armor >= Speed) Armor += gain; // Increase Armor if it's the highest stat
        else if (Speed >= Armor && Speed >= Strength) Speed += gain; // Increase Speed if it's the highest stat
        else Strength += gain; // Otherwise, increase Strength

        MaxLifePoint += gain * 2; // Increase max HP on level up
        LifePoint += gain * 2;
    }

    /// <summary>
    /// Set the player vision when the for arrives. The fog minus 1 vision of the player, except some specific rules.
    /// </summary>
    public void SetPlayerVision(int vision)
    {
        // cannot go below 1, or below the minimal requirement of the GlassesOfClairvoyance
        var glassesOfClairvoyance = Inventory.FirstOrDefault(i => i.Id == ItemId.GlassesOfClairvoyance);
        int minimalPlayerVision = glassesOfClairvoyance != null ? glassesOfClairvoyance.Value : 1;
        Vision = Math.Max(minimalPlayerVision, vision);
    }
}
