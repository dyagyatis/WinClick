using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using WinClickWpf.Models;

namespace WinClickWpf.Services
{
    public static class StartupManagerService
    {
        private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string RunOnceKeyPath = @"Software\Microsoft\Windows\CurrentVersion\RunOnce";
        private const string StartupApprovedHkcu = @"Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";
        private const string StartupApprovedHklm = @"Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run32";

        public static async Task<List<StartupItem>> GetStartupItemsAsync()
        {
            return await Task.Run(() =>
            {
                var list = new List<StartupItem>();

                // 1. Programs - HKCU Run & RunOnce
                ScanRegistryKey(Registry.CurrentUser, RunKeyPath, StartupApprovedHkcu, true, "Реестр (HKCU Run)", StartupCategoryType.Programs, list);
                ScanRegistryKey(Registry.CurrentUser, RunOnceKeyPath, null, true, "Реестр (HKCU RunOnce)", StartupCategoryType.Programs, list);

                // 2. Programs - HKLM Run & RunOnce
                ScanRegistryKey(Registry.LocalMachine, RunKeyPath, StartupApprovedHklm, false, "Реестр (HKLM Run)", StartupCategoryType.Programs, list);
                ScanRegistryKey(Registry.LocalMachine, RunOnceKeyPath, null, false, "Реестр (HKLM RunOnce)", StartupCategoryType.Programs, list);

                // 3. Programs - User Startup Folder
                string userStartup = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                ScanStartupFolder(userStartup, true, "Папка Автозагрузка", list);

                // 4. Programs - Common Startup Folder
                string commonStartup = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup);
                ScanStartupFolder(commonStartup, false, "Общая Автозагрузка", list);

                // 5. Scheduled Tasks
                ScanScheduledTasks(list);

                // 6. 3rd-Party Services
                ScanThirdPartyServices(list);

                return list;
            });
        }

        private static void ScanRegistryKey(RegistryKey root, string keyPath, string? approvedPath, bool isCurrentUser, string locType, StartupCategoryType cat, List<StartupItem> list)
        {
            try
            {
                using var key = root.OpenSubKey(keyPath, false);
                if (key == null) return;

                using var approvedKey = approvedPath != null ? root.OpenSubKey(approvedPath, false) : null;

                foreach (var valueName in key.GetValueNames())
                {
                    if (string.IsNullOrWhiteSpace(valueName)) continue;

                    string? cmd = key.GetValue(valueName)?.ToString();
                    if (string.IsNullOrWhiteSpace(cmd)) continue;

                    bool isEnabled = true;
                    if (approvedKey != null)
                    {
                        var val = approvedKey.GetValue(valueName) as byte[];
                        if (val != null && val.Length > 0 && val[0] % 2 != 0)
                        {
                            isEnabled = false;
                        }
                    }

                    string cleanPath = ExtractFilePath(cmd);
                    string pub = ExtractPublisher(cleanPath);
                    string impact = CalculateImpact(valueName, cleanPath);

                    list.Add(new StartupItem
                    {
                        Name = valueName,
                        Command = cmd,
                        CleanFilePath = cleanPath,
                        Publisher = pub,
                        LocationType = locType,
                        RegistryPath = keyPath,
                        OriginalValue = cmd,
                        IsCurrentUser = isCurrentUser,
                        IsEnabled = isEnabled,
                        Category = cat,
                        Impact = impact
                    });
                }
            }
            catch { }
        }

        private static void ScanStartupFolder(string folderPath, bool isCurrentUser, string locType, List<StartupItem> list)
        {
            try
            {
                if (!Directory.Exists(folderPath)) return;

                foreach (var file in Directory.GetFiles(folderPath))
                {
                    string ext = Path.GetExtension(file).ToLower();
                    if (ext == ".lnk" || ext == ".exe" || ext == ".bat" || ext == ".cmd" || ext == ".vbs")
                    {
                        string name = Path.GetFileNameWithoutExtension(file);
                        string pub = ExtractPublisher(file);
                        string impact = CalculateImpact(name, file);

                        list.Add(new StartupItem
                        {
                            Name = name,
                            Command = file,
                            CleanFilePath = file,
                            Publisher = pub.Contains("Неизвестно") ? "Ярлык автозагрузки" : pub,
                            LocationType = locType,
                            RegistryPath = folderPath,
                            OriginalValue = file,
                            IsCurrentUser = isCurrentUser,
                            IsEnabled = true,
                            Category = StartupCategoryType.Programs,
                            Impact = impact
                        });
                    }
                }
            }
            catch { }
        }

