using System;
using System.Net;
namespace Watchdog
{
    partial class Program
    {
        public static bool DownloadGithubZipFile(string sourceUrl, string destinationPath)
        {
            Msg($"loading project: {sourceUrl}");
            bool results;
            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(sourceUrl, destinationPath);
                    Msg($"Download completed....{destinationPath}");
                }
                results = true;
            }
            catch (Exception ex)
            {
                Error(ex.Message);
                Error("make sure username and project name are correct");
                Error("Network connection failed to download project from Github");
                results = false;
            }
            return results;
        }
    }
}