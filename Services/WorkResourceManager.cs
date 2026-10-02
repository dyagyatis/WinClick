using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace WinClickWpf.Services
{
    public static class WorkResourceManager
    {
        private static string? _cachedWorkDir;
        private static readonly object _lock = new();

        public static string WorkDir
        {
            get
            {
                if (_cachedWorkDir != null) return _cachedWorkDir;
                lock (_lock)
                {
                    if (_cachedWorkDir != null) return _cachedWorkDir;
                    _cachedWorkDir = EnsureWorkDirectory();
                    return _cachedWorkDir;
                }
            }
        }

        private static string EnsureWorkDirectory()
        {
            // 1. If local Work directory exists alongside exe and has NSudoLG.exe, use it
            string localWork = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Work");
            if (Directory.Exists(localWork) && File.Exists(Path.Combine(localWork, "NSudoLG.exe")))
            {
                return localWork;
            }

            // 2. Otherwise extract embedded Work.zip into %TEMP%\WinClick_Work
            string tempWork = Path.Combine(Path.GetTempPath(), "WinClick_Work");
            try
            {
                if (!Directory.Exists(tempWork))
                {
                    Directory.CreateDirectory(tempWork);
                }

                // Check if already extracted
                if (!File.Exists(Path.Combine(tempWork, "NSudoLG.exe")) || !File.Exists(Path.Combine(tempWork, "setup.exe")))
                {
                    ExtractEmbeddedZip(tempWork);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error ensuring Work directory: {ex.Message}");
            }

            return tempWork;
        }

        private static void ExtractEmbeddedZip(string targetDir)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = $"{assembly.GetName().Name}.Work.zip";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream != null)
            {
                using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
                foreach (var entry in archive.Entries)
                {
                    string fullPath = Path.Combine(targetDir, entry.FullName);
                    if (string.IsNullOrEmpty(entry.Name))
                    {
                        // Directory entry
                        Directory.CreateDirectory(fullPath);
                    }
                    else
                    {
                        string? parent = Path.GetDirectoryName(fullPath);
                        if (!string.IsNullOrEmpty(parent) && !Directory.Exists(parent))
                        {
                            Directory.CreateDirectory(parent);
                        }

                        entry.ExtractToFile(fullPath, overwrite: true);
                    }
                }
            }
        }
    }
}
