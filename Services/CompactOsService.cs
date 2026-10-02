using System.Diagnostics;
using System.IO;

namespace WinClickWpf.Services
{
    public static class CompactOsService
    {
        public static string SystemDrive => Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.Windows)) ?? "C:\\";

        public static string[] GetDefaultLzxFolders()
        {
            string drive = SystemDrive;
            return new[]
            {
                Path.Combine(drive, "Program Files"),
                Path.Combine(drive, "Program Files (x86)"),
                Path.Combine(drive, "ProgramData"),
                Path.Combine(drive, "Users"),
                Path.Combine(drive, "Windows")
            };
        }

        public static async Task CompressFullLzxAsync(Action<string> logCallback)
        {
            await Task.Run(() =>
            {
                logCallback("=== [1] Запуск COMPACT OS LZX [LZMS] (для SSD/NVMe) ===");
                logCallback("Включение сжатия ядра CompactOS:always...");
                ProcessRunner.Run("compact.exe", "/CompactOS:always");

                var folders = GetDefaultLzxFolders();
                foreach (var folder in folders)
                {
                    if (Directory.Exists(folder))
                    {
                        logCallback($"\n>>> Сжатие директории LZX: {folder}...");
                        RunCompactProcess($"/c /s:\"{folder}\" /exe:LZX /i /a /f", logCallback);
                    }
                }

                logCallback("\n=== Полное сжатие Compact OS LZX успешно завершено! ===");
            });
        }

        public static async Task CompressNormalAsync(Action<string> logCallback)
        {
            await Task.Run(() =>
            {
                logCallback("=== [2] Запуск COMPACT OS NORMAL ===");
                logCallback("Применение стандартного сжатия Windows CompactOS...");
                RunCompactProcess("/CompactOS:always", logCallback);
                logCallback("\n=== Стандартное сжатие Compact OS завершено! ===");
            });
        }

        public static async Task CompressSpecificFolderAsync(string folderPath, Action<string> logCallback)
        {
            await Task.Run(() =>
            {
                logCallback($"=== [3] Сжатие выбранной папки [LZMS]: {folderPath} ===");
                RunCompactProcess($"/c /s:\"{folderPath}\" /exe:LZX /i /a /f", logCallback);
                logCallback($"\n=== Сжатие папки {Path.GetFileName(folderPath)} завершено! ===");
            });
        }

        public static async Task UncompressWindowsAsync(Action<string> logCallback)
        {
            await Task.Run(() =>
            {
                string winFolder = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                logCallback($"=== [4] Распаковка [Uncompress] {winFolder} ===");
                logCallback("Отключение сжатия ядра CompactOS...");
                ProcessRunner.Run("compact.exe", "/CompactOS:never");

                logCallback("Распаковка файлов Windows...");
                RunCompactProcess($"/u /s:\"{winFolder}\" /i /a /f", logCallback);
                logCallback("\n=== Распаковка C:\\Windows завершена! ===");
            });
        }

        public static async Task UncompressSpecificFolderAsync(string folderPath, Action<string> logCallback)
        {
            await Task.Run(() =>
            {
                logCallback($"=== [5] Распаковка [Uncompress] выбранной папки: {folderPath} ===");
                RunCompactProcess($"/u /s:\"{folderPath}\" /i /a /f", logCallback);
                logCallback($"\n=== Распаковка папки {Path.GetFileName(folderPath)} завершена! ===");
            });
        }

        private static void RunCompactProcess(string arguments, Action<string> logCallback)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "compact.exe",
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = psi };
                process.OutputDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(e.Data))
                    {
                        logCallback(e.Data);
                    }
                };
                process.ErrorDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(e.Data))
                    {
                        logCallback(e.Data);
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                logCallback($"Ошибка compact.exe: {ex.Message}");
            }
        }
    }
}
