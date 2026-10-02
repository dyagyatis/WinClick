using System.Diagnostics;
using Microsoft.Win32;

namespace WinClickWpf.Services
{
    public class MsiDeviceItem
    {
        public string Name { get; set; } = "";
        public string DevicePath { get; set; } = "";
        public string RegistryPath { get; set; } = "";
        public bool IsMsiSupported { get; set; }
        public string Priority { get; set; } = "Normal";
        public string DeviceType { get; set; } = "GPU / Audio";
    }

    public static class GamingLatencyService
    {
        private const string PciEnumPath = @"SYSTEM\CurrentControlSet\Enum\PCI";

        public static List<MsiDeviceItem> GetMsiDevices()
        {
            var list = new List<MsiDeviceItem>();
            try
            {
                using var pciKey = Registry.LocalMachine.OpenSubKey(PciEnumPath);
                if (pciKey == null) return list;

                foreach (var vendorKeyName in pciKey.GetSubKeyNames())
                {
                    using var vendorKey = pciKey.OpenSubKey(vendorKeyName);
                    if (vendorKey == null) continue;

                    foreach (var devInstanceName in vendorKey.GetSubKeyNames())
                    {
                        using var devKey = vendorKey.OpenSubKey(devInstanceName);
                        if (devKey == null) continue;

                        string devDesc = devKey.GetValue("DeviceDesc")?.ToString() ?? "";
                        string friendlyName = devKey.GetValue("FriendlyName")?.ToString() ?? "";
                        string className = devKey.GetValue("Class")?.ToString() ?? "";

                        string rawName = !string.IsNullOrWhiteSpace(friendlyName) ? friendlyName : devDesc;
                        string displayName = rawName;
                        if (displayName.Contains(";"))
                        {
                            displayName = displayName.Split(';').Last();
                        }
                        if (string.IsNullOrWhiteSpace(displayName) || displayName.StartsWith("@"))
                        {
                            displayName = devInstanceName;
                        }

                        // Filter to relevant hardware: GPU, Audio, Network, USB Controllers
                        bool isRelevant = className.Contains("Display", StringComparison.OrdinalIgnoreCase) ||
                                          className.Contains("MEDIA", StringComparison.OrdinalIgnoreCase) ||
                                          className.Contains("Net", StringComparison.OrdinalIgnoreCase) ||
                                          className.Contains("USB", StringComparison.OrdinalIgnoreCase) ||
                                          displayName.Contains("NVIDIA", StringComparison.OrdinalIgnoreCase) ||
                                          displayName.Contains("Radeon", StringComparison.OrdinalIgnoreCase) ||
                                          displayName.Contains("Intel", StringComparison.OrdinalIgnoreCase) ||
                                          displayName.Contains("Realtek", StringComparison.OrdinalIgnoreCase);

                        if (isRelevant)
                        {
                            string relMsiPath = $@"{PciEnumPath}\{vendorKeyName}\{devInstanceName}\Device Parameters\Interrupt Management\MessageSignaledInterruptProperties";
                            using var msiKey = Registry.LocalMachine.OpenSubKey(relMsiPath);

                            bool isMsi = false;
                            string priority = "Undefined";

                            if (msiKey != null)
                            {
                                isMsi = (msiKey.GetValue("MSISupported") as int? ?? 0) == 1;
                            }

                            string relAffPath = $@"{PciEnumPath}\{vendorKeyName}\{devInstanceName}\Device Parameters\Interrupt Management\Affinity Policy";
                            using var affKey = Registry.LocalMachine.OpenSubKey(relAffPath);
                            if (affKey != null)
                            {
                                int? prioVal = affKey.GetValue("DevicePriority") as int?;
                                priority = prioVal switch
                                {
                                    1 => "Low",
                                    2 => "Normal",
                                    3 => "High",
                                    _ => "Undefined"
                                };
                            }

                            string devType = "⚡ PCI Устройство";
                            if (className.Contains("Display", StringComparison.OrdinalIgnoreCase) ||
                                displayName.Contains("GeForce", StringComparison.OrdinalIgnoreCase) ||
                                displayName.Contains("Radeon", StringComparison.OrdinalIgnoreCase) ||
                                displayName.Contains("RTX", StringComparison.OrdinalIgnoreCase) ||
                                displayName.Contains("GTX", StringComparison.OrdinalIgnoreCase) ||
                                displayName.Contains("Graphics", StringComparison.OrdinalIgnoreCase))
                            {
                                devType = "🎮 Видеокарта";
                            }
                            else if (className.Contains("MEDIA", StringComparison.OrdinalIgnoreCase) ||
                                     displayName.Contains("Audio", StringComparison.OrdinalIgnoreCase) ||
                                     displayName.Contains("Sound", StringComparison.OrdinalIgnoreCase) ||
                                     displayName.Contains("Realtek", StringComparison.OrdinalIgnoreCase))
                            {
                                devType = "🔊 Звуковая карта";
                            }
                            else if (className.Contains("Net", StringComparison.OrdinalIgnoreCase) ||
                                     displayName.Contains("Ethernet", StringComparison.OrdinalIgnoreCase) ||
                                     displayName.Contains("Wireless", StringComparison.OrdinalIgnoreCase) ||
                                     displayName.Contains("Wi-Fi", StringComparison.OrdinalIgnoreCase) ||
                                     displayName.Contains("LAN", StringComparison.OrdinalIgnoreCase))
                            {
                                devType = "🌐 Сетевой адаптер";
                            }
                            else if (displayName.Contains("USB", StringComparison.OrdinalIgnoreCase) ||
                                     className.Contains("USB", StringComparison.OrdinalIgnoreCase))
                            {
                                devType = "🔌 USB Контроллер";
                            }

                            list.Add(new MsiDeviceItem
                            {
                                Name = displayName,
                                DevicePath = $@"{vendorKeyName}\{devInstanceName}",
                                RegistryPath = relMsiPath,
                                IsMsiSupported = isMsi,
                                Priority = priority,
                                DeviceType = devType
                            });
                        }
                    }
                }
            }
            catch { }

            return list;
        }

