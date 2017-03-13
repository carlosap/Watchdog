using System;
namespace Watchdog
{
    partial class Program
    {
        static void Main(string[] args)
        {
            ShowHeader();
            var response = Console.ReadLine();
            do
            {
                switch (response)
                {
                    case "1":
                    case "github":
                        Console.Write(">Enter github username: ");
                        var userName = Console.ReadLine();
                        Console.Write(">Enter project name: ");
                        var projectName = Console.ReadLine();
                        GetGitHubProject(userName, projectName);
                        response = string.Empty;
                        break;
                    case "payless":
                        response = string.Empty;
                        TestPayless();
                        break;
                    case "date":
                        response = string.Empty;
                        Console.WriteLine(DateTime.Now.ToShortDateString());
                        break;
                    case "time":
                        response = string.Empty;
                        Console.WriteLine(DateTime.Now.ToShortTimeString());
                        break;
                    case "?":
                    case "help":
                        ShowHeader();
                        response = Console.ReadLine().ToLower();
                        break;
                    case "clear":
                    case "cls":
                        Console.Clear();
                        response = Console.ReadLine().ToLower();
                        break;
                    default:
                        Console.Write(">");
                        response = Console.ReadLine();
                        break;
                }
            } while (response != "exit");
        }

    }
}
