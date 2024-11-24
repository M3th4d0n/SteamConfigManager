using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SteamConfigManager.utils
{
    public static class SteamUserUtils
    {
        public class SteamAccount
        {
            public string SteamID { get; set; }
            public string Name { get; set; }
        }

        public class Game
        {
            public string AppID { get; set; }
            public string Name { get; set; }
        }

        public static List<SteamAccount> GetSteamAccounts(string steamPath)
        {
            var accounts = new List<SteamAccount>();
            string userdataPath = Path.Combine(steamPath, "userdata");

            foreach (var directory in Directory.GetDirectories(userdataPath))
            {
                string steamId = new DirectoryInfo(directory).Name;
                string name = GetSteamName(steamPath, steamId);
                accounts.Add(new SteamAccount { SteamID = steamId, Name = name });
            }

            return accounts;
        }

        public static List<Game> GetGamesForAccount(string steamPath, string steamId)
        {
            var games = new List<Game>();
            string userdataPath = Path.Combine(steamPath, "userdata", steamId);

            foreach (var directory in Directory.GetDirectories(userdataPath))
            {
                string appId = new DirectoryInfo(directory).Name;
                games.Add(new Game { AppID = appId, Name = $"Unknown (AppID: {appId})" });
            }

            return games;
        }


        private static string GetSteamName(string steamPath, string steamId)
        {
            string localConfigPath = Path.Combine(steamPath, "userdata", steamId, "config", "localconfig.vdf");
            if (File.Exists(localConfigPath))
            {
                foreach (var line in File.ReadAllLines(localConfigPath))
                {
                    if (line.Contains("PersonaName"))
                    {
                        return line.Split('"')[3];
                    }
                }
            }
            return "Unknown";
        }
    }
}