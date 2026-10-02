using Microsoft.Win32;
using System.Diagnostics;

namespace WinClickWpf.Services
{
    public static class RegistryHelper
    {
        public static void SetDword(RegistryHive hive, string subKey, string valueName, int value, RegistryView view = RegistryView.Default)
        {
            try
            {
                using var baseKey = RegistryKey.OpenBaseKey(hive, view);
                using var key = baseKey.CreateSubKey(subKey, true);
                key?.SetValue(valueName, value, RegistryValueKind.DWord);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"RegistryHelper SetDword error: {ex.Message}");
            }
        }

        public static void SetString(RegistryHive hive, string subKey, string valueName, string value, RegistryValueKind kind = RegistryValueKind.String, RegistryView view = RegistryView.Default)
        {
            try
            {
                using var baseKey = RegistryKey.OpenBaseKey(hive, view);
                using var key = baseKey.CreateSubKey(subKey, true);
                key?.SetValue(valueName, value, kind);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"RegistryHelper SetString error: {ex.Message}");
            }
        }

        public static void SetBinary(RegistryHive hive, string subKey, string valueName, byte[] value, RegistryView view = RegistryView.Default)
        {
            try
            {
                using var baseKey = RegistryKey.OpenBaseKey(hive, view);
                using var key = baseKey.CreateSubKey(subKey, true);
                key?.SetValue(valueName, value, RegistryValueKind.Binary);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"RegistryHelper SetBinary error: {ex.Message}");
            }
        }

        public static void DeleteValue(RegistryHive hive, string subKey, string valueName, RegistryView view = RegistryView.Default)
        {
            try
            {
                using var baseKey = RegistryKey.OpenBaseKey(hive, view);
                using var key = baseKey.OpenSubKey(subKey, true);
                key?.DeleteValue(valueName, false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"RegistryHelper DeleteValue error: {ex.Message}");
            }
        }

        public static void DeleteSubKeyTree(RegistryHive hive, string subKey, RegistryView view = RegistryView.Default)
        {
            try
            {
                using var baseKey = RegistryKey.OpenBaseKey(hive, view);
                baseKey.DeleteSubKeyTree(subKey, false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"RegistryHelper DeleteSubKeyTree error: {ex.Message}");
            }
        }
    }
}
