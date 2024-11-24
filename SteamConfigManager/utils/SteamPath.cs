using Microsoft.Win32;

namespace SteamConfigManager.utils
{
    public static class SteamPath
    {
        public static string GetSteamPath()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam"))
                {
                    return key?.GetValue("SteamPath") as string;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}