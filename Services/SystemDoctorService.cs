using System.Diagnostics;

namespace WinClickWpf.Services
{
    public static class SystemDoctorService
    {
        public static async Task RunFullSystemCheckAsync(Action<string> log)
        {
            await Task.Run(() =>
            {
                log("=== [1/2] Запуск проверки и восстановления файлов (SFC /scannow) ===");
                RunProcessStream("sfc.exe", "/scannow", log);

                log("\n=== [2/2] Запуск восстановления хранилища компонентов (DISM RestoreHealth) ===");
                RunProcessStream("dism.exe", "/online /cleanup-image /restorehealth", log);

                log("\n=== Проверка целостности системы полностью завершена! ===");
            });
        }

        public static async Task ResetNetworkStackAsync(Action<string> log)
        {
            await Task.Run(() =>
            {
                log("=== Сброс сетевого стека и параметров TCP/IP ===");

                log("1. Очистка кэша DNS...");
                RunProcessStream("ipconfig.exe", "/flushdns", log);

                log("2. Сброс каталога Winsock...");
                RunProcessStream("netsh.exe", "winsock reset", log);

                log("3. Сброс стека TCP/IP...");
                RunProcessStream("netsh.exe", "int ip reset", log);

                log("4. Сброс конфигурации адаптеров...");
                RunProcessStream("ipconfig.exe", "/release", log);
                RunProcessStream("ipconfig.exe", "/renew", log);

                log("\n=== Сетевой стек успешно сброшен и восстановлен! Рекомендуется перезагрузить ПК. ===");
            });
        }

        public static async Task RepairWindowsStoreAsync(Action<string> log)
        {
            await Task.Run(() =>
            {
                log("=== Восстановление и сброс Microsoft Store ===");

                log("1. Сброс кэша Магазина Windows (WSReset)...");
                ProcessRunner.Run("wsreset.exe", "");

                log("2. Перерегистрация пакетов Microsoft Store...");
                string psCmd = "Get-AppXPackage *WindowsStore* -AllUsers | Foreach {Add-AppxPackage -DisableDevelopmentMode -Register \"$($_.InstallLocation)\\AppXManifest.xml\" -ErrorAction SilentlyContinue}";
                ProcessRunner.RunPowerShell(psCmd);

                log("3. Перезапуск служб установки приложений...");
                ProcessRunner.RunCmd("sc start AppXSvc");
                ProcessRunner.RunCmd("sc start InstallService");

                log("\n=== Microsoft Store успешно перерегистрирован и восстановлен! ===");
            });
        }

        private static void RunProcessStream(string fileName, string args, Action<string> log)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
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
            }
            catch (Exception ex)
            {
                log($"Ошибка запуска {fileName}: {ex.Message}");
            }
        }
    }
}
