using System.IO;
using System.IO.Compression;

namespace Watchdog
{
    partial class Program
    {
        private static void ExtractZipToFolder(string zipFilePath)
        {
            var tempFolder = Path.GetTempPath();
            Msg($"Extracting: {zipFilePath} to {tempFolder}");
            ZipFile.ExtractToDirectory(zipFilePath,tempFolder);
            Msg($"Cleaning up previous downloads....{zipFilePath}");
            File.Delete(zipFilePath);
        }
    }
}