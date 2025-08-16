using Roguelike.Console.Properties.i18n;

namespace Roguelike.Console.Game.Characters.Enemies.Mobs.Undeads;

public class ArmoredZombie : Enemy
{
    public ArmoredZombie(int x, int y, int level) : base(x, y, level)
    {
        LifePoint = (15 + _random.Next(0, 3)) * level + (level - 1); // Lvl1: 15-17, Lvl2: 30-34, Lvl3: 45-51
        MaxLifePoint = LifePoint;
        Armor = (7 + _random.Next(0, 3)) * level + (level - 1); // Lvl1: 7-9, Lvl2: 14-18, Lvl3: 21-27
        Strength = (7 + _random.Next(0, 3)) * level + (level - 1); // Lvl1: 6-8, Lvl2: 12-16, Lvl3: 18-24
        Speed = 3 * level + (level - 1); // Zombies are slow
        Vision = 2; // Limited vision range
        Name = Messages.ArmoredZombie;
        Category = EnemyType.Undead;
    }
}
