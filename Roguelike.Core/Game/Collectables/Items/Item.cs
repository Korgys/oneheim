namespace Roguelike.Core.Game.Collectables.Items;

public class Item
{
    public ItemId Id { get; set; }
    public string Name { get; set; }
    public string Effect { get; set; }
    public string EffectDescription => string.Format(Effect, Value);
    public string GetEffectDescription(int displayValue) => string.Format(Effect, displayValue);
    public int Value { get; set; }
    public int UpgradableIncrementValue { get; set; }
    public ItemRarity Rarity { get; set; }
}