using System.IO;
using System;
using System.Linq;

namespace SteamConfigManager.utils
{
    public static class SteamConfigTransfer
    {
        private static readonly string[] IgnoredFiles = { "localconfig.vdf" };

        public static void TransferAllGames(string steamPath, string sourceId, string targetId)
        {
            string sourcePath = Path.Combine(steamPath, "userdata", sourceId);
            string targetPath = Path.Combine(steamPath, "userdata", targetId);

            foreach (var directory in Directory.GetDirectories(sourcePath))
            {
                string destination = Path.Combine(targetPath, new DirectoryInfo(directory).Name);
                CopyDirectory(directory, destination);
            }
        }

        public static void TransferGame(string steamPath, string sourceId, string targetId, string appId)
        {
            string sourcePath = Path.Combine(steamPath, "userdata", sourceId, appId);
            string targetPath = Path.Combine(steamPath, "userdata", targetId, appId);
            CopyDirectory(sourcePath, targetPath);
        }

        private static void CopyDirectory(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);
            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(file);
                
                if (IgnoredFiles.Contains(fileName, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                string targetFile = Path.Combine(targetDir, fileName);
                File.Copy(file, targetFile, true);
            }

            foreach (var directory in Directory.GetDirectories(sourceDir))
            {
                CopyDirectory(directory, Path.Combine(targetDir, new DirectoryInfo(directory).Name));
            }
        }
    }
}