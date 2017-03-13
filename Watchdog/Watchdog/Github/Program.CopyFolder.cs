using System;
using System.IO;
using System.Linq;
namespace Watchdog
{
    partial class Program
    {

        public static bool CopyFolder(string src, string dest, string excludeDir = "", string excludeFileName = "")
        {
            Msg($"Copying source: {src} to");
            Msg($"Target: {dest} to");
            src = src.EndsWith(@"\") ? src : src + @"\";
            dest = dest.EndsWith(@"\") ? dest : dest + @"\";
            try
            {
                if (Directory.Exists(src))
                {
                    if (Directory.Exists(dest) == false)
                    {
                        Directory.CreateDirectory(dest);
                    }

                    foreach (var files in Directory.GetFiles(src))
                    {
                        var fileInfo = new FileInfo(files);
                        if (!string.IsNullOrWhiteSpace(excludeFileName))
                        {
                            if (excludeFileName.ToLower().Contains(fileInfo.Name.ToLower().Replace(fileInfo.Extension, ""))) continue;
                        }

                        Console.WriteLine($"{fileInfo.FullName}");
                        fileInfo.CopyTo($@"{dest}\{fileInfo.Name}", true);
                    }

                    return !(from drs in Directory.GetDirectories(src)
                             let directoryInfo = new DirectoryInfo(drs)
                             where !excludeDir.ToLower().Contains(directoryInfo.Name.ToLower())
                             where CopyFolder(drs, dest + directoryInfo.Name, excludeDir, excludeFileName) == false
                             select drs).Any();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*********ERROR:{ex.Message}******");
                return false;
            }
        }
    }
}