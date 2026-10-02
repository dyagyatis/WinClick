using System.Diagnostics;
using System.ServiceProcess;
using Microsoft.Win32;

namespace WinClickWpf.Services
{
    public enum ServiceProfileType
    {
        Safe,
        Gaming,
        Extreme
    }

    public class ServiceProfileItem
    {
        public string ServiceName { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Description { get; set; } = "";
        public ServiceProfileType MinimumProfile { get; set; }
    }

    public static class ServicesOptimizerService
    {
        public static readonly List<ServiceProfileItem> TargetServices = new()
        {
            // SAFE PROFILE (Telemetry, CEIP, WER)
            new ServiceProfileItem { ServiceName = "DiagTrack", DisplayName = "Connected User Experiences and Telemetry", Description = "Служба телеметрии и диагностических данных Microsoft", MinimumProfile = ServiceProfileType.Safe },
            new ServiceProfileItem { ServiceName = "dmwappushservice", DisplayName = "Device Management Wireless Application", Description = "Маршрутизация push-сообщений телеметрии WAP", MinimumProfile = ServiceProfileType.Safe },
            new ServiceProfileItem { ServiceName = "WerSvc", DisplayName = "Windows Error Reporting Service", Description = "Служба отчетов об ошибках Windows", MinimumProfile = ServiceProfileType.Safe },
            new ServiceProfileItem { ServiceName = "PcaSvc", DisplayName = "Program Compatibility Assistant", Description = "Помощник по совместимости программ", MinimumProfile = ServiceProfileType.Safe },
            new ServiceProfileItem { ServiceName = "diagnosticshub.standardcollector.service", DisplayName = "Microsoft (R) Diagnostics Hub Standard Collector", Description = "Сбор диагностических логов в реальном времени", MinimumProfile = ServiceProfileType.Safe },

            // GAMING PROFILE (Search indexing, Biometrics, Sensors, Remote)
            new ServiceProfileItem { ServiceName = "WSearch", DisplayName = "Windows Search", Description = "Индексация поиска файлов (разгружает диск и процессор)", MinimumProfile = ServiceProfileType.Gaming },
            new ServiceProfileItem { ServiceName = "WbioSrvc", DisplayName = "Windows Biometric Service", Description = "Служба биометрии Windows Hello (отпечатки/лицо)", MinimumProfile = ServiceProfileType.Gaming },
            new ServiceProfileItem { ServiceName = "lfsvc", DisplayName = "Geolocation Service", Description = "Служба геолокации и определения местоположения", MinimumProfile = ServiceProfileType.Gaming },
            new ServiceProfileItem { ServiceName = "RemoteRegistry", DisplayName = "Remote Registry", Description = "Удаленный реестр (потенциальная уязвимость)", MinimumProfile = ServiceProfileType.Gaming },
            new ServiceProfileItem { ServiceName = "SCardSvr", DisplayName = "Smart Card", Description = "Служба смарт-карт (электронных ключей)", MinimumProfile = ServiceProfileType.Gaming },
            new ServiceProfileItem { ServiceName = "SensorService", DisplayName = "Sensor Service", Description = "Служба датчиков ориентации экрана и света", MinimumProfile = ServiceProfileType.Gaming },
            new ServiceProfileItem { ServiceName = "Fax", DisplayName = "Fax Service", Description = "Служба факсов", MinimumProfile = ServiceProfileType.Gaming },
            new ServiceProfileItem { ServiceName = "RetailDemo", DisplayName = "Retail Demo Service", Description = "Демонстрационный режим для магазинов", MinimumProfile = ServiceProfileType.Gaming },

            // EXTREME PROFILE (Superlite, SysMain, Tablet, Maps)
            new ServiceProfileItem { ServiceName = "SysMain", DisplayName = "SysMain (Superfetch)", Description = "Кэширование приложений в RAM (рекомендуется отключать на SSD)", MinimumProfile = ServiceProfileType.Extreme },
            new ServiceProfileItem { ServiceName = "MapsBroker", DisplayName = "Downloaded Maps Manager", Description = "Менеджер загруженных автономных карт", MinimumProfile = ServiceProfileType.Extreme },
            new ServiceProfileItem { ServiceName = "wisvc", DisplayName = "Windows Insider Service", Description = "Служба программы предварительной оценки Windows", MinimumProfile = ServiceProfileType.Extreme },
            new ServiceProfileItem { ServiceName = "TroubleshootingSvc", DisplayName = "Recommended Troubleshooting Service", Description = "Служба рекомендуемого устранения неполадок", MinimumProfile = ServiceProfileType.Extreme },
            new ServiceProfileItem { ServiceName = "SharedAccess", DisplayName = "Internet Connection Sharing (ICS)", Description = "Общий доступ к подключению к Интернету", MinimumProfile = ServiceProfileType.Extreme },
        };

        public static async Task ApplyServiceProfileAsync(ServiceProfileType profile, Action<string> log)
        {
            await Task.Run(() =>
            {
                log($"=== Применение профиля служб: {profile} ===");

                var toDisable = TargetServices.Where(s => s.MinimumProfile <= profile).ToList();

                foreach (var item in toDisable)
                {
                    try
                    {
                        log($"Отключение: {item.DisplayName} ({item.ServiceName})...");
                        SetServiceStartMode(item.ServiceName, "disabled");
                        StopServiceIfExists(item.ServiceName);
                    }
                    catch (Exception ex)
                    {
                        log($"Предупреждение по службе {item.ServiceName}: {ex.Message}");
                    }
                }

                log($"\n✓ Профиль «{profile}» успешно применен! Отключено служб: {toDisable.Count}.");
            });
        }

        public static async Task RestoreAllServicesToDefaultAsync(Action<string> log)
        {
            await Task.Run(() =>
            {
                log("=== Восстановление всех служб в стандартное состояние Windows ===");

                foreach (var item in TargetServices)
                {
                    try
                    {
                        log($"Восстановление: {item.DisplayName} ({item.ServiceName})...");
                        string defaultMode = item.ServiceName switch
                        {
                            "DiagTrack" => "auto",
                            "WSearch" => "auto",
                            "SysMain" => "auto",
                            "RemoteRegistry" => "demand",
                            "Fax" => "demand",
                            _ => "demand"
                        };

                        SetServiceStartMode(item.ServiceName, defaultMode);
                    }
                    catch (Exception ex)
                    {
                        log($"Ошибка при восстановлении {item.ServiceName}: {ex.Message}");
                    }
                }

                log("\n✓ Все службы успешно возвращены в стандартные значения Windows!");
            });
        }

        private static void SetServiceStartMode(string serviceName, string startMode)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "sc.exe",
                Arguments = $"config \"{serviceName}\" start= {startMode}",
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var p = Process.Start(psi);
            p?.WaitForExit(2000);
        }

        private static void StopServiceIfExists(string serviceName)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "sc.exe",
                    Arguments = $"stop \"{serviceName}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var p = Process.Start(psi);
                p?.WaitForExit(2000);
            }
            catch { }
        }
    }
}
