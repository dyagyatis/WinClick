using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using WinClickWpf.Models;

namespace WinClickWpf.Services
{
    public static class DiskCleanerService
    {
        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        private static extern uint SHEmptyRecycleBin(IntPtr hwnd, string? pszRootPath, uint dwFlags);

        private const uint SHERB_NOCONFIRMATION = 0x00000001;
        private const uint SHERB_NOPROGRESSUI = 0x00000002;
        private const uint SHERB_NOSOUND = 0x00000004;

        public static List<CleanableCategory> GetDefaultCategories()
        {
            string userTemp = Path.GetTempPath();
            string sysTemp = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp");
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);

            return new List<CleanableCategory>
            {
                new CleanableCategory
                {
                    Id = "UserTemp",
                    Title = "Временные файлы пользователя (%TEMP%)",
                    Description = "Остаточные кэши приложений, временные распаковщики и логи",
                    IconSvgPath = "M19 4h-3.5l-1-1h-5l-1 1H5v2h14M6 19a2 2 0 002 2h8a2 2 0 002-2V7H6v12z",
                    TargetDirectories = new() { userTemp }
                },
                new CleanableCategory
                {
                    Id = "SysTemp",
                    Title = "Системные временные файлы (Windows Temp)",
                    Description = "Временные файлы служб и системных процессов Windows",
                    IconSvgPath = "M19.14 12.94c.04-.3.06-.61.06-.94 0-.32-.02-.64-.07-.94l2.03-1.58c.18-.14.23-.41.12-.61l-1.92-3.32c-.12-.22-.37-.29-.59-.22l-2.39.96c-.5-.38-1.03-.7-1.62-.94l-.36-2.54c-.04-.24-.24-.41-.48-.41h-3.84c-.24 0-.43.17-.47.41l-.36 2.54c-.59.24-1.13.57-1.62.94l-2.39-.96c-.22-.08-.47 0-.59.22L2.74 8.87c-.12.21-.08.47.12.61l2.03 1.58c-.05.3-.09.63-.09.94s.02.64.07.94l-2.03 1.58c-.18.14-.23.41-.12.61l1.92 3.32c.12.22.37.29.59.22l2.39-.96c.5.38 1.03.7 1.62.94l.36 2.54c.05.24.24.41.48.41h3.84c.24 0 .44-.17.47-.41l.36-2.54c.59-.24 1.13-.56 1.62-.94l2.39.96c.22.08.47 0 .59-.22l1.92-3.32c.12-.22.07-.47-.12-.61l-2.01-1.58zM12 15.6c-1.98 0-3.6-1.62-3.6-3.6s1.62-3.6 3.6-3.6 3.6 1.62 3.6 3.6-1.62 3.6-3.6 3.6z",
                    TargetDirectories = new() { sysTemp }
                },
                new CleanableCategory
                {
                    Id = "UpdateCache",
                    Title = "Кэш загрузок Центра обновления",
                    Description = "Уже установленные установочные пакеты обновлений Windows",
                    IconSvgPath = "M12 4V1L8 5l4 4V6c3.31 0 6 2.69 6 6 0 1.01-.25 1.97-.7 2.8l1.46 1.46A7.93 7.93 0 0020 12c0-4.42-3.58-8-8-8zm0 14c-3.31 0-6-2.69-6-6 0-1.01.25-1.97.7-2.8L5.24 7.74A7.93 7.93 0 004 12c0 4.42 3.58 8 8 8v3l4-4-4-4v3z",
                    TargetDirectories = new() { Path.Combine(winDir, "SoftwareDistribution", "Download") }
                },
                new CleanableCategory
                {
                    Id = "ShaderCache",
                    Title = "Кэш шейдеров DirectX и видеокарт",
                    Description = "Устаревшие скомпилированные шейдеры игр и приложений",
                    IconSvgPath = "M21 6H3c-1.1 0-2 .9-2 2v8c0 1.1.9 2 2 2h18c1.1 0 2-.9 2-2V8c0-1.1-.9-2-2-2zm-10 7H8v3H6v-3H3v-2h3V8h2v3h3v2zm4.5 2c-.83 0-1.5-.67-1.5-1.5s.67-1.5 1.5-1.5 1.5.67 1.5 1.5-.67 1.5-1.5 1.5zm4-3c-.83 0-1.5-.67-1.5-1.5S18.67 9 19.5 9s1.5.67 1.5 1.5-.67 1.5-1.5 1.5z",
                    TargetDirectories = new()
                    {
                        Path.Combine(localAppData, "D3DSCache"),
                        Path.Combine(localAppData, "NVIDIA", "DXCache"),
                        Path.Combine(localAppData, "AMD", "DxCache"),
                        Path.Combine(localAppData, "Intel", "ShaderCache")
                    }
                },
                new CleanableCategory
                {
                    Id = "CrashDumps",
                    Title = "Отчеты об ошибках и дампы памяти",
                    Description = "Логи сбоев приложений Windows Error Reporting (WER) и Minidump",
                    IconSvgPath = "M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-2h2v2zm0-4h-2V7h2v6z",
                    TargetDirectories = new()
                    {
                        Path.Combine(localAppData, "CrashDumps"),
                        Path.Combine(localAppData, "Microsoft", "Windows", "WER"),
                        Path.Combine(winDir, "Minidump")
                    }
                },
                new CleanableCategory
                {
                    Id = "RecycleBin",
                    Title = "Корзина Windows",
                    Description = "Удаленные файлы со всех дисков",
                    IconSvgPath = "M6 19c0 1.1.9 2 2 2h8c1.1 0 2-.9 2-2V7H6v12zM19 4h-3.5l-1-1h-5l-1 1H5v2h14V4z",
                    TargetDirectories = new()
                }
            };
        }

