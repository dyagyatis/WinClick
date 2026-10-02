using System.IO;
using System.Text.Json;

namespace WinClickWpf.Services
{
    public class AppConfig
    {
        public string Language { get; set; } = "ru"; // "ru" or "en"
        public string Theme { get; set; } = "system"; // "dark", "light", "system"
    }

    public static class AppConfigService
    {
        private static readonly string ConfigDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WinClick");
        private static readonly string ConfigPath = Path.Combine(ConfigDir, "config.json");
        private static AppConfig? _current;

        public static AppConfig Current
        {
            get
            {
                if (_current == null)
                {
                    Load();
                }
                return _current!;
            }
        }

        public static void Load()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    string json = File.ReadAllText(ConfigPath);
                    _current = JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
                    return;
                }
            }
            catch { }

            _current = new AppConfig();
        }

        public static void Save()
        {
            try
            {
                if (!Directory.Exists(ConfigDir))
                {
                    Directory.CreateDirectory(ConfigDir);
                }
                string json = JsonSerializer.Serialize(_current, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ConfigPath, json);
            }
            catch { }
        }
    }
}
