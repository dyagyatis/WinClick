using System.IO;
using System.Text.Json;
using WinClickWpf.ViewModels;

namespace WinClickWpf.Services
{
    public class PresetFileModel
    {
        public string PresetName { get; set; } = "My Preset";
        public string CreatedAt { get; set; } = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
        public List<string> SelectedTags { get; set; } = new();
    }

    public static class PresetBackupService
    {
        public static async Task<bool> CreateRestorePointAsync(string description, Action<string> log)
        {
            return await Task.Run(() =>
            {
                try
                {
                    log("Создание контрольной точки восстановления Windows...");
                    string psCmd = $"Checkpoint-Computer -Description '{description}' -RestorePointType 'MODIFY_SETTINGS' -ErrorAction Stop";
                    ProcessRunner.RunPowerShell(psCmd);
                    log("✓ Точка восстановления Windows успешно создана!");
                    return true;
                }
                catch (Exception ex)
                {
                    log($"Ошибка создания точки восстановления: {ex.Message}");
                    return false;
                }
            });
        }

        public static bool ExportPresetToFile(MainViewModel vm, string filePath, string presetName = "Пользовательский пресет")
        {
            try
            {
                var selectedTags = vm.Categories
                    .SelectMany(c => c.Items)
                    .Where(i => i.IsChecked)
                    .Select(i => i.Tag)
                    .ToList();

                var model = new PresetFileModel
                {
                    PresetName = presetName,
                    CreatedAt = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
                    SelectedTags = selectedTags
                };

                string json = JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool ImportPresetFromFile(MainViewModel vm, string filePath)
        {
            try
            {
                if (!File.Exists(filePath)) return false;

                string json = File.ReadAllText(filePath);
                var model = JsonSerializer.Deserialize<PresetFileModel>(json);
                if (model?.SelectedTags == null) return false;

                var tagSet = new HashSet<string>(model.SelectedTags);

                foreach (var cat in vm.Categories)
                {
                    foreach (var item in cat.Items)
                    {
                        item.IsChecked = tagSet.Contains(item.Tag);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void ApplyQuickPreset(MainViewModel vm, string presetType)
        {
            var tagSet = new HashSet<string>();

            switch (presetType.ToLower())
            {
                case "gaming":
                    // Gaming & Maximum Performance preset
                    tagSet = new HashSet<string>
                    {
                        "/CompressOS", "/CompactOSFast", "/CleanWinSxS", "/ClearStandbyList", "/DisableHibernate", "/DisableReservedStorage",
                        "/RemoveAppx", "/RemoveOneDrive", "/CleanStartMenu", "/RemoveDefender", "/RemoveComponents", "/DisableTasks",
                        "/FastFolders", "/DisableVBS", "/DisableGameDVR", "/UltimatePerformance", "/DisableWUDrivers", "/DisableUAC",
                        "/KillFreezeApps", "/TakeOwnership", "/ClassicContextMenu", "/DarkTheme"
                    };
                    break;

                case "privacy":
                    // Maximum Privacy preset
                    tagSet = new HashSet<string>
                    {
                        "/RemoveAppx", "/RemoveOneDrive", "/RemoveEdge", "/RemoveEdgeWebView", "/RemoveComponents",
                        "/DisableTasks", "/DisableWUDrivers", "/DisableDefenderUpdates", "/PauseUpdates", "/DisableAutoUpdates",
                        "/DisableNotificationsAds", "/TTL", "/HideRecommended"
                    };
                    break;

                case "office":
                    // Safe Office Workstation preset
                    tagSet = new HashSet<string>
                    {
                        "/CleanWinSxS", "/ClearStandbyList", "/RemoveUpdateFiles", "/RemoveStoreCache", "/RemoveExplorerCache",
                        "/FastFolders", "/BoostIconCache", "/DarkTheme", "/ShowExtensions", "/ExplorerThisPC"
                    };
                    break;

                case "clean":
                    // Uncheck all
                    tagSet.Clear();
                    break;
            }

            foreach (var cat in vm.Categories)
            {
                foreach (var item in cat.Items)
                {
                    item.IsChecked = tagSet.Contains(item.Tag);
                }
            }
        }
    }
}
