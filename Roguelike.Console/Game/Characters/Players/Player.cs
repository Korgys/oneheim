using Roguelike.Console.Game.Collectables.Items;

namespace Roguelike.Console.Game.Characters.Players;

public class Player : Character
{
    public static char Character { get; set; } = '@'; // Default player character
    public int MaxInventorySize { get; set; } = 3; // Default maximum inventory size
    public int Steps { get; set; } = 0;

    public int GetNextLevelXP()
    {
        // Simple formula: 20 * Level^2
        return 20 * Level * Level;
    }

    public void GainXP(int amount)
    {
        XP += amount;
        if (XP >= GetNextLevelXP())
        {
            LevelUp();
        }
    }

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
    public void SetPlayerVisionAfterFogArrival(int fogVisionMalus = 1)
    {
        // cannot go below 1, or below the minimal requirement of the GlassesOfClairvoyance
        var glassesOfClairvoyance = Inventory.FirstOrDefault(i => i.Id == ItemId.GlassesOfClairvoyance);
        int minimalPlayerVision = glassesOfClairvoyance != null ? glassesOfClairvoyance.Value : 1;
        Vision = Math.Max(minimalPlayerVision, Vision - fogVisionMalus);
    }
}