        public static bool SetMsiMode(MsiDeviceItem item, bool enableMsi, string priority = "High")
        {
            try
            {
                using var msiKey = Registry.LocalMachine.CreateSubKey(item.RegistryPath, true);
                if (msiKey != null)
                {
                    msiKey.SetValue("MSISupported", enableMsi ? 1 : 0, RegistryValueKind.DWord);
                    if (enableMsi)
                    {
                        msiKey.SetValue("MessageNumberLimit", 16, RegistryValueKind.DWord);
                    }
                }

                string affPath = item.RegistryPath.Replace("MessageSignaledInterruptProperties", "Affinity Policy");
                using var affKey = Registry.LocalMachine.CreateSubKey(affPath, true);
                if (affKey != null)
                {
                    int prioVal = priority switch
                    {
                        "Low" => 1,
                        "High" => 3,
                        _ => 2
                    };
                    affKey.SetValue("DevicePriority", prioVal, RegistryValueKind.DWord);
                }

                item.IsMsiSupported = enableMsi;
                item.Priority = priority;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task ApplyBcdeditTimerTweaksAsync(Action<string> log)
        {
            await Task.Run(() =>
            {
                log("Применение BCDedit таймеров сверхнизкой задержки...");
                ProcessRunner.Run("bcdedit.exe", "/set useplatformclock false");
                ProcessRunner.Run("bcdedit.exe", "/set disabledynamictick yes");
                ProcessRunner.Run("bcdedit.exe", "/set tscsyncpolicy Enhanced");
                ProcessRunner.Run("bcdedit.exe", "/set bootux disabled");
                log("✓ Настройки BCDedit применены (useplatformclock=false, disabledynamictick=yes, tscsyncpolicy=Enhanced)");
            });
        }

        public static async Task ApplyGamingNetworkTweaksAsync(Action<string> log)
        {
            await Task.Run(() =>
            {
                log("Оптимизация сетевого стека для онлайн-игр...");

                // Multimedia System Responsiveness & Network Throttling
                RegistryHelper.SetDword(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", "NetworkThrottlingIndex", unchecked((int)0xFFFFFFFF));
                RegistryHelper.SetDword(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", "SystemResponsiveness", 0);

                // Gaming task priority
                RegistryHelper.SetDword(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games", "GPU Priority", 8);
                RegistryHelper.SetDword(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games", "Priority", 6);
                RegistryHelper.SetString(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games", "Scheduling Category", "High");
                RegistryHelper.SetString(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games", "SFIO Priority", "High");

                // TCP Gaming tweaks across interfaces
                try
                {
                    using var interfacesKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces", true);
                    if (interfacesKey != null)
                    {
                        foreach (var subName in interfacesKey.GetSubKeyNames())
                        {
                            using var sub = interfacesKey.OpenSubKey(subName, true);
                            if (sub != null)
                            {
                                sub.SetValue("TcpAckFrequency", 1, RegistryValueKind.DWord);
                                sub.SetValue("TCPNoDelay", 1, RegistryValueKind.DWord);
                                sub.SetValue("TcpDelAckTicks", 0, RegistryValueKind.DWord);
                            }
                        }
                    }
                }
                catch { }

                log("✓ Сетевые параметры онлайн-игр применены (SystemResponsiveness=0, TcpAckFrequency=1, TCPNoDelay=1)");
            });
        }

        public static async Task ApplyUltimateGamingPowerPlanAsync(Action<string> log)
        {
            await Task.Run(() =>
            {
                log("Активация схемы питания «Максимальная производительность» (Ultimate Gaming)...");
                ProcessRunner.Run("powercfg.exe", "-duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61");
                ProcessRunner.Run("powercfg.exe", "/setactive e9a42b02-d5df-448d-aa00-03f14749eb61");

                // Core Unparking (0% min parking)
                RegistryHelper.SetDword(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Power\PowerSettings\54533251-82be-4824-96c1-47b60b740d00\0cc5b647-c1df-4637-891a-dec35c318583", "ValueMax", 0);
                RegistryHelper.SetDword(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Power\PowerSettings\54533251-82be-4824-96c1-47b60b740d00\0cc5b647-c1df-4637-891a-dec35c318583", "ValueMin", 0);

                log("✓ Схема электропитания Ultimate Performance активирована, парковка ядер отключена.");
            });
        }
    }
}
