using System.IO;

namespace Watchdog
{
    partial class Program
    {
        private static void GetGitHubProject(string user, string projectname)
        {
            Msg($"GetGitHubProject: {user} / {projectname}");
            string url = $@"https://github.com/{user}/{projectname}/archive/master.zip";
            var targetPath = Directory.GetCurrentDirectory();
            var sourcePath = Path.GetTempPath() + projectname + "-master";
            var zipFilePath = Path.GetTempFileName();
            var isSuccess = DownloadGithubZipFile(url, zipFilePath);
            if (!isSuccess) return;

            //clean up
            if (Directory.Exists(sourcePath))
                Directory.Delete(sourcePath, true);

            targetPath = Path.Combine(targetPath, projectname);
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            
            ExtractZipToFolder(zipFilePath);
            CopyFolder(sourcePath, targetPath, "node_modules,bin,.git");
            Directory.Delete(sourcePath, true);
            Msg("..completed");
        }
    }
}