        public static async Task ScanCategoryAsync(CleanableCategory category)
        {
            await Task.Run(() =>
            {
                if (category.Id == "RecycleBin")
                {
                    category.Bytes = GetRecycleBinSize();
                    return;
                }

                long total = 0;
                foreach (var dir in category.TargetDirectories)
                {
                    if (Directory.Exists(dir))
                    {
                        total += CalculateDirectorySize(new DirectoryInfo(dir));
                    }
                }
                category.Bytes = total;
            });
        }

        public static async Task<long> CleanCategoryAsync(CleanableCategory category, Action<string>? logCallback = null)
        {
            return await Task.Run(() =>
            {
                long cleaned = 0;

                if (category.Id == "RecycleBin")
                {
                    try
                    {
                        long size = GetRecycleBinSize();
                        SHEmptyRecycleBin(IntPtr.Zero, null, SHERB_NOCONFIRMATION | SHERB_NOPROGRESSUI | SHERB_NOSOUND);
                        logCallback?.Invoke("Корзина успешно очищена.");
                        category.Bytes = 0;
                        return size;
                    }
                    catch (Exception ex)
                    {
                        logCallback?.Invoke($"Ошибка очистки корзины: {ex.Message}");
                        return 0;
                    }
                }

                foreach (var dirPath in category.TargetDirectories)
                {
                    if (!Directory.Exists(dirPath)) continue;

                    var di = new DirectoryInfo(dirPath);

                    // Delete files safely
                    try
                    {
                        var stack = new Stack<DirectoryInfo>();
                        stack.Push(di);

                        while (stack.Count > 0)
                        {
                            var currentDir = stack.Pop();

                            try
                            {
                                foreach (var file in currentDir.GetFiles())
                                {
                                    try
                                    {
                                        long len = file.Length;
                                        file.Attributes = FileAttributes.Normal;
                                        file.Delete();
                                        cleaned += len;
                                    }
                                    catch { }
                                }
                            }
                            catch { }

                            try
                            {
                                foreach (var sub in currentDir.GetDirectories())
                                {
                                    // Skip reparse points / junctions
                                    if ((sub.Attributes & FileAttributes.ReparsePoint) == 0)
                                    {
                                        stack.Push(sub);
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                    catch { }

                    // Delete empty subdirectories safely
                    try
                    {
                        foreach (var subDir in di.GetDirectories())
                        {
                            try
                            {
                                if ((subDir.Attributes & FileAttributes.ReparsePoint) == 0)
                                {
                                    subDir.Delete(true);
                                }
                            }
                            catch { }
                        }
                    }
                    catch { }
                }

                category.Bytes = 0;
                return cleaned;
            });
        }

        private static long CalculateDirectorySize(DirectoryInfo dir)
        {
            long size = 0;
            try
            {
                var stack = new Stack<DirectoryInfo>();
                stack.Push(dir);

                while (stack.Count > 0)
                {
                    var current = stack.Pop();

                    try
                    {
                        foreach (var file in current.GetFiles())
                        {
                            try { size += file.Length; } catch { }
                        }
                    }
                    catch { }

                    try
                    {
                        foreach (var sub in current.GetDirectories())
                        {
                            if ((sub.Attributes & FileAttributes.ReparsePoint) == 0)
                            {
                                stack.Push(sub);
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return size;
        }

        private static long GetRecycleBinSize()
        {
            try
            {
                long size = 0;
                foreach (var drive in DriveInfo.GetDrives())
                {
                    if (drive.IsReady && drive.DriveType == DriveType.Fixed)
                    {
                        string recyclePath = Path.Combine(drive.RootDirectory.FullName, "$Recycle.Bin");
                        if (Directory.Exists(recyclePath))
                        {
                            size += CalculateDirectorySize(new DirectoryInfo(recyclePath));
                        }
                    }
                }
                return size;
            }
            catch
            {
                return 0;
            }
        }
    }
}
