using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using WinClickWpf.Models;

namespace WinClickWpf.Services
{
    public static class JunkwareScannerService
    {
        private record JunkSignature(string Pattern, string Risk, string Reason, bool MatchPublisher = false);

        private static readonly List<JunkSignature> Signatures = new()
        {
            // Браузеры с рекламой / трекерами / принудительным DNS
            new JunkSignature(@"Yandex\s*(Browser)?|Яндекс\s*Браузер|Кнопка\s*Яндекс", "High", "Браузер с принудительным DNS/DoH, ломающим обход замедления YouTube и Discord, а также множеством фоновых служб"),
            new JunkSignature(@"Opera\s*GX", "Medium", "Навязчивый браузер со встроенной рекламой, партнерскими трекерами и повышенным расходом ресурсов"),
            new JunkSignature(@"Opera\s*Stable|Opera\s*Browser|Opera\s*developer", "Medium", "Браузер с предустановленными рекламными закладками и трекерами"),
            new JunkSignature(@"CCleaner\s*Browser", "High", "Навязчиво устанавливаемый браузер от Avast/Piriform"),
            new JunkSignature(@"Амиго|Amigo|Браузер\s*Комета", "High", "Рекламный браузер (Adware/PUP), встраивающийся в систему"),

            // Антивирусные триалы и псевдо-защита
            new JunkSignature(@"McAfee", "High", "Навязчивый антивирус-триал, сильно нагружающий процессор и накопитель"),
            new JunkSignature(@"Norton\s*(360|Security|LifeLock|AntiVirus)", "High", "Ресурсоемкий антивирус с постоянными рекламными предложениями платных подписок"),
            new JunkSignature(@"Avast\s*(Free|Antivirus|Premier)|AVG\s*Antivirus", "High", "Антивирус с агрессивным сбором телеметрии и навязчивыми уведомлениями"),
            new JunkSignature(@"RAV\s*(Antivirus|Endpoint)|Segurazo", "High", "Скрытно устанавливаемый псевдо-антивирус (PUP/Adware), мешающий работе системы"),
            new JunkSignature(@"ByteFence|SpyHunter|TotalAV", "High", "Сомнительный псевдо-сканер, требующий платную подписку для удаления мнимых угроз"),

            // Псевдо-оптимизаторы и сомнительные автоустановщики
            new JunkSignature(@"Driver\s*Booster|Advanced\s*SystemCare|IObit", "High", "Сомнительный чистильщик реестра и драйверов, навязывающий платный софт"),
            new JunkSignature(@"DriverPack|Driver\s*Pack", "High", "Установщик мусорного и партнерского софта под видом обновления драйверов"),
            new JunkSignature(@"Driver\s*Easy|Driver\s*Genius", "Medium", "Платный навязчивый апдейтер драйверов"),
            new JunkSignature(@"Auslogics\s*(BoostSpeed|Registry|Driver)", "Medium", "Псевдо-оптимизатор с рекламными модулями"),
            new JunkSignature(@"WinZip\s*Driver|WinZip\s*System", "Medium", "Навязчивые платные оптимизаторы WinZip"),

            // Рекламные тулбары и поисковые угонщики
            new JunkSignature(@"Bing\s*Bar|Yahoo\s*Toolbar|Ask\s*Toolbar", "High", "Рекламный тулбар, перехватывающий поисковые запросы браузеров"),
            new JunkSignature(@"Спутник\s*Mail|Mail\.Ru|Поиск\s*Mail", "High", "Рекламные сервисы Mail.Ru, меняющие домашние страницы и поиск"),
            new JunkSignature(@"Web\s*Discover|Search\s*Protect", "High", "Рекламный бар на рабочем столе и перехватчик поиска"),

            // OEM Bloatware
            new JunkSignature(@"HP\s*(Support\s*Assistant|JumpStarts|Touchpoint|Audio\s*Switch)", "Medium", "Предустановленный OEM-хлам HP, потребляющий оперативную память в фоне"),
            new JunkSignature(@"Dell\s*(SupportAssist|Command|Digital\s*Delivery)", "Medium", "OEM-агент Dell с регулярным сбором телеметрии и фоновыми службами"),
            new JunkSignature(@"Lenovo\s*(Welcome|Now|Vantage\s*Service)", "Medium", "Предустановленный софт Lenovo с фоновыми процессами и рекламой"),
            new JunkSignature(@"ASUS\s*(Giftbox|Live\s*Update|Splendid)", "Medium", "Предустановленные рекламные утилиты ASUS"),
            new JunkSignature(@"Acer\s*(Care\s*Center|Configuration\s*Manager)", "Medium", "OEM-агент Acer, замедляющий запуск Windows"),
            new JunkSignature(@"WildTangent\s*Games", "Medium", "Устаревшие рекламные игровые демо-версии")
        };

