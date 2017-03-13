using System;

namespace Watchdog
{
    partial class Program
    {
        private static void Error(string value)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(value.PadRight(Console.WindowWidth - 1));
            Console.ResetColor();
        }
    }
}