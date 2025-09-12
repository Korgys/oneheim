namespace Roguelike.Core.Utils;

/// <summary>
/// Add extension methods to the Random class.
/// </summary>
public static class RandomExtensions
{
    public static T NextWeighted<T>(this Random random, Dictionary<T, int> weights)
    {
        if (weights == null || weights.Count == 0)
            throw new ArgumentException("Le dictionnaire des poids ne peut pas être vide", nameof(weights));

        int totalWeight = weights.Values.Sum();
        int roll = random.Next(totalWeight); // [0, totalWeight)

        int cumulative = 0;
        foreach (var kvp in weights)
        {
            cumulative += kvp.Value;
            if (roll < cumulative)
                return kvp.Key;
        }

        // Should never reach here
        throw new InvalidOperationException("Erreur lors du tirage pondéré");
    }
}

