using System.Diagnostics;
using System.IO;
using System.Net.Http;
using WinClickWpf.Models;

namespace WinClickWpf.Services
{
    public static class SoftwareInstallerService
    {
        public static List<SoftwareApp> GetCatalog()
        {
            return new List<SoftwareApp>
            {
                // Браузеры
                new SoftwareApp("Google Chrome", "Google.Chrome", "Браузеры", "Быстрый и надежный веб-браузер от Google", true),
                new SoftwareApp("Mozilla Firefox", "Mozilla.Firefox", "Браузеры", "Приватный браузер с открытым исходным кодом", true),
                new SoftwareApp("Brave Browser", "Brave.Brave", "Браузеры", "Быстрый браузер со встроенной блокировкой рекламы", true),
                new SoftwareApp("LibreWolf", "LibreWolf.LibreWolf", "Браузеры", "Максимально приватная сборка Firefox без телеметрии"),
                new SoftwareApp("Vivaldi", "VivaldiTechnologies.Vivaldi", "Браузеры", "Гибко настраиваемый браузер для продвинутых пользователей"),
                new SoftwareApp("Floorp", "Floorp.Floorp", "Браузеры", "Японский быстрый и кастомизируемый форк Firefox"),

                // Архиваторы и утилиты
                new SoftwareApp("7-Zip", "7zip.7zip", "Архиваторы и утилиты", "Высокоэффективный бесплатный архиватор", true),
                new SoftwareApp("WinRAR", "RARLab.WinRAR", "Архиваторы и утилиты", "Популярный мощный архиватор файлов"),
                new SoftwareApp("NanaZip", "M2Team.NanaZip", "Архиваторы и утилиты", "Современный 7-Zip с интеграцией в Windows 11"),
                new SoftwareApp("Everything", "voidtools.Everything", "Архиваторы и утилиты", "Мгновенный поиск файлов и папок по всей системе", true),
                new SoftwareApp("Notepad++", "Notepad++.Notepad++", "Архиваторы и утилиты", "Продвинутый текстовый редактор с подсветкой синтаксиса", true),
                new SoftwareApp("PowerToys", "Microsoft.PowerToys", "Архиваторы и утилиты", "Набор полезных системных утилит от Microsoft", true),
                new SoftwareApp("Total Commander", "Ghisler.TotalCommander", "Архиваторы и утилиты", "Двухпанельный файловый менеджер"),

                // Мессенджеры и стриминг
                new SoftwareApp("Telegram Desktop", "Telegram.TelegramDesktop", "Мессенджеры", "Быстрый и безопасный мессенджер", true),
                new SoftwareApp("Discord", "Discord.Discord", "Мессенджеры", "Голосовой и текстовый чат для геймеров и сообществ", true),
                new SoftwareApp("OBS Studio", "OBSProject.OBSStudio", "Мессенджеры", "Программа для записи видео и прямых трансляций"),
                new SoftwareApp("WhatsApp", "WhatsApp.WhatsApp", "Мессенджеры", "Официальное приложение WhatsApp для ПК"),

                // Медиа и плееры
                new SoftwareApp("VLC Media Player", "VideoLAN.VLC", "Медиа", "Универсальный плеер со всеми встроенными кодеками", true),
                new SoftwareApp("PotPlayer", "Daum.PotPlayer", "Медиа", "Многофункциональный медиаплеер с плавной картинкой"),
                new SoftwareApp("K-Lite Codec Pack", "CodecGuide.K-LiteCodecPack.Standard", "Медиа", "Набор всех необходимых видео и аудио кодеков"),
                new SoftwareApp("AIMP", "AIMP.AIMP", "Медиа", "Качественный аудиопроигрыватель с эквалайзером"),
                new SoftwareApp("Spotify", "Spotify.Spotify", "Медиа", "Популярный стриминговый музыкальный сервис"),
                new SoftwareApp("foobar2000", "PeterPawlowski.foobar2000", "Медиа", "Легковесный аудиофильский аудиоплеер"),

                // Игры и лаунчеры
                new SoftwareApp("Steam", "Valve.Steam", "Игры", "Главная цифровая платформа для ПК-игр", true),
                new SoftwareApp("Epic Games Launcher", "EpicGames.EpicGamesLauncher", "Игры", "Лаунчер Epic Games с регулярными раздачами"),
                new SoftwareApp("Ubisoft Connect", "Ubisoft.Connect", "Игры", "Лаунчер для игр компании Ubisoft"),
                new SoftwareApp("EA App", "ElectronicArts.EADesktop", "Игры", "Платформа для игр Electronic Arts"),
                new SoftwareApp("Battle.net", "Blizzard.BattleNet", "Игры", "Лаунчер для игр Blizzard и Activision"),

                // Диагностика и железо
                new SoftwareApp("CPU-Z", "CPUID.CPU-Z", "Диагностика", "Информация о процессоре, памяти и материнской плате", true),
                new SoftwareApp("GPU-Z", "TechPowerUp.GPU-Z", "Диагностика", "Детальная информация о видеокарте и датчиках", true),
                new SoftwareApp("HWMonitor", "CPUID.HWMonitor", "Диагностика", "Мониторинг температур, напряжений и вентиляторов"),
                new SoftwareApp("CrystalDiskInfo", "CrystalDewWorld.CrystalDiskInfo", "Диагностика", "Проверка состояния и здоровья SSD/HDD накопителей", true),
                new SoftwareApp("MSI Afterburner", "Guru3D.Afterburner", "Диагностика", "Мониторинг FPS, температур и настройка видеокарт"),
                new SoftwareApp("FurMark", "Geeks3D.FurMark", "Диагностика", "Стресс-тест видеокарты ('бублик')"),
                new SoftwareApp("Rufus", "Rufus.Rufus", "Диагностика", "Создание загрузочных USB-флешек с Windows и Linux"),

                // Разработка
                new SoftwareApp("Visual Studio Code", "Microsoft.VisualStudioCode", "Разработка", "Современный легковесный редактор кода", true),
                new SoftwareApp("Git", "Git.Git", "Разработка", "Распределенная система контроля версий"),
                new SoftwareApp("Node.js LTS", "OpenJS.NodeJS.LTS", "Разработка", "Среда выполнения JavaScript"),
                new SoftwareApp("Python 3.12", "Python.Python.3.12", "Разработка", "Интерпретатор языка программирования Python")
            };
        }

