using System.Diagnostics;
using System.Text.Json;
using WinClickWpf.Models;

namespace WinClickWpf.Services
{
    public static class UwpManager
    {
        public static async Task<List<UwpAppInfo>> GetInstalledAppsAsync()
        {
            return await Task.Run(() =>
            {
                var list = new List<UwpAppInfo>();
                try
                {
                    string script = @"
$startApps = Get-StartApps
$apps = Get-AppxPackage | Where-Object { -not $_.NonRemovable -and -not $_.IsFramework -and $_.Name -notmatch 'Microsoft.MicrosoftEdge' }
$res = @()
foreach ($app in $apps) {
    $startApp = $startApps | Where-Object { $_.AppID -like ""*$($app.Name)*"" } | Select-Object -First 1
    $displayName = if ($startApp) { $startApp.Name } else { $app.Name }
    $displayName = $displayName -replace '^(MicrosoftWindows|Microsoft)[.\s]+', ''
    if ($app.Name -like '*Edge.GameAssist*') { $displayName = 'GameAssist' }
    $res += [PSCustomObject]@{
        DisplayName = $displayName
        PackageName = $app.Name
        PackageFullName = $app.PackageFullName
    }
}
$res | ConvertTo-Json -Compress
";
                    var psi = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{script.Replace("\"", "\\\"")}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        StandardOutputEncoding = System.Text.Encoding.UTF8
                    };

                    using var proc = Process.Start(psi);
                    if (proc != null)
                    {
                        string json = proc.StandardOutput.ReadToEnd();
                        proc.WaitForExit();

                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            json = json.Trim();
                            if (json.StartsWith("["))
                            {
                                var items = JsonSerializer.Deserialize<List<UwpAppInfoJson>>(json);
                                if (items != null)
                                {
                                    foreach (var item in items)
                                    {
                                        list.Add(new UwpAppInfo
                                        {
                                            DisplayName = item.DisplayName ?? item.PackageName ?? "",
                                            PackageName = item.PackageName ?? "",
                                            PackageFullName = item.PackageFullName ?? "",
                                            IsSelected = true
                                        });
                                    }
                                }
                            }
                            else if (json.StartsWith("{"))
                            {
                                var single = JsonSerializer.Deserialize<UwpAppInfoJson>(json);
                                if (single != null)
                                {
                                    list.Add(new UwpAppInfo
                                    {
                                        DisplayName = single.DisplayName ?? single.PackageName ?? "",
                                        PackageName = single.PackageName ?? "",
                                        PackageFullName = single.PackageFullName ?? "",
                                        IsSelected = true
                                    });
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"GetInstalledAppsAsync error: {ex.Message}");
                }

                return list.OrderBy(x => x.DisplayName).ToList();
            });
        }

        public static void RemoveSelectedApps(IEnumerable<UwpAppInfo> apps, Action<string>? statusCallback = null)
        {
            // Stop relevant helper processes first
            string[] procs = { "msedgewebview2", "widgets", "WidgetService", "WebViewHost" };
            foreach (var p in procs)
            {
                try
                {
                    foreach (var process in Process.GetProcessesByName(p))
                    {
                        process.Kill();
                    }
                }
                catch { }
            }

            foreach (var app in apps)
            {
                if (string.IsNullOrWhiteSpace(app.PackageFullName)) continue;
                statusCallback?.Invoke($"Удаление UWP: {app.DisplayName}...");
                ProcessRunner.RunPowerShell($"Remove-AppxPackage -Package '{app.PackageFullName}' -AllUsers -ErrorAction SilentlyContinue");
            }
        }

        private class UwpAppInfoJson
        {
            public string? DisplayName { get; set; }
            public string? PackageName { get; set; }
            public string? PackageFullName { get; set; }
        }
    }
}