        public static async Task<List<JunkwareItem>> ScanInstalledJunkwareAsync()
        {
            return await Task.Run(() =>
            {
                var detected = new List<JunkwareItem>();
                var seenNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                // Check 64-bit and 32-bit registry paths
                ScanRegistryKey(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", RegistryView.Registry64, detected, seenNames);
                ScanRegistryKey(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", RegistryView.Registry32, detected, seenNames);
                ScanRegistryKey(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Uninstall", RegistryView.Default, detected, seenNames);

                return detected.OrderByDescending(d => d.RiskLevel == "High").ThenBy(d => d.Name).ToList();
            });
        }

        private static void ScanRegistryKey(RegistryHive hive, string subKeyPath, RegistryView view, List<JunkwareItem> detected, HashSet<string> seenNames)
        {
            try
            {
                using var baseKey = RegistryKey.OpenBaseKey(hive, view);
                using var key = baseKey.OpenSubKey(subKeyPath);
                if (key == null) return;

                foreach (var subName in key.GetSubKeyNames())
                {
                    try
                    {
                        using var appKey = key.OpenSubKey(subName);
                        if (appKey == null) continue;

                        string? displayName = appKey.GetValue("DisplayName")?.ToString()?.Trim();
                        if (string.IsNullOrEmpty(displayName)) continue;

                        string? publisher = appKey.GetValue("Publisher")?.ToString()?.Trim() ?? "";
                        string? version = appKey.GetValue("DisplayVersion")?.ToString()?.Trim() ?? "";
                        string? uninstallString = appKey.GetValue("UninstallString")?.ToString()?.Trim() ?? "";
                        string? quietUninstallString = appKey.GetValue("QuietUninstallString")?.ToString()?.Trim();
                        int isSystemComponent = (int)(appKey.GetValue("SystemComponent") ?? 0);

                        if (isSystemComponent == 1 || string.IsNullOrEmpty(uninstallString)) continue;
                        if (seenNames.Contains(displayName)) continue;

                        // Match against junk signatures
                        foreach (var sig in Signatures)
                        {
                            bool matches = Regex.IsMatch(displayName, sig.Pattern, RegexOptions.IgnoreCase) ||
                                           (sig.MatchPublisher && !string.IsNullOrEmpty(publisher) && Regex.IsMatch(publisher, sig.Pattern, RegexOptions.IgnoreCase));

                            if (matches)
                            {
                                seenNames.Add(displayName);
                                detected.Add(new JunkwareItem
                                {
                                    Name = displayName,
                                    Publisher = publisher,
                                    Version = version,
                                    UninstallString = uninstallString,
                                    QuietUninstallString = quietUninstallString,
                                    RiskLevel = sig.Risk,
                                    Reason = sig.Reason,
                                    RegistryPath = $@"{hive}\{subKeyPath}\{subName}",
                                    IsSelected = true
                                });
                                break;
                            }
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error scanning registry: {ex.Message}");
            }
        }

        public static async Task<bool> UninstallJunkwareAsync(JunkwareItem item, Action<string>? logCallback = null)
        {
            return await Task.Run(() =>
            {
                try
                {
                    logCallback?.Invoke($"[{item.Name}] Запуск тихого удаления...");

                    // 1. If QuietUninstallString is specified
                    if (!string.IsNullOrWhiteSpace(item.QuietUninstallString))
                    {
                        int code = ExecuteUninstallCommand(item.QuietUninstallString);
                        if (code == 0 || code == 3010) return true;
                    }

                    // 2. If UninstallString contains MsiExec.exe or GUID
                    string uninst = item.UninstallString.Trim();
                    var msiMatch = Regex.Match(uninst, @"\{[0-9A-Fa-f\-]{36}\}");
                    if (msiMatch.Success)
                    {
                        string guid = msiMatch.Value;
                        logCallback?.Invoke($"[{item.Name}] Удаление через MSI: {guid}...");
                        int code = ProcessRunner.Run("msiexec.exe", $"/x {guid} /qn /norestart", true, true);
                        if (code == 0 || code == 3010) return true;
                    }

                    // 3. Try InnoSetup / NSIS / InstallShield / Executable silent flags
                    string silentCmd = MakeSilentUninstallCommand(uninst);
                    logCallback?.Invoke($"[{item.Name}] Выполнение: {silentCmd}...");
                    int exitCode = ExecuteUninstallCommand(silentCmd);
                    if (exitCode == 0 || exitCode == 3010) return true;

                    // 4. Fallback: try winget uninstall
                    logCallback?.Invoke($"[{item.Name}] Пробуем winget uninstall...");
                    int wingetCode = ProcessRunner.Run("winget.exe", $"uninstall \"{item.Name}\" --silent --accept-source-agreements --disable-interactivity", true, true);
                    if (wingetCode == 0 || wingetCode == 3010) return true;

                    return true;
                }
                catch (Exception ex)
                {
                    logCallback?.Invoke($"[{item.Name}] Ошибка при удалении: {ex.Message}");
                    return false;
                }
            });
        }

        private static string MakeSilentUninstallCommand(string rawCommand)
        {
            string cmd = rawCommand.Trim();

            // Strip quotes if needed to check executable extension
            if (cmd.StartsWith("\""))
            {
                int nextQuote = cmd.IndexOf('"', 1);
                if (nextQuote > 1)
                {
                    string exe = cmd.Substring(0, nextQuote + 1);
                    string args = cmd.Substring(nextQuote + 1).Trim();

                    // If Inno Setup / NSIS
                    if (!args.Contains("/SILENT", StringComparison.OrdinalIgnoreCase) &&
                        !args.Contains("/S", StringComparison.OrdinalIgnoreCase))
                    {
                        return $"{exe} /VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SILENT /S /qn {args}";
                    }
                    return cmd;
                }
            }

            return $"{cmd} /VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SILENT /S /qn";
        }

        private static int ExecuteUninstallCommand(string fullCommand)
        {
            try
            {
                return ProcessRunner.RunTrustedInstaller(fullCommand, true);
            }
            catch
            {
                return ProcessRunner.RunCmd(fullCommand, true);
            }
        }
    }
}
