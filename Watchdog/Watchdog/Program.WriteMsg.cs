using System;

namespace Watchdog
{
    partial class Program
    {
        private static void Msg(string value)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(value.PadRight(Console.WindowWidth - 1));
            Console.ResetColor();
        }
    }
}