        private static void ScanScheduledTasks(List<StartupItem> list)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "schtasks.exe",
                    Arguments = "/query /fo CSV /nh /v",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var proc = Process.Start(psi);
                if (proc == null) return;

                string output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit(3000);

                using var reader = new StringReader(output);
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // Parse CSV line
                    var parts = ParseCsvLine(line);
                    if (parts.Count >= 9)
                    {
                        string taskName = parts[1].Trim('\"', '\\');
                        string statusStr = parts[3].Trim('\"');
                        string author = parts[7].Trim('\"');
                        string taskToRun = parts[8].Trim('\"');

                        // Filter only startup/logon or active 3rd party tasks
                        if (string.IsNullOrWhiteSpace(taskName) || taskName.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase))
                            continue;

                        bool isEnabled = !statusStr.Contains("Disabled", StringComparison.OrdinalIgnoreCase) && !statusStr.Contains("Отключено", StringComparison.OrdinalIgnoreCase);
                        string cleanPath = ExtractFilePath(taskToRun);
                        string pub = !string.IsNullOrWhiteSpace(author) && !author.Contains("N/A") ? author : ExtractPublisher(cleanPath);

                        list.Add(new StartupItem
                        {
                            Name = Path.GetFileName(taskName),
                            Command = taskToRun,
                            CleanFilePath = cleanPath,
                            Publisher = pub,
                            LocationType = "Планировщик задач",
                            TaskPath = taskName,
                            IsEnabled = isEnabled,
                            Category = StartupCategoryType.Tasks,
                            Impact = CalculateImpact(taskName, cleanPath)
                        });
                    }
                }
            }
            catch { }
        }

        private static void ScanThirdPartyServices(List<StartupItem> list)
        {
            try
            {
                using var servicesKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services");
                if (servicesKey == null) return;

                string winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);

                foreach (var serviceName in servicesKey.GetSubKeyNames())
                {
                    try
                    {
                        using var sKey = servicesKey.OpenSubKey(serviceName);
                        if (sKey == null) continue;

                        int? startType = sKey.GetValue("Start") as int?;
                        // 2 = Auto, 3 = Manual, 4 = Disabled
                        if (startType == null || (startType != 2 && startType != 3)) continue;

                        int? serviceType = sKey.GetValue("Type") as int?;
                        // Exclude Drivers (Type 1 = Kernel Driver, Type 2 = File System Driver, Type 8 = Recognizer Driver)
                        if (serviceType != null && serviceType < 16) continue;

                        string? imagePath = sKey.GetValue("ImagePath")?.ToString();
                        if (string.IsNullOrWhiteSpace(imagePath)) continue;

                        // Exclude driver files (.sys) and driver directories
                        if (imagePath.EndsWith(".sys", StringComparison.OrdinalIgnoreCase) ||
                            imagePath.Contains(@"\drivers\", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        string clean = ExtractFilePath(imagePath);
                        if (clean.EndsWith(".sys", StringComparison.OrdinalIgnoreCase)) continue;

                        // Check ServiceDll for svchost services
                        string? serviceDll = null;
                        using (var paramsKey = sKey.OpenSubKey("Parameters"))
                        {
                            serviceDll = paramsKey?.GetValue("ServiceDll")?.ToString();
                        }

                        string targetBinary = !string.IsNullOrWhiteSpace(serviceDll) && clean.Contains("svchost.exe", StringComparison.OrdinalIgnoreCase)
                            ? ExtractFilePath(serviceDll)
                            : clean;

                        // Exclude internal Windows binaries unless explicitly 3rd party
                        if (targetBinary.StartsWith(winDir, StringComparison.OrdinalIgnoreCase))
                        {
                            if (!targetBinary.Contains("Epic", StringComparison.OrdinalIgnoreCase) &&
                                !targetBinary.Contains("Steam", StringComparison.OrdinalIgnoreCase) &&
                                !targetBinary.Contains("NVIDIA", StringComparison.OrdinalIgnoreCase) &&
                                !targetBinary.Contains("Radeon", StringComparison.OrdinalIgnoreCase) &&
                                !targetBinary.Contains("AMD", StringComparison.OrdinalIgnoreCase))
                            {
                                string winPub = ExtractPublisher(targetBinary);
                                if (winPub.Contains("Microsoft", StringComparison.OrdinalIgnoreCase) ||
                                    winPub.Contains("Windows", StringComparison.OrdinalIgnoreCase) ||
                                    winPub.Contains("Майкрософт", StringComparison.OrdinalIgnoreCase) ||
                                    winPub.Contains("Неизвестный", StringComparison.OrdinalIgnoreCase))
                                {
                                    continue;
                                }
                            }
                        }

                        string pub = ExtractPublisher(targetBinary);
                        if (pub.Contains("Microsoft", StringComparison.OrdinalIgnoreCase) ||
                            pub.Contains("Windows", StringComparison.OrdinalIgnoreCase) ||
                            pub.Contains("Майкрософт", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        // Exclude known Microsoft service name prefixes
                        if (serviceName.StartsWith("AppX", StringComparison.OrdinalIgnoreCase) ||
                            serviceName.StartsWith("WdNis", StringComparison.OrdinalIgnoreCase) ||
                            serviceName.StartsWith("WinDefend", StringComparison.OrdinalIgnoreCase) ||
                            serviceName.StartsWith("wuauserv", StringComparison.OrdinalIgnoreCase) ||
                            serviceName.StartsWith("CryptSvc", StringComparison.OrdinalIgnoreCase) ||
                            serviceName.StartsWith("Lanman", StringComparison.OrdinalIgnoreCase) ||
                            serviceName.StartsWith("Rpc", StringComparison.OrdinalIgnoreCase) ||
                            serviceName.StartsWith("Dcom", StringComparison.OrdinalIgnoreCase) ||
                            serviceName.StartsWith("EventLog", StringComparison.OrdinalIgnoreCase) ||
                            serviceName.StartsWith("Schedule", StringComparison.OrdinalIgnoreCase) ||
                            serviceName.StartsWith("PlugPlay", StringComparison.OrdinalIgnoreCase) ||
                            serviceName.StartsWith("Themes", StringComparison.OrdinalIgnoreCase) ||
                            serviceName.StartsWith("MpsSvc", StringComparison.OrdinalIgnoreCase) ||
                            serviceName.StartsWith("BFE", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        string dispName = sKey.GetValue("DisplayName")?.ToString() ?? serviceName;
                        if (dispName.StartsWith("@") && dispName.Contains(","))
                        {
                            // Internal Windows MUI localized resource service
                            continue;
                        }

                        list.Add(new StartupItem
                        {
                            Name = dispName,
                            Command = imagePath,
                            CleanFilePath = clean,
                            Publisher = pub,
                            LocationType = startType == 2 ? "Служба (Автозапуск)" : "Служба (Вручную)",
                            ServiceName = serviceName,
                            IsEnabled = startType == 2,
                            Category = StartupCategoryType.Services,
                            Impact = "Среднее"
                        });
                    }
                    catch { }
                }
            }
            catch { }
        }

        public static async Task<bool> ToggleItemAsync(StartupItem item, bool enable)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (item.Category == StartupCategoryType.Tasks && !string.IsNullOrWhiteSpace(item.TaskPath))
                    {
                        string changeArg = enable ? "/enable" : "/disable";
                        var psi = new ProcessStartInfo
                        {
                            FileName = "schtasks.exe",
                            Arguments = $"/change /tn \"{item.TaskPath}\" {changeArg}",
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };
                        using var p = Process.Start(psi);
                        p?.WaitForExit(2000);
                        item.IsEnabled = enable;
                        return true;
                    }

                    if (item.Category == StartupCategoryType.Services && !string.IsNullOrWhiteSpace(item.ServiceName))
                    {
                        string startVal = enable ? "auto" : "demand";
                        var psi = new ProcessStartInfo
                        {
                            FileName = "sc.exe",
                            Arguments = $"config \"{item.ServiceName}\" start= {startVal}",
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };
                        using var p = Process.Start(psi);
                        p?.WaitForExit(2000);
                        item.IsEnabled = enable;
                        item.LocationType = enable ? "Служба (Автозапуск)" : "Служба (Вручную)";
                        return true;
                    }

                    // Program in Registry: toggle via StartupApproved
                    var root = item.IsCurrentUser ? Registry.CurrentUser : Registry.LocalMachine;
                    string approvedPath = item.IsCurrentUser ? StartupApprovedHkcu : StartupApprovedHklm;

                    using var approvedKey = root.CreateSubKey(approvedPath, true);
                    if (approvedKey != null)
                    {
                        byte[] buffer = enable
                            ? new byte[] { 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
                            : new byte[] { 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                        approvedKey.SetValue(item.Name, buffer, RegistryValueKind.Binary);
                        item.IsEnabled = enable;
                        return true;
                    }

                    return false;
                }
                catch
                {
                    return false;
                }
            });
        }

        public static async Task<bool> DeleteItemAsync(StartupItem item)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (item.Category == StartupCategoryType.Tasks && !string.IsNullOrWhiteSpace(item.TaskPath))
                    {
                        var psi = new ProcessStartInfo
                        {
                            FileName = "schtasks.exe",
                            Arguments = $"/delete /tn \"{item.TaskPath}\" /f",
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };
                        using var p = Process.Start(psi);
                        p?.WaitForExit(2000);
                        return true;
                    }

                    if (item.Category == StartupCategoryType.Services && !string.IsNullOrWhiteSpace(item.ServiceName))
                    {
                        // Stop & disable service
                        var psi = new ProcessStartInfo
                        {
                            FileName = "sc.exe",
                            Arguments = $"config \"{item.ServiceName}\" start= disabled",
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };
                        using var p = Process.Start(psi);
                        p?.WaitForExit(2000);
                        return true;
                    }

                    if (item.LocationType.Contains("Папка"))
                    {
                        if (File.Exists(item.Command))
                        {
                            File.Delete(item.Command);
                            return true;
                        }
                    }

                    var root = item.IsCurrentUser ? Registry.CurrentUser : Registry.LocalMachine;
                    using var key = root.OpenSubKey(item.RegistryPath, true);
                    if (key != null)
                    {
                        key.DeleteValue(item.Name, false);
                        return true;
                    }

                    return false;
                }
                catch
                {
                    return false;
                }
            });
        }

        public static bool AddNewStartupProgram(string name, string exePath)
        {
            try
            {
                using var key = Registry.CurrentUser.CreateSubKey(RunKeyPath, true);
                if (key != null)
                {
                    key.SetValue(name, $"\"{exePath}\"");
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static void OpenFileLocation(StartupItem item)
        {
            try
            {
                string path = !string.IsNullOrWhiteSpace(item.CleanFilePath) ? item.CleanFilePath : ExtractFilePath(item.Command);
                if (File.Exists(path))
                {
                    Process.Start("explorer.exe", $"/select,\"{path}\"");
                }
                else if (Directory.Exists(path))
                {
                    Process.Start("explorer.exe", $"\"{path}\"");
                }
            }
            catch { }
        }

        public static void SearchOnline(StartupItem item)
        {
            try
            {
                string query = Uri.EscapeDataString($"{item.Name} windows process startup");
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"https://www.google.com/search?q={query}",
                    UseShellExecute = true
                });
            }
            catch { }
        }

        public static string ExtractFilePath(string rawCmd)
        {
            if (string.IsNullOrWhiteSpace(rawCmd)) return "";
            string cmd = rawCmd.Trim();

            // Check if quoted
            if (cmd.StartsWith("\""))
            {
                int nextQuote = cmd.IndexOf('\"', 1);
                if (nextQuote > 1)
                {
                    return cmd.Substring(1, nextQuote - 1);
                }
            }

            // Check first space before arguments
            int spaceIdx = cmd.IndexOf(".exe", StringComparison.OrdinalIgnoreCase);
            if (spaceIdx > 0)
            {
                return cmd.Substring(0, spaceIdx + 4).Trim('\"');
            }

            return cmd.Split(' ')[0].Trim('\"');
        }

        private static string ExtractPublisher(string cleanPath)
        {
            try
            {
                if (File.Exists(cleanPath))
                {
                    var vi = FileVersionInfo.GetVersionInfo(cleanPath);
                    if (!string.IsNullOrWhiteSpace(vi.CompanyName))
                    {
                        return vi.CompanyName;
                    }
                    if (!string.IsNullOrWhiteSpace(vi.ProductName))
                    {
                        return vi.ProductName;
                    }
                }
            }
            catch { }

            return "Неизвестный издатель";
        }

        private static string CalculateImpact(string name, string path)
        {
            string combined = $"{name} {path}".ToLower();

            if (combined.Contains("epic") || combined.Contains("steam") || combined.Contains("discord") ||
                combined.Contains("adobe") || combined.Contains("antivirus") || combined.Contains("avast") ||
                combined.Contains("browser") || combined.Contains("yandex") || combined.Contains("chrome"))
            {
                return "Высокое";
            }

            if (combined.Contains("update") || combined.Contains("onedrive") || combined.Contains("dropbox") ||
                combined.Contains("launcher") || combined.Contains("service"))
            {
                return "Среднее";
            }

            return "Низкое";
        }

        private static List<string> ParseCsvLine(string line)
        {
            var list = new List<string>();
            bool inQuotes = false;
            var current = new System.Text.StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '\"')
                {
                    inQuotes = !inQuotes;
                    current.Append(c);
                }
                else if (c == ',' && !inQuotes)
                {
                    list.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }
            list.Add(current.ToString());
            return list;
        }
    }
}
