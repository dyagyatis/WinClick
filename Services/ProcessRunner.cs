using System.Diagnostics;
using System.IO;

namespace WinClickWpf.Services
{
    public static class ProcessRunner
    {
        public static string WorkDir => WorkResourceManager.WorkDir;
        public static string NSudoPath => Path.Combine(WorkDir, "NSudoLG.exe");

        public static int Run(string fileName, string arguments, bool wait = true, bool hidden = true)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    WorkingDirectory = WorkDir,
                    UseShellExecute = false,
                    CreateNoWindow = hidden,
                    WindowStyle = hidden ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal
                };

                using var process = Process.Start(psi);
                if (process == null) return -1;
                if (wait)
                {
                    process.WaitForExit();
                    return process.ExitCode;
                }
                return 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ProcessRunner error ({fileName} {arguments}): {ex.Message}");
                return -1;
            }
        }

        public static int RunCmd(string command, bool wait = true)
        {
            return Run("cmd.exe", $"/c {command}", wait, true);
        }

        public static int RunTrustedInstaller(string command, bool wait = true)
        {
            if (File.Exists(NSudoPath))
            {
                string args = $"-U:T -P:E -ShowWindowMode:Hide {(wait ? "-Wait" : "")} cmd.exe /c {command}";
                return Run(NSudoPath, args, wait, true);
            }
            else
            {
                return RunCmd(command, wait);
            }
        }

        public static int RunPowerShell(string script, bool wait = true)
        {
            string escaped = script.Replace("\"", "\\\"");
            return Run("powershell.exe", $"-NoProfile -ExecutionPolicy Bypass -Command \"{escaped}\"", wait, true);
        }

        public static int RunPowerShellEncoded(string base64Script, bool wait = true)
        {
            return Run("powershell.exe", $"-NoProfile -ExecutionPolicy Bypass -EncodedCommand {base64Script}", wait, true);
        }
    }
}
