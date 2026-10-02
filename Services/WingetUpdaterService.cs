using System.Diagnostics;
using System.IO;

namespace WinClickWpf.Services
{
    public class WingetPackageItem
    {
        public string Name { get; set; } = "";
        public string Id { get; set; } = "";
        public string Version { get; set; } = "";
        public string AvailableVersion { get; set; } = "";
        public bool IsSelected { get; set; } = true;
    }

    public static class WingetUpdaterService
    {
        public static async Task<List<WingetPackageItem>> GetAvailableUpdatesAsync()
        {
            return await Task.Run(() =>
            {
                var list = new List<WingetPackageItem>();
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = "winget",
                        Arguments = "upgrade --include-unknown",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using var proc = Process.Start(psi);
                    if (proc == null) return list;

                    string output = proc.StandardOutput.ReadToEnd();
                    proc.WaitForExit(15000);

                    using var reader = new StringReader(output);
                    string? line;
                    bool startParsing = false;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        if (line.StartsWith("---") || line.Contains("---"))
                        {
                            startParsing = true;
                            continue;
                        }

                        if (!startParsing) continue;

                        // Parse table row
                        var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 4)
                        {
                            string availVer = parts.Last();
                            string curVer = parts[parts.Length - 2];
                            string id = parts[parts.Length - 3];
                            string name = string.Join(" ", parts.Take(parts.Length - 3));

                            if (id.Contains('.') || !string.IsNullOrWhiteSpace(id))
                            {
                                list.Add(new WingetPackageItem
                                {
                                    Name = name,
                                    Id = id,
                                    Version = curVer,
                                    AvailableVersion = availVer,
                                    IsSelected = true
                                });
                            }
                        }
                    }
                }
                catch { }

                return list;
            });
        }

        public static async Task UpgradeAllPackagesAsync(Action<string> log)
        {
            await Task.Run(() =>
            {
                log("=== Запуск пакетного обновления всех программ (Winget) ===");
                log("Сканирование и установка обновлений в тихом режиме...\n");

                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = "winget",
                        Arguments = "upgrade --all --include-unknown --accept-package-agreements --accept-source-agreements",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using var proc = new Process { StartInfo = psi };
                    proc.OutputDataReceived += (s, e) =>
                    {
                        if (!string.IsNullOrWhiteSpace(e.Data)) log(e.Data);
                    };
                    proc.ErrorDataReceived += (s, e) =>
                    {
                        if (!string.IsNullOrWhiteSpace(e.Data)) log(e.Data);
                    };

                    proc.Start();
                    proc.BeginOutputReadLine();
                    proc.BeginErrorReadLine();
                    proc.WaitForExit();

                    log("\n=== Все обновления успешно установлены! ===");
                }
                catch (Exception ex)
                {
                    log($"Ошибка выполнения Winget: {ex.Message}");
                }
            });
        }
    }
}
