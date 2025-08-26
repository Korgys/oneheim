using Roguelike.Console.Properties.i18n;

namespace Roguelike.Console.Game.Characters.Enemies.Mobs.Undeads;

public class Revenant : Enemy
{
    public Revenant(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (20 + _random.Next(0, 5)) * level + (level - 1);
        MaxLifePoint = LifePoint;
        Armor = (8 + _random.Next(0, 3)) * level + (level - 1); 
        Strength = (11 + _random.Next(0, 4)) * level + (level - 1);
        Speed = (4 + _random.Next(0, 2)) * level + (level - 1); // Revenants are faster than regular zombies but still slower than most enemies
        Vision = 2; // normal vision range
        Name = Messages.Revenant;
        Category = EnemyType.Undead;
    }
}
