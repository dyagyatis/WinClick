using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using WinClickWpf.Models;

namespace WinClickWpf.Services
{
    public static class SystemInfoService
    {
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetPhysicallyInstalledSystemMemory(out ulong totalMemoryInKilobytes);

        public static async Task<SystemSpecs> GetSpecsAsync()
        {
            return await Task.Run(() =>
            {
                var specs = new SystemSpecs();

                // 1. OS & Uptime
                specs.PcName = Environment.MachineName;
                specs.UserName = Environment.UserName;
                specs.OsArchitecture = Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit";

                var uptimeSpan = TimeSpan.FromMilliseconds(Environment.TickCount64);
                specs.Uptime = $"{uptimeSpan.Days} дн. {uptimeSpan.Hours} ч. {uptimeSpan.Minutes} мин.";

                try
                {
                    using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                    if (key != null)
                    {
                        string prodName = key.GetValue("ProductName")?.ToString() ?? "Windows";
                        string displayVer = key.GetValue("DisplayVersion")?.ToString() ?? "";
                        string currentBuild = key.GetValue("CurrentBuild")?.ToString() ?? "";
                        string ubr = key.GetValue("UBR")?.ToString() ?? "";

                        // In Windows 11, ProductName may still say Windows 10
                        if (int.TryParse(currentBuild, out int bNum) && bNum >= 22000)
                        {
                            prodName = prodName.Replace("Windows 10", "Windows 11");
                        }

                        specs.OsName = $"{prodName} {displayVer}".Trim();
                        specs.OsBuild = $"Сборка {currentBuild}.{ubr}";
                    }
                }
                catch { }

                // 2. CPU
                try
                {
                    using var cpuKey = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0");
                    if (cpuKey != null)
                    {
                        specs.CpuName = cpuKey.GetValue("ProcessorNameString")?.ToString()?.Trim() ?? "Процессор";
                    }
                    specs.CpuCores = $"{Environment.ProcessorCount} логических ядер";
                }
                catch { }

                // 3. RAM
                try
                {
                    if (GetPhysicallyInstalledSystemMemory(out ulong memKb))
                    {
                        double memGb = memKb / (1024.0 * 1024.0);
                        specs.RamTotal = $"{Math.Round(memGb)} ГБ ({memGb:F1} ГБ)";
                    }
                    else
                    {
                        specs.RamTotal = $"{Environment.WorkingSet / (1024 * 1024 * 1024)} ГБ";
                    }
                }
                catch
                {
                    specs.RamTotal = "Не определено";
                }

                // 4. Drives / Storage
                try
                {
                    var driveStrings = new List<string>();
                    foreach (var drive in DriveInfo.GetDrives())
                    {
                        if (drive.IsReady && drive.DriveType == DriveType.Fixed)
                        {
                            double freeGb = drive.TotalFreeSpace / (1024.0 * 1024 * 1024);
                            double totalGb = drive.TotalSize / (1024.0 * 1024 * 1024);
                            string label = string.IsNullOrEmpty(drive.VolumeLabel) ? "Локальный диск" : drive.VolumeLabel;
                            driveStrings.Add($"{drive.Name} ({label}) — {freeGb:F1} ГБ свободно из {totalGb:F0} ГБ");
                        }
                    }
                    specs.StorageInfo = string.Join("\n", driveStrings);
                }
                catch
                {
                    specs.StorageInfo = "Диски не определены";
                }

                // 5. GPU & Motherboard via Registry (Fast & Native)
                try
                {
                    var gpus = new List<string>();
                    using var videoClassKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}");
                    if (videoClassKey != null)
                    {
                        foreach (var subKeyName in videoClassKey.GetSubKeyNames())
                        {
                            if (subKeyName.StartsWith("000"))
                            {
                                using var sub = videoClassKey.OpenSubKey(subKeyName);
                                string? desc = sub?.GetValue("DriverDesc")?.ToString();
                                if (!string.IsNullOrWhiteSpace(desc) && !desc.Contains("Basic", StringComparison.OrdinalIgnoreCase))
                                {
                                    gpus.Add(desc);
                                }
                            }
                        }
                    }
                    if (gpus.Count > 0) specs.GpuName = string.Join(" + ", gpus.Distinct());
                    else specs.GpuName = "Дискретный / Встроенный видеоадаптер";
                }
                catch
                {
                    specs.GpuName = "Базовый видеоадаптер";
                }

                try
                {
                    using var biosKey = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\BIOS");
                    if (biosKey != null)
                    {
                        string mfg = biosKey.GetValue("BaseBoardManufacturer")?.ToString()?.Trim() ?? "";
                        string prod = biosKey.GetValue("BaseBoardProduct")?.ToString()?.Trim() ?? "";
                        specs.Motherboard = $"{mfg} {prod}".Trim();
                        if (string.IsNullOrWhiteSpace(specs.Motherboard)) specs.Motherboard = "Системная плата";
                    }
                }
                catch
                {
                    specs.Motherboard = "Системная плата";
                }

                return specs;
            });
        }
    }
}