        public static async Task<bool> InstallAppAsync(SoftwareApp app, Action<string>? logCallback = null)
        {
            return await Task.Run(() =>
            {
                try
                {
                    logCallback?.Invoke($"[{app.Name}] Запуск установки через winget...");
                    
                    var psi = new ProcessStartInfo
                    {
                        FileName = "winget.exe",
                        Arguments = $"install --id {app.WingetId} -e --silent --accept-package-agreements --accept-source-agreements --disable-interactivity",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        StandardOutputEncoding = System.Text.Encoding.UTF8
                    };

                    using var proc = Process.Start(psi);
                    if (proc != null)
                    {
                        proc.OutputDataReceived += (s, e) =>
                        {
                            if (!string.IsNullOrWhiteSpace(e.Data)) logCallback?.Invoke(e.Data);
                        };
                        proc.ErrorDataReceived += (s, e) =>
                        {
                            if (!string.IsNullOrWhiteSpace(e.Data)) logCallback?.Invoke(e.Data);
                        };

                        proc.BeginOutputReadLine();
                        proc.BeginErrorReadLine();
                        proc.WaitForExit();

                        if (proc.ExitCode == 0 || proc.ExitCode == 3010) // 3010 = reboot required
                        {
                            logCallback?.Invoke($"[{app.Name}] Успешно установлено!");
                            return true;
                        }
                    }

                    logCallback?.Invoke($"[{app.Name}] Winget завершился с кодом, пробуем резервный способ...");
                }
                catch (Exception ex)
                {
                    logCallback?.Invoke($"[{app.Name}] Ошибка: {ex.Message}");
                }

                return false;
            });
        }
    }
}
