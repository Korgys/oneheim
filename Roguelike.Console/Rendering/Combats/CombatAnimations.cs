namespace Roguelike.Console.Rendering.Combats;

using System;

public class CombatAnimations
{
    public static void Blink(bool isBoss)
    {
        if (isBoss) BlinkBoss();
        else BlinkEnemy();
    }

    public static void BlinkEnemy()
    {
        Console.Clear();
        for (int i = 4; i >= 0; i--)
        {
            for (int y = 0; y < 22; y++)
            {
                for (int x = 0; x < 60; x++)
                    Console.Write(i);
                Console.WriteLine();
            }
            Thread.Sleep(100);
            Console.Clear();
        }
    }

    public static void BlinkBoss()
    {
        Console.Clear();
        bool darkRed = true;
        for (int i = 10; i >= 0; i--)
        {
            for (int y = 0; y < 22; y++)
            {
                for (int x = 0; x < 60; x++)
                    Console.Write(i);
                Console.WriteLine();
            }
            Thread.Sleep(100);
            Console.Clear();
            Console.BackgroundColor = darkRed ? ConsoleColor.Black : ConsoleColor.DarkRed;
            darkRed = !darkRed;
        }
        Console.ResetColor();
    }
}
