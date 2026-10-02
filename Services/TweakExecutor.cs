using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace WinClickWpf.Services
{
    public static class TweakExecutor
    {
        public static string WorkDir => WorkResourceManager.WorkDir;
        public static string SystemRoot => Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        public static string SystemDrive => Path.GetPathRoot(SystemRoot) ?? "C:\\";
        public static string ProgramFiles => Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        public static string ProgramFilesX86 => Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        public static string LocalAppData => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string AppData => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string UserProfile => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        public static void ExecuteTweak(string tag, Action<string>? statusCallback = null)
        {
            try
            {
                switch (tag)
                {
                    case "/CompactOSFast":
                        statusCallback?.Invoke("Сжатие ядра системы CompactOS (освобождение 4-7 ГБ)...");
                        CompactOSFast();
                        break;
                    case "/ClearStandbyList":
                        statusCallback?.Invoke("Очистка оперативной памяти и системного кэша...");
                        ClearStandbyList();
                        break;
                    case "/RemoveUpdateFiles":
                        statusCallback?.Invoke("Удаление файлов обновлений...");
                        RemoveUpdateFiles();
                        break;
                    case "/RemoveStoreCache":
                        statusCallback?.Invoke("Удаление кэша Windows Store...");
                        RemoveStoreCache();
                        break;
                    case "/RemoveExplorerCache":
                        statusCallback?.Invoke("Удаление кэша Проводника...");
                        RemoveExplorerCache();
                        break;
                    case "/CleanWinSxS":
                        statusCallback?.Invoke("Очистка WinSxS...");
                        CleanWinSxS();
                        break;
                    case "/RemoveJunkFolders":
                        statusCallback?.Invoke("Удаление лишних папок на диске C:...");
                        RemoveJunkFolders();
                        break;
                    case "/RemoveOldDrivers":
                        statusCallback?.Invoke("Удаление старых драйверов...");
                        RemoveOldDrivers();
                        break;
                    case "/RemoveShellBags":
                        statusCallback?.Invoke("Удаление ShellBags...");
                        RemoveShellBags();
                        break;

                    case "/RemoveAppx":
                        statusCallback?.Invoke("Удаление всех UWP-приложений...");
                        RemoveAppx();
                        break;
                    case "/RemoveOneDrive":
                        statusCallback?.Invoke("Удаление OneDrive...");
                        RemoveOneDrive();
                        break;
                    case "/CleanStartMenu":
                        statusCallback?.Invoke("Очистка меню Пуск...");
                        CleanStartMenu();
                        break;
                    case "/RemoveRemoteAssistant":
                        statusCallback?.Invoke("Удаление Помощника по удаленному подключению...");
                        RemoveRemoteAssistant();
                        break;

                    case "/RemoveEdge":
                        statusCallback?.Invoke("Удаление Microsoft Edge...");
                        RemoveEdge();
                        break;
                    case "/RemoveEdgeWebView":
                        statusCallback?.Invoke("Удаление Edge WebView2...");
                        RemoveEdgeWebView();
                        break;

                    case "/RemoveDefender":
                        statusCallback?.Invoke("Удаление Защитника Windows...");
                        RemoveDefender();
                        break;

                    case "/RemoveComponents":
                        statusCallback?.Invoke("Удаление дополнительных компонентов...");
                        RemoveComponents();
                        break;

                    case "/DisableTasks":
                        statusCallback?.Invoke("Отключение задач телеметрии...");
                        DisableTasks();
                        break;

                    case "/DisableHibernate":
                        statusCallback?.Invoke("Отключение гибернации...");
                        DisableHibernate();
                        break;
                    case "/DisableReservedStorage":
                        statusCallback?.Invoke("Отключение зарезервированного хранилища...");
                        DisableReservedStorage();
                        break;
                    case "/DisableRestorePoints":
                        statusCallback?.Invoke("Отключение точек восстановления...");
                        DisableRestorePoints();
                        break;
                    case "/DelayedServices":
                        statusCallback?.Invoke("Отложенный запуск автоматических служб...");
                        DelayedServices();
                        break;
                    case "/SystemLog":
                        statusCallback?.Invoke("Минимизация системных отчетов...");
                        SystemLog();
                        break;
                    case "/BoostIconCache":
                        statusCallback?.Invoke("Увеличение кэша иконок...");
                        BoostIconCache();
                        break;
                    case "/SvcSplit":
                        statusCallback?.Invoke("Увеличение порога разделения SVC...");
                        SvcSplit();
                        break;
                    case "/FastFolders":
                        statusCallback?.Invoke("Ускорение открытия папок...");
                        FastFolders();
                        break;
                    case "/DisableVBS":
                        statusCallback?.Invoke("Отключение VBS и HVCI...");
                        DisableVBS();
                        break;
                    case "/DisableGameDVR":
                        statusCallback?.Invoke("Отключение GameDVR...");
                        DisableGameDVR();
                        break;
                    case "/UltimatePerformance":
                        statusCallback?.Invoke("Схема питания Максимальная производительность...");
                        UltimatePerformance();
                        break;
                    case "/DisableResume":
                        statusCallback?.Invoke("Отключение функции Возобновить...");
                        DisableResume();
                        break;

                    case "/DisableWUDrivers":
                        statusCallback?.Invoke("Запрет установки драйверов из ЦО...");
                        DisableWUDrivers();
                        break;
                    case "/DisableDefenderUpdates":
                        statusCallback?.Invoke("Запрет обновлений средств удаления вредоносных программ...");
                        DisableDefenderUpdates();
                        break;
                    case "/PauseUpdates":
                        statusCallback?.Invoke("Пауза обновлений до 2077 года...");
                        PauseUpdates();
                        break;
                    case "/DisableAutoUpdates":
                        statusCallback?.Invoke("Запрет автоматических обновлений...");
                        DisableAutoUpdates();
                        break;

                    case "/DisableUAC":
                        statusCallback?.Invoke("Отключение UAC...");
                        DisableUAC();
                        break;
                    case "/EnableAdmin":
                        statusCallback?.Invoke("Назначение учетной записи Административной...");
                        EnableAdmin();
                        break;
                    case "/UnlockRegion":
                        statusCallback?.Invoke("Снятие региональных ограничений...");
                        UnlockRegion();
                        break;
                    case "/KillFreezeApps":
                        statusCallback?.Invoke("Принудительное завершение зависших программ...");
                        KillFreezeApps();
                        break;
                    case "/DisableRemote":
                        statusCallback?.Invoke("Отключение Удаленного помощника...");
                        DisableRemote();
                        break;
                    case "/DisableStickyKeys":
                        statusCallback?.Invoke("Отключение залипания клавиш...");
                        DisableStickyKeys();
                        break;
                    case "/TTL":
                        statusCallback?.Invoke("Скрытие реального TTL...");
                        TTL();
                        break;
                    case "/DisableNotificationsAds":
                        statusCallback?.Invoke("Отключение уведомлений и рекомендаций...");
                        DisableNotificationsAds();
                        break;
                    case "/DNS":
                        statusCallback?.Invoke("Установка DNS...");
                        DNS();
                        break;
                    case "/TakeOwnership":
                        statusCallback?.Invoke("Добавление пункта Стать владельцем в контекстное меню...");
                        TakeOwnership();
                        break;
                    case "/OpenAdminTerminal":
                        statusCallback?.Invoke("Добавление пункта Открыть Терминал в контекстное меню...");
                        OpenAdminTerminal();
                        break;
                    case "/CopyPath":
                        statusCallback?.Invoke("Добавление пункта Копировать путь в контекстное меню...");
                        CopyPath();
                        break;
                    case "/ClassicContextMenu":
                        statusCallback?.Invoke("Включение классического контекстного меню...");
                        ClassicContextMenu();
                        break;

                    case "/InstallDrivers":
                        statusCallback?.Invoke("Установка драйверов...");
                        InstallDrivers();
                        break;

                    case "/InstallVC":
                        statusCallback?.Invoke("Установка Visual C++...");
                        InstallVC();
                        break;
                    case "/InstallDX":
                        statusCallback?.Invoke("Установка DirectX 9-11...");
                        InstallDX();
                        break;

                    case "/RemoveHome":
                        statusCallback?.Invoke("Удаление Главная из Проводника...");
                        RemoveHome();
                        break;
                    case "/RemoveGallery":
                        statusCallback?.Invoke("Удаление Галерея из Проводника...");
                        RemoveGallery();
                        break;
                    case "/RemoveNetwork":
                        statusCallback?.Invoke("Удаление Сеть из Проводника...");
                        RemoveNetwork();
                        break;
                    case "/DarkTheme":
                        statusCallback?.Invoke("Установка темной темы...");
                        DarkTheme();
                        break;
                    case "/SetWallpaper":
                        statusCallback?.Invoke("Установка обоев...");
                        SetWallpaper();
                        break;
                    case "/BlueIcons":
                        statusCallback?.Invoke("Установка синих папок...");
                        BlueIcons();
                        break;
                    case "/Icaros":
                        statusCallback?.Invoke("Установка эскизов Icaros...");
                        Icaros();
                        break;
                    case "/TraySeconds":
                        statusCallback?.Invoke("Установка секунд в трее...");
                        TraySeconds();
                        break;
                    case "/TrayDate":
                        statusCallback?.Invoke("Установка даты в трее...");
                        TrayDate();
                        break;
                    case "/TaskbarEndTask":
                        statusCallback?.Invoke("Включение Завершить задачу...");
                        TaskbarEndTask();
                        break;
                    case "/RemoveTaskbarIcons":
                        statusCallback?.Invoke("Удаление лишних значков панели задач...");
                        RemoveTaskbarIcons();
                        break;
                    case "/HideRecommended":
                        statusCallback?.Invoke("Скрытие раздела Рекомендуем в Пуске...");
                        HideRecommended();
                        break;
                    case "/StartSettingsIcon":
                        statusCallback?.Invoke("Установка значка Настройки в Пуске...");
                        StartSettingsIcon();
                        break;
                    case "/WallpaperQuality":
                        statusCallback?.Invoke("Удаление сжатия обоев...");
                        WallpaperQuality();
                        break;
                    case "/RemoveLockScreen":
                        statusCallback?.Invoke("Удаление экрана блокировки...");
                        RemoveLockScreen();
                        break;
                    case "/NoIconShadow":
                        statusCallback?.Invoke("Удаление тени на значках...");
                        NoIconShadow();
                        break;
                    case "/ExplorerThisPC":
                        statusCallback?.Invoke("Открывать Проводник в Этот компьютер...");
                        ExplorerThisPC();
                        break;
                    case "/ShowExtensions":
                        statusCallback?.Invoke("Показывать расширения файлов...");
                        ShowExtensions();
                        break;

                    case "/CompressOS":
                        statusCallback?.Invoke("Сжатие системных папок LZX (Program Files, ProgramData, Users, Windows)...");
                        CompressOS(statusCallback);
                        break;
                    case "/CompressDrive":
                        statusCallback?.Invoke("Полное сжатие системного диска LZX...");
                        CompressDrive(statusCallback);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error executing tweak {tag}: {ex.Message}");
            }
        }

        #region Tweaks Implementation

        [DllImport("psapi.dll")]
        private static extern int EmptyWorkingSet(IntPtr hwProc);

        public static void CompactOSFast()
        {
            ProcessRunner.Run("compact.exe", "/CompactOS:always");
        }

        public static void ClearStandbyList()
        {
            try
            {
                foreach (var proc in Process.GetProcesses())
                {
                    try
                    {
                        using (proc)
                        {
                            EmptyWorkingSet(proc.Handle);
                        }
                    }
                    catch { }
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch { }
        }

        public static void RemoveUpdateFiles()
        {
            SafeDeleteDirectory(Path.Combine(SystemRoot, "SoftwareDistribution", "Download"));
            SafeDeleteDirectory(Path.Combine(ProgramFilesX86, "Microsoft", "EdgeUpdate", "Download"));
        }

        public static void RemoveStoreCache()
        {
            string storeCache = Path.Combine(UserProfile, "AppData", "Local", "Packages", "Microsoft.WindowsStore_8wekyb3d8bbwe", "LocalCache");
            SafeDeleteDirectory(storeCache);
        }

        public static void RemoveExplorerCache()
        {
            string explorerPath = Path.Combine(LocalAppData, "Microsoft", "Windows", "Explorer");
            if (Directory.Exists(explorerPath))
            {
                try
                {
                    foreach (var file in Directory.GetFiles(explorerPath, "IconCache*")) File.Delete(file);
                    foreach (var file in Directory.GetFiles(explorerPath, "thumbcache*")) File.Delete(file);
                }
                catch { }
            }

            try
            {
                string iconDb = Path.Combine(LocalAppData, "IconCache.db");
                if (File.Exists(iconDb)) File.Delete(iconDb);
                string iconDbWal = Path.Combine(LocalAppData, "IconCache.db-wal");
                if (File.Exists(iconDbWal)) File.Delete(iconDbWal);
            }
            catch { }
        }

        public static void CleanWinSxS()
        {
            ProcessRunner.Run("dism.exe", "/online /Cleanup-Image /StartComponentCleanup /ResetBase");
        }

        public static void RemoveJunkFolders()
        {
            string[] junk = { Path.Combine(SystemDrive, "Windows.old"), Path.Combine(SystemDrive, "PerfLogs"), Path.Combine(SystemDrive, "inetpub") };
            foreach (var f in junk)
            {
                ProcessRunner.RunTrustedInstaller($"rd /q /s \"{f}\"");
            }
        }

        public static void RemoveOldDrivers()
        {
            string psScript = @"$dismOut = dism /online /get-drivers
$lines = $dismOut | select -Skip 10
$Operation = 'theName'
$Drivers = @()
foreach ($Line in $lines) {
    $tmp = $Line
    $txt = $($tmp.Split(':'))[1]
    switch ($Operation) {
        'theName' { $Name = $txt; $Operation = 'theFileName'; break }
        'theFileName' { $FileName = $txt.Trim(); $Operation = 'theEntr'; break }
        'theEntr' { $Entr = $txt.Trim(); $Operation = 'theClassName'; break }
        'theClassName' { $ClassName = $txt.Trim(); $Operation = 'theVendor'; break }
        'theVendor' { $Vendor = $txt.Trim(); $Operation = 'theDate'; break }
        'theDate' { $tmp = $txt.split('.'); $txt = ""$($tmp[2]).$($tmp[1]).$($tmp[0].Trim())""; $Date = $txt; $Operation = 'theVersion'; break }
        'theVersion' { $Version = $txt.Trim(); $Operation = 'theNull'; $params = [ordered]@{ 'FileName' = $FileName; 'Vendor' = $Vendor; 'Date' = $Date; 'Name' = $Name; 'ClassName' = $ClassName; 'Version' = $Version; 'Entr' = $Entr }; $obj = New-Object -TypeName PSObject -Property $params; $Drivers += $obj; break }
        'theNull' { $Operation = 'theName'; break }
    }
}
$last = ''
$NotUnique = @()
foreach ($Dr in ($Drivers | sort Filename)) {
    if ($Dr.FileName -eq $last) { $NotUnique += $Dr }
    $last = $Dr.FileName
}
$list = $NotUnique | select -ExpandProperty FileName -Unique
$ToDel = @()
foreach ($Dr in $list) {
    $sel = $Drivers | where { $_.FileName -eq $Dr } | sort date -Descending | select -Skip 1
    $ToDel += $sel
}
foreach ($item in $ToDel) {
    $Name = $($item.Name).Trim()
    Invoke-Expression -Command ""pnputil.exe /delete-driver $Name""
}";
            ProcessRunner.RunPowerShell(psScript);
        }

        public static void RemoveShellBags()
        {
            string[] keys = { "Bags", "BagMRU", "BagsMRU" };
            foreach (var k in keys)
            {
                RegistryHelper.DeleteSubKeyTree(RegistryHive.CurrentUser, $@"Software\Microsoft\Windows\Shell\{k}");
                RegistryHelper.DeleteSubKeyTree(RegistryHive.CurrentUser, $@"Software\Microsoft\Windows\ShellNoRoam\{k}");
                RegistryHelper.DeleteSubKeyTree(RegistryHive.CurrentUser, $@"Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\{k}");
            }
        }

        public static void RemoveAppx()
        {
            string cmd = "Get-AppxPackage | Where-Object { $_.NonRemovable -eq $false } | ForEach-Object { Remove-AppxPackage -Package $_.PackageFullName -AllUsers -ErrorAction SilentlyContinue }";
            ProcessRunner.RunPowerShell(cmd);
        }

        public static void RemoveOneDrive()
        {
            ProcessRunner.RunCmd("taskkill /f /im OneDrive.exe");
            string setupPath = Path.Combine(SystemRoot, "System32", "OneDriveSetup.exe");
            if (File.Exists(setupPath))
            {
                ProcessRunner.Run(setupPath, "/uninstall");
            }

            string[] dirs = {
                Path.Combine(LocalAppData, "OneDrive"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft OneDrive"),
                Path.Combine(UserProfile, "OneDrive"),
                Path.Combine(LocalAppData, "Microsoft", "OneDrive")
            };
            foreach (var d in dirs) SafeDeleteDirectory(d);

            string winSxS = Path.Combine(SystemRoot, "WinSxS");
            if (Directory.Exists(winSxS))
            {
                foreach (var dir in Directory.GetDirectories(winSxS, "amd64_microsoft-windows-onedrive-setup*"))
                {
                    ProcessRunner.RunTrustedInstaller($"rd /s /q \"{dir}\"");
                }
            }

            ProcessRunner.RunTrustedInstaller($"del /q \"{Path.Combine(SystemRoot, "System32", "OneDriveSetup.exe")}\"");
            ProcessRunner.RunTrustedInstaller($"del /q \"{Path.Combine(SystemRoot, "System32", "OneDrive.ico")}\"");

            RegistryHelper.DeleteSubKeyTree(RegistryHive.CurrentUser, @"Software\Microsoft\OneDrive");
            RegistryHelper.DeleteSubKeyTree(RegistryHive.LocalMachine, @"Software\Microsoft\OneDrive");
        }

        public static void CleanStartMenu()
        {
            SafeDeleteDirectory(Path.Combine(AppData, "Microsoft", "Windows", "Start Menu", "Programs", "Accessibility"));
            SafeDeleteDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft", "Windows", "Start Menu", "Programs", "Accessories", "System Tools"));
        }

        public static void RemoveRemoteAssistant()
        {
            ProcessRunner.RunPowerShell("Start-Process mstsc.exe -ArgumentList '/uninstall' -WindowStyle Hidden -ErrorAction SilentlyContinue");
            Thread.Sleep(3000);
            ProcessRunner.RunCmd("taskkill /f /im mstsc.exe");
        }

        public static void RemoveEdge()
        {
            ProcessRunner.RunTrustedInstaller("taskkill /f /im MicrosoftEdge.exe");
            ProcessRunner.RunTrustedInstaller("taskkill /f /im MicrosoftEdgeUpdate.exe");
            ProcessRunner.RunTrustedInstaller("taskkill /f /im MicrosoftEdgeWebView.exe");

            string setup = Path.Combine(WorkDir, "setup.exe");
            if (File.Exists(setup))
            {
                ProcessRunner.Run(setup, "--uninstall --system-level --force-uninstall --msedge");
            }

            ProcessRunner.RunTrustedInstaller($"rd /s /q \"{Path.Combine(ProgramFilesX86, "Microsoft", "Edge")}\"");
            ProcessRunner.RunTrustedInstaller($"rd /s /q \"{Path.Combine(ProgramFilesX86, "Microsoft", "EdgeCore")}\"");
            ProcessRunner.RunTrustedInstaller($"rd /s /q \"{Path.Combine(ProgramFilesX86, "Microsoft", "Temp")}\"");
        }

        public static void RemoveEdgeWebView()
        {
            ProcessRunner.RunTrustedInstaller("taskkill /f /im MicrosoftEdge.exe");
            ProcessRunner.RunTrustedInstaller("taskkill /f /im MicrosoftEdgeUpdate.exe");
            ProcessRunner.RunTrustedInstaller("taskkill /f /im MicrosoftEdgeWebView.exe");

            string setup = Path.Combine(WorkDir, "setup.exe");
            if (File.Exists(setup))
            {
                ProcessRunner.Run(setup, "--uninstall --system-level --force-uninstall --msedgewebview");
            }

            ProcessRunner.RunTrustedInstaller($"rd /s /q \"{Path.Combine(ProgramFilesX86, "Microsoft", "EdgeWebView")}\"");
            ProcessRunner.RunTrustedInstaller($"rd /s /q \"{Path.Combine(ProgramFilesX86, "Microsoft", "EdgeCore")}\"");
        }

        public static void RemoveDefender()
        {
            string dkBat = Path.Combine(WorkDir, "DK", "DefenderKiller.bat");
            if (File.Exists(dkBat))
            {
                ProcessRunner.Run("cmd.exe", $"/c \"{dkBat}\" /DelWD", true, false);
            }
            else
            {
                // Standalone Defender disable tweaks
                RegistryHelper.SetDword(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender", "DisableAntiSpyware", 1);
                RegistryHelper.SetDword(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender", "DisableRealtimeMonitoring", 1);
            }
        }

        public static void RemoveComponents()
        {
            string[] caps = {
                "Microsoft.Windows.Notepad.System",
                "Microsoft.Windows.PowerShell.ISE",
                "Print.Management.Console",
                "VBSCRIPT",
                "OpenSSH.Client",
                "Hello.Face",
                "MathRecognizer",
                "InternetExplorer",
                "StepsRecorder",
                "Media.WindowsMediaPlayer",
                "Microsoft.Wallpapers.Extended"
            };

            foreach (var cap in caps)
            {
                ProcessRunner.RunCmd($"dism /Online /Remove-Capability /CapabilityName:{cap}~~~~0.0.1.0 /NoRestart");
            }
        }

        public static void DisableTasks()
        {
            string[] tasks = {
                @"\Microsoft\Windows\Active Directory Rights Management Services Client\AD RMS Rights Policy Template Management (Automated)",
                @"\Microsoft\Windows\AppID\EDP Policy Manager",
                @"\Microsoft\Windows\AppID\PolicyConverter",
                @"\Microsoft\Windows\Application Experience\MareBackup",
                @"\Microsoft\Windows\Application Experience\Microsoft Compatibility Appraiser",
                @"\Microsoft\Windows\Application Experience\Microsoft Compatibility Appraiser Exp",
                @"\Microsoft\Windows\Application Experience\PcaPatchDbTask",
                @"\Microsoft\Windows\Application Experience\SdbinstMergeDbTask",
                @"\Microsoft\Windows\Application Experience\StartupAppTask",
                @"\Microsoft\Windows\Application Experience\ProgramDataUpdater",
                @"\Microsoft\Windows\Application Experience\ProgramInventoryUpdater",
                @"\Microsoft\Windows\ApplicationData\appuriverifierdaily",
                @"\Microsoft\Windows\ApplicationData\appuriverifierinstall",
                @"\Microsoft\Windows\ApplicationData\DsSvcCleanup",
                @"\Microsoft\Windows\AppxDeploymentClient\Pre-staged app cleanup",
                @"\Microsoft\Windows\AppxDeploymentClient\UCPD velocity",
                @"\Microsoft\Windows\Autochk\Proxy",
                @"\Microsoft\Windows\AutoLogger\AutoLogger-Diagtrack-Listener",
                @"\Microsoft\Windows\AutoLogger\AutoLogger-FileSizeTracking",
                @"\Microsoft\Windows\BrokerInfrastructure\BgTaskRegistrationMaintenanceTask",
                @"\Microsoft\Windows\CEIP\Uploader",
                @"\Microsoft\Windows\CertificateServicesClient\AikCertEnrollTask",
                @"\Microsoft\Windows\CertificateServicesClient\CryptoPolicyTask",
                @"\Microsoft\Windows\CertificateServicesClient\KeyPreGenTask",
                @"\Microsoft\Windows\CertificateServicesClient\SystemTask",
                @"\Microsoft\Windows\Cleanup\UpdateCleanup",
                @"\Microsoft\Windows\Clip\License Validation",
                @"\Microsoft\Windows\Clip\LicenseImdsIntegration",
                @"\Microsoft\Windows\CloudExperienceHost\CreateObjectTask",
                @"\Microsoft\Windows\CloudExperienceHost\SyncHost",
                @"\Microsoft\Windows\CloudRestore\Backup",
                @"\Microsoft\Windows\CloudRestore\Restore",
                @"\Microsoft\Windows\ContactSupport\Scheduled",
                @"\Microsoft\Windows\Customer Experience Improvement Program\Consolidator",
                @"\Microsoft\Windows\Customer Experience Improvement Program\KernelCeipTask",
                @"\Microsoft\Windows\Customer Experience Improvement Program\UsbCeip",
                @"\Microsoft\Windows\Customer Experience Improvement Program\BthSQM",
                @"\Microsoft\Windows\Customer Experience Improvement Program\Uploader",
                @"\Microsoft\Windows\Device Information\Device",
                @"\Microsoft\Windows\Device Information\Device User",
                @"\Microsoft\Windows\Device Setup\Driver Recovery on Reboot",
                @"\Microsoft\Windows\Device Setup\Metadata Refresh",
                @"\Microsoft\Windows\DeviceDirectoryClient\HandleCommand",
                @"\Microsoft\Windows\DeviceDirectoryClient\HandleWnsCommand",
                @"\Microsoft\Windows\DeviceDirectoryClient\IntegrityCheck",
                @"\Microsoft\Windows\DeviceDirectoryClient\LocateCommandUserSession",
                @"\Microsoft\Windows\DeviceDirectoryClient\RegisterDeviceAccountChange",
                @"\Microsoft\Windows\DeviceDirectoryClient\RegisterDeviceLocationRightsChange",
                @"\Microsoft\Windows\DeviceDirectoryClient\RegisterDevicePeriodic24",
                @"\Microsoft\Windows\DeviceDirectoryClient\RegisterDevicePolicyChange",
                @"\Microsoft\Windows\DeviceDirectoryClient\RegisterDeviceProtectionStateChanged",
                @"\Microsoft\Windows\DeviceDirectoryClient\RegisterDeviceSettingChange",
                @"\Microsoft\Windows\DeviceDirectoryClient\RegisterUserDevice",
                @"\Microsoft\Windows\Diagnosis\RecommendedTroubleshootingScanner",
                @"\Microsoft\Windows\Diagnosis\Scheduled",
                @"\Microsoft\Windows\Diagnosis\UnexpectedCodepath",
                @"\Microsoft\Windows\DiskCleanup\SilentCleanup",
                @"\Microsoft\Windows\DiskDiagnostic\Microsoft-Windows-DiskDiagnosticDataCollector",
                @"\Microsoft\Windows\DiskDiagnostic\Microsoft-Windows-DiskDiagnosticResolver",
                @"\Microsoft\Windows\DiskDiagnostic\DiagnosticResolver",
                @"\Microsoft\Windows\DiskDiagnostic\DiskDiagnostic",
                @"\Microsoft\Windows\DiskFootprint\Diagnostics",
                @"\Microsoft\Windows\DiskFootprint\StorageSense",
                @"\Microsoft\Windows\DUSM\dusmtask",
                @"\Microsoft\Windows\ErrorReporting\QueueReporting",
                @"\Microsoft\Windows\ErrorReporting\KernelCeipTask",
                @"\Microsoft\Windows\ExploitGuard\ExploitGuard MDM policy Refresh",
                @"\Microsoft\Windows\Feedback\Siuf\DmClient",
                @"\Microsoft\Windows\Feedback\Siuf\DmClientOnScenarioDownload",
                @"\Microsoft\Windows\Feedback\Siuf\DmClientOnScenarioUpload",
                @"\Microsoft\Windows\Feedback\Siuf\DmClientOnScenarioRun",
                @"\Microsoft\Windows\Feedback\Siuf\DmClientOnUserSignIn",
                @"\Microsoft\Windows\File Classification Infrastructure\Property Definition Sync",
                @"\Microsoft\Windows\FileHistory\File History (maintenance mode)",
                @"\Microsoft\Windows\Help\OEMSupport",
                @"\Microsoft\Windows\Help\WindowsHelpUpdateTask",
                @"\Microsoft\Windows\HelloFace\FODCleanupTask",
                @"\Microsoft\Windows\HelloFace\FeatureCleanup",
                @"\Microsoft\Windows\input\InputSettingsRestoreDataAvailable",
                @"\Microsoft\Windows\input\LocalUserSyncDataAvailable",
                @"\Microsoft\Windows\input\MouseSyncDataAvailable",
                @"\Microsoft\Windows\input\PenSyncDataAvailable",
                @"\Microsoft\Windows\input\RemoteMouseSyncDataAvailable",
                @"\Microsoft\Windows\input\RemotePenSyncDataAvailable",
                @"\Microsoft\Windows\input\RemoteTouchpadSyncDataAvailable",
                @"\Microsoft\Windows\input\TouchpadSyncDataAvailable",
                @"\Microsoft\Windows\InstallService\WakeUpAndContinueUpdates",
                @"\Microsoft\Windows\InstallService\WakeUpAndScanForUpdates",
                @"\Microsoft\Windows\International\Synchronize Language Settings",
                @"\Microsoft\Windows\LanguageComponentsInstaller\Installation",
                @"\Microsoft\Windows\LanguageComponentsInstaller\ReconcileLanguageResources",
                @"\Microsoft\Windows\LanguageComponentsInstaller\Uninstallation",
                @"\Microsoft\Windows\License Manager\TempSignedLicenseExchange",
                @"\Microsoft\Windows\Location\WindowsActionDialog",
                @"\Microsoft\Windows\Maintenance\WinSAT",
                @"\Microsoft\Windows\Maps\MapsToastTask",
                @"\Microsoft\Windows\Maps\MapsUpdateTask",
                @"\Microsoft\Windows\MemoryDiagnostic\AutomaticOfflineMemoryDiagnostic",
                @"\Microsoft\Windows\MemoryDiagnostic\RunFullMemoryDiagnostic",
                @"\Microsoft\Windows\NlaSvc\WiFiTask",
                @"\Microsoft\Windows\Offline Files\Background Synchronization",
                @"\Microsoft\Windows\Offline Files\Logon Synchronization",
                @"\Microsoft\Windows\PCRPF\PCR Prediction Framework Firmware Update Task",
                @"\Microsoft\Windows\PerformanceTrace\WhesvcToast",
                @"\Microsoft\Windows\PI\Secure-Boot-Update",
                @"\Microsoft\Windows\PI\Sqm-Tasks",
                @"\Microsoft\Windows\Pluton\Pluton-Ksp-Provisioning",
                @"\Microsoft\Windows\Power Efficiency Diagnostics\AnalyzeSystem",
                @"\Microsoft\Windows\Printing\EduPrintProv",
                @"\Microsoft\Windows\Printing\PrintJobCleanupTask",
                @"\Microsoft\Windows\PushToInstall\LoginCheck",
                @"\Microsoft\Windows\PushToInstall\Registration",
                @"\Microsoft\Windows\Ras\MobilityManager",
                @"\Microsoft\Windows\ReFsDedupSvc\Initialization",
                @"\Microsoft\Windows\Registry\RegIdleBackup",
                @"\Microsoft\Windows\RemoteAssistance\RemoteAssistanceTask",
                @"\Microsoft\Windows\RetailDemo\CleanupOfflineContent",
                @"\Microsoft\Windows\Search\IndexerDiagnosticsTask",
                @"\Microsoft\Windows\Search\SearchIndexerMaintenance",
                @"\Microsoft\Windows\Setup\SetupCleanupTask",
                @"\Microsoft\Windows\SharedPC\Account Cleanup",
                @"\Microsoft\Windows\Shell\FamilySafetyMonitor",
                @"\Microsoft\Windows\Shell\FamilySafetyRefreshTask",
                @"\Microsoft\Windows\Shell\IndexerAutomaticMaintenance",
                @"\Microsoft\Windows\Shell\ThemeAssetTask_SyncFODState",
                @"\Microsoft\Windows\Shell\ThemesSyncedImageDownload",
                @"\Microsoft\Windows\Shell\UndockedFlightingUpdate",
                @"\Microsoft\Windows\Shell\UpdateUserPictureTask",
                @"\Microsoft\Windows\Storage Tiers Management\Storage Tiers Optimization",
                @"\Microsoft\Windows\Subscription\EnableLicenseAcquisition",
                @"\Microsoft\Windows\Subscription\LicenseAcquisition",
                @"\Microsoft\Windows\Sysmain\WsSwapAssessmentTask",
                @"\Microsoft\Windows\Sysmain\HybridDriveCacheRebalance",
                @"\Microsoft\Windows\Sysmain\HybridDriveCachePrepopulate",
                @"\Microsoft\Windows\UPnP\UPnPHostConfig",
                @"\Microsoft\Windows\UpdateOrchestrator\CleanupUpdateTask",
                @"\Microsoft\Windows\User Profile Service\HiveUploadTask",
                @"\Microsoft\Windows\WaaSMedic\PerformRemediation",
                @"\Microsoft\Windows\WaaSMedic\ScanForUpdates",
                @"\Microsoft\Windows\WaaSMedic\WsusScan",
                @"\Microsoft\Windows\Windows Error Reporting\QueueReporting",
                @"\Microsoft\Windows\Windows Error Reporting\ReportQueue",
                @"\Microsoft\Windows\Windows Filtering Platform\BfeOnServiceStartTypeChange",
                @"\Microsoft\Windows\WindowsAI\Recall\InitialConfiguration",
                @"\Microsoft\Windows\WindowsAI\Recall\PolicyConfiguration",
                @"\Microsoft\Windows\WindowsAI\Settings\InitialConfiguration",
                @"\Microsoft\Windows\WindowsAI\Copilot\CopilotDataCollectionTask",
                @"\Microsoft\Windows\WindowsAI\Insights\InsightsDataCollectionTask",
                @"\Microsoft\Windows\WindowsUpdate\Refresh Group Policy Cache",
                @"\Microsoft\Windows\WlanSvc\CDSSync",
                @"\Microsoft\Windows\WOF\WIM-Hash-Management",
                @"\Microsoft\Windows\WOF\WIM-Hash-Validation",
                @"\Microsoft\Windows\Workplace Join\Automatic-Device-Join",
                @"\Microsoft\Windows\Workplace Join\Device-Sync",
                @"\Microsoft\Windows\Workplace Join\Recovery-Check",
                @"\Microsoft\Windows\UNP\RunCampaignManager",
                @"MicrosoftEdgeUpdateTaskMachineCore",
                @"MicrosoftEdgeUpdateTaskMachineUA",
                @"\Microsoft\Windows\Windows Defender\Windows Defender Cache Maintenance",
                @"\Microsoft\Windows\Windows Defender Cleanup",
                @"\Microsoft\Windows\Windows Defender Scheduled Scan",
                @"\Microsoft\Windows\Windows Defender Verification"
            };

            foreach (var task in tasks)
            {
                ProcessRunner.RunTrustedInstaller($"schtasks /Change /TN \"{task}\" /Disable");
            }
        }

        public static void DisableHibernate()
        {
            ProcessRunner.Run("powercfg.exe", "-h off");
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"System\CurrentControlSet\Control\Power", "HibernateEnabledDefault", 0);
        }

        public static void DisableReservedStorage()
        {
            ProcessRunner.Run("dism.exe", "/Online /Set-ReservedStorageState /State:Disabled");
        }

        public static void DisableRestorePoints()
        {
            ProcessRunner.Run("vssadmin.exe", "resize shadowstorage /on=c: /for=c: /maxsize=1%");
            ProcessRunner.RunPowerShell("Disable-ComputerRestore -Drive 'C:\\'");
            ProcessRunner.Run("vssadmin.exe", "delete shadows /all /quiet");
            RegistryHelper.DeleteValue(RegistryHive.LocalMachine, @"Software\Microsoft\Windows NT\CurrentVersion\SystemRestore", "RPSessionInterval");
        }

        public static void DelayedServices()
        {
            string[] svcs = { "EventSystem", "NlaSvc" };
            foreach (var s in svcs)
            {
                RegistryHelper.SetDword(RegistryHive.LocalMachine, $@"System\CurrentControlSet\Services\{s}", "DelayedAutostart", 1);
            }
        }

        public static void SystemLog()
        {
            string eventLogExe = Path.Combine(WorkDir, "EventLog.exe");
            if (File.Exists(eventLogExe))
            {
                ProcessRunner.Run(eventLogExe, "");
            }
        }

        public static void BoostIconCache()
        {
            RegistryHelper.SetString(RegistryHive.LocalMachine, @"Software\Microsoft\Windows\CurrentVersion\Explorer", "MaxCachedIcons", "4096");
        }

        public static void SvcSplit()
        {
            string ps = @"$key = 'HKLM:\SYSTEM\CurrentControlSet\Control'; if (Get-ItemProperty -Path $key -Name 'SvcHostSplitThresholdInKB' -ErrorAction SilentlyContinue) { if (-not (Get-ItemProperty -Path $key -Name 'SvcHostSplitThresholdInKB_orig' -ErrorAction SilentlyContinue)) { Rename-ItemProperty -Path $key -Name 'SvcHostSplitThresholdInKB' -NewName 'SvcHostSplitThresholdInKB_orig'; $mem = (Get-CimInstance Win32_OperatingSystem).TotalVisibleMemorySize + 1024000; Set-ItemProperty -Path $key -Name 'SvcHostSplitThresholdInKB' -Value $mem -Type DWord } }";
            ProcessRunner.RunPowerShell(ps);
        }

        public static void FastFolders()
        {
            RegistryHelper.SetString(RegistryHive.CurrentUser, @"Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags\AllFolders\Shell", "FolderType", "NotSpecified");
            string[] dirs = { "Directory.Audio", "Directory.Image", "Directory.Video" };
            string[] commands = { "Enqueue", "Play" };
            foreach (var d in dirs)
            {
                foreach (var c in commands)
                {
                    RegistryHelper.SetString(RegistryHive.ClassesRoot, $@"SystemFileAssociations\{d}\shell\{c}", "LegacyDisable", "");
                }
            }
        }

        public static void DisableVBS()
        {
            ProcessRunner.Run("bcdedit.exe", "/set hypervisorlaunchtype off");
            string[] deletePolicy = { "HypervisorEnforcedCodeIntegrity", "LsaCfgFlags", "RequirePlatformSecurityFeatures", "ConfigureSystemGuardLaunch", "ConfigureKernelShadowStacksLaunch" };
            foreach (var p in deletePolicy) RegistryHelper.DeleteValue(RegistryHive.LocalMachine, @"Software\Policies\Microsoft\Windows\DeviceGuard", p);

            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"Software\Policies\Microsoft\Windows\DeviceGuard", "EnableVirtualizationBasedSecurity", 0);
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"Software\Policies\Microsoft\Windows\DeviceGuard", "HVCIMATRequired", 0);

            RegistryHelper.DeleteValue(RegistryHive.LocalMachine, @"System\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "WasEnabledBy");
            RegistryHelper.DeleteValue(RegistryHive.LocalMachine, @"System\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "WasEnabledBySysprep");

            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"System\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "Enabled", 0);
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"System\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "HVCIMATRequired", 0);
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"System\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "Locked", 0);

            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"System\CurrentControlSet\Control\DeviceGuard", "EnableVirtualizationBasedSecurity", 0);
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"System\CurrentControlSet\Control\DeviceGuard", "RequirePlatformSecurityFeatures", 0);
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"System\CurrentControlSet\Control\DeviceGuard", "Locked", 0);

            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"System\CurrentControlSet\Control\DeviceGuard\Scenarios\KernelShadowStacks", "Enabled", 0);
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"System\CurrentControlSet\Control\DeviceGuard\Scenarios\KernelShadowStacks", "AuditModeEnabled", 0);
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"System\CurrentControlSet\Control\DeviceGuard\Scenarios\KernelShadowStacks", "WasEnabledBy", 0);
        }

        public static void DisableGameDVR()
        {
            RegistryHelper.SetDword(RegistryHive.ClassesRoot, @"System\GameConfigStore", "GameDVR_Enabled", 0);
            RegistryHelper.SetDword(RegistryHive.ClassesRoot, @"Software\Microsoft\Windows\CurrentVersion\GameDVR", "AppCaptureEnabled", 0);
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"Software\Policies\Microsoft\Windows\GameDVR", "AllowGameDVR", 0);
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"Software\Microsoft\PolicyManager\default\ApplicationManagement\AllowGameDVR", "Value", 0);
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"Software\Microsoft\Windows\CurrentVersion\GameDVR", "AllowGameDVR", 0);
        }

        public static void UltimatePerformance()
        {
            ProcessRunner.RunTrustedInstaller("reg add \"HKLM\\System\\CurrentControlSet\\Control\\Power\\User\\PowerSchemes\" /v ActivePowerScheme /t REG_SZ /d e9a42b02-d5df-448d-aa00-03f14749eb61 /f");
        }

        public static void DisableResume()
        {
            string vivetool = Path.Combine(WorkDir, "ViVeTool.exe");
            if (File.Exists(vivetool))
            {
                ProcessRunner.Run(vivetool, "/disable /id:56517033");
            }
            RegistryHelper.SetDword(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\CrossDeviceResume\Configuration", "IsResumeAllowed", 0);
            RegistryHelper.SetDword(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\CrossDeviceResume\Configuration", "IsOneDriveResumeAllowed", 0);
        }

        public static void DisableWUDrivers()
        {
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"Software\Microsoft\Windows\CurrentVersion\Device Metadata", "PreventDeviceMetadataFromNetwork", 1);
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"Software\Policies\Microsoft\Windows\Device Metadata", "PreventDeviceMetadataFromNetwork", 1);
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"Software\Policies\Microsoft\Windows\DriverSearching", "SearchOrderConfig", 2);
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"Software\Policies\Microsoft\Windows\WindowsUpdate", "ExcludeWUDriversInQualityUpdate", 1);
        }

        public static void DisableDefenderUpdates()
        {
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"Software\Policies\Microsoft\MRT", "DontOfferThroughWUAU", 1);
        }

        public static void PauseUpdates()
        {
            string startTime = "2024-09-13T00:00:00Z";
            string expiryTime = "2077-07-07T00:00:00Z";
            string pauseDate = "2077-07-07 13:00:00";

            string uxKey = @"SOFTWARE\Microsoft\WindowsUpdate\UX\Settings";
            RegistryHelper.SetString(RegistryHive.LocalMachine, uxKey, "PauseUpdatesStartTime", startTime);
            RegistryHelper.SetString(RegistryHive.LocalMachine, uxKey, "PauseUpdatesExpiryTime", expiryTime);
            RegistryHelper.SetString(RegistryHive.LocalMachine, uxKey, "PauseFeatureUpdatesStartTime", startTime);
            RegistryHelper.SetString(RegistryHive.LocalMachine, uxKey, "PauseFeatureUpdatesExpiryTime", expiryTime);
            RegistryHelper.SetString(RegistryHive.LocalMachine, uxKey, "PauseQualityUpdatesStartTime", startTime);
            RegistryHelper.SetString(RegistryHive.LocalMachine, uxKey, "PauseQualityUpdatesExpiryTime", expiryTime);

            string polKey = @"Software\Microsoft\WindowsUpdate\UpdatePolicy\Settings";
            RegistryHelper.SetDword(RegistryHive.LocalMachine, polKey, "PausedFeatureStatus", 1);
            RegistryHelper.SetDword(RegistryHive.LocalMachine, polKey, "PausedQualityStatus", 1);
            RegistryHelper.SetString(RegistryHive.LocalMachine, polKey, "PausedQualityDate", pauseDate);
            RegistryHelper.SetString(RegistryHive.LocalMachine, polKey, "PausedFeatureDate", pauseDate);
        }

        public static void DisableAutoUpdates()
        {
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "NoAutoUpdate", 1);
        }

        public static void DisableUAC()
        {
            string[] sysVals = { "EnableLUA", "PromptOnSecureDesktop", "EnableVirtualization", "ConsentPromptBehaviorAdmin" };
            foreach (var v in sysVals)
            {
                RegistryHelper.SetDword(RegistryHive.LocalMachine, @"Software\Microsoft\Windows\CurrentVersion\Policies\System", v, 0);
            }

            string[] exts = { "batfile", "cmdfile", "exefile", "cplfile", "mscfile" };
            foreach (var ext in exts)
            {
                RegistryHelper.SetString(RegistryHive.LocalMachine, $@"Software\Classes\{ext}\shell\runas", "ProgrammaticAccessOnly", "");
            }
            RegistryHelper.SetString(RegistryHive.LocalMachine, @"Software\Classes\exefile\shell\runas2", "ProgrammaticAccessOnly", "");
        }

        public static void EnableAdmin()
        {
            ProcessRunner.RunCmd($"net user \"{Environment.UserName}\" /active:yes");
        }

        public static void UnlockRegion()
        {
            ProcessRunner.RunCmd("sc start TrustedInstaller");
            string sys32 = Path.Combine(SystemRoot, "System32");
            string targetJson = Path.Combine(sys32, "IntegratedServicesRegionPolicySet.json");
            string sourceJson = Path.Combine(WorkDir, "IntegratedServicesRegionPolicySet.json");

            if (File.Exists(targetJson))
            {
                ProcessRunner.RunTrustedInstaller($"ren \"{targetJson}\" IntegratedServicesRegionPolicySet.json_bak");
            }
            if (File.Exists(sourceJson))
            {
                ProcessRunner.RunTrustedInstaller($"copy /y \"{sourceJson}\" \"{sys32}\"");
            }
        }

        public static void KillFreezeApps()
        {
            RegistryHelper.SetString(RegistryHive.CurrentUser, @"Control Panel\Desktop", "AutoEndTasks", "1");
            RegistryHelper.SetString(RegistryHive.ClassesRoot, @"Control Panel\Desktop", "AutoEndTasks", "1");
        }

        public static void DisableRemote()
        {
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"System\ControlSet001\Control\Remote Assistance", "fAllowToGetHelp", 0);
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"System\ControlSet001\Control\Remote Assistance", "fAllowFullControl", 0);
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"System\ControlSet001\Control\Terminal Server", "fDenyTSConnections", 1);
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"System\ControlSet001\Control\Terminal Server\WinStations\RDP-Tcp", "UserAuthentication", 0);
        }

        public static void DisableStickyKeys()
        {
            RegistryHelper.SetString(RegistryHive.CurrentUser, @"Control Panel\Accessibility\StickyKeys", "Flags", "506");
        }

        public static void TTL()
        {
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"SYSTEM\ControlSet001\Services\Tcpip\Parameters", "DefaultTTL", 0x41);
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"SYSTEM\ControlSet001\Services\Tcpip6\Parameters", "DefaultTTL", 0x41);
        }

        public static void DisableNotificationsAds()
        {
            string cdm = @"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager";
            RegistryHelper.SetDword(RegistryHive.CurrentUser, cdm, "SubscribedContent-338389Enabled", 0);
            RegistryHelper.SetDword(RegistryHive.CurrentUser, cdm, "SubscribedContent-310093Enabled", 0);
            RegistryHelper.SetDword(RegistryHive.CurrentUser, cdm, "ContentDeliveryAllowed", 0);
            RegistryHelper.SetDword(RegistryHive.CurrentUser, cdm, "SystemPaneSuggestionsEnabled", 0);

            RegistryHelper.SetDword(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\UserProfileEngagement", "ScoobeSystemSettingEnabled", 0);

            string start = @"Software\Microsoft\Windows\CurrentVersion\Start";
            RegistryHelper.SetDword(RegistryHive.CurrentUser, start, "ShowRecentList", 0);
            RegistryHelper.SetDword(RegistryHive.CurrentUser, start, "ShowFrequentList", 0);

            string adv = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
            RegistryHelper.SetDword(RegistryHive.CurrentUser, adv, "Start_TrackDocs", 0);
            RegistryHelper.SetDword(RegistryHive.CurrentUser, adv, "Start_IrisRecommendations", 0);
            RegistryHelper.SetDword(RegistryHive.CurrentUser, adv, "Start_AccountNotifications", 0);
            RegistryHelper.SetDword(RegistryHive.CurrentUser, adv, "Start_Layout", 1);

            string exp = @"Software\Microsoft\Windows\CurrentVersion\Explorer";
            RegistryHelper.SetDword(RegistryHive.CurrentUser, exp, "ShowRecent", 0);
            RegistryHelper.SetDword(RegistryHive.CurrentUser, exp, "ShowFrequent", 0);
            RegistryHelper.SetDword(RegistryHive.CurrentUser, exp, "ShowCloudFilesInQuickAccess", 0);
            RegistryHelper.SetDword(RegistryHive.CurrentUser, exp, "ShowRecommendations", 0);
        }

        public static void DNS()
        {
            DnsManager.ApplyDns(DnsManager.SelectedPreset);
        }

        public static void TakeOwnership()
        {
            // Files
            RegistryHelper.SetString(RegistryHive.ClassesRoot, @"*\shell\TakeOwnership", "", "Стать владельцем");
            RegistryHelper.SetString(RegistryHive.ClassesRoot, @"*\shell\TakeOwnership", "HasLUAShield", "");
            RegistryHelper.SetString(RegistryHive.ClassesRoot, @"*\shell\TakeOwnership", "NoWorkingDirectory", "");
            RegistryHelper.SetString(RegistryHive.ClassesRoot, @"*\shell\TakeOwnership\command", "", "powershell.exe -windowstyle hidden -command \"takeown /f \\\"%1\\\" && icacls \\\"%1\\\" /grant administrators:F\"");

            // Directories
            RegistryHelper.SetString(RegistryHive.ClassesRoot, @"Directory\shell\TakeOwnership", "", "Стать владельцем");
            RegistryHelper.SetString(RegistryHive.ClassesRoot, @"Directory\shell\TakeOwnership", "HasLUAShield", "");
            RegistryHelper.SetString(RegistryHive.ClassesRoot, @"Directory\shell\TakeOwnership", "NoWorkingDirectory", "");
            RegistryHelper.SetString(RegistryHive.ClassesRoot, @"Directory\shell\TakeOwnership\command", "", "powershell.exe -windowstyle hidden -command \"takeown /f \\\"%1\\\" /r /d y && icacls \\\"%1\\\" /grant administrators:F /t\"");
        }

        public static void OpenAdminTerminal()
        {
            RegistryHelper.SetString(RegistryHive.ClassesRoot, @"Directory\Background\shell\OpenAdminTerminal", "", "Открыть Терминал от Администратора");
            RegistryHelper.SetString(RegistryHive.ClassesRoot, @"Directory\Background\shell\OpenAdminTerminal", "HasLUAShield", "");
            RegistryHelper.SetString(RegistryHive.ClassesRoot, @"Directory\Background\shell\OpenAdminTerminal\command", "", "powershell.exe -Command \"Start-Process powershell -Verb RunAs -WorkingDirectory '%V'\"");
        }

        public static void CopyPath()
        {
            RegistryHelper.SetString(RegistryHive.ClassesRoot, @"*\shell\CopyPath", "", "Копировать путь к файлу");
            RegistryHelper.SetString(RegistryHive.ClassesRoot, @"*\shell\CopyPath\command", "", "powershell.exe -windowstyle hidden -command \"Set-Clipboard -Value '%1'\"");
        }

        public static void ClassicContextMenu()
        {
            RegistryHelper.SetString(RegistryHive.CurrentUser, @"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32", "", "");
        }

        public static void InstallDrivers()
        {
            string driverDir = Path.Combine(UserProfile, "Desktop", "Drivers");
            if (Directory.Exists(driverDir))
            {
                ProcessRunner.RunCmd($"pnputil /add-driver \"{Path.Combine(driverDir, "*.inf")}\" /subdirs /install");
            }
        }

        public static void InstallVC()
        {
            string vcExe = Path.Combine(WorkDir, "VisualCppRedist_AIO_x86_x64.exe");
            if (File.Exists(vcExe))
            {
                ProcessRunner.Run(vcExe, "/aiA /gm2", true, false);
            }
            string[] vcs = { "vcredist08_x64", "vcredist08_x86", "vcredist09_x64", "vcredist09_x86", "vcredist10_x64", "vcredist10_x86", "vcredist11_x64", "vcredist11_x86", "vcredist12_x64", "vcredist12_x86", "vcredist14_x64", "vcredist14_x86" };
            foreach (var r in vcs)
            {
                RegistryHelper.DeleteSubKeyTree(RegistryHive.LocalMachine, $@"Software\Microsoft\Windows\CurrentVersion\Uninstall\{r}");
            }
        }

        public static void InstallDX()
        {
            string dxExe = Path.Combine(WorkDir, "DirectX.exe");
            if (File.Exists(dxExe))
            {
                ProcessRunner.Run(dxExe, "", true, false);
            }
        }

        public static void RemoveHome()
        {
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"Software\Microsoft\Windows\CurrentVersion\Explorer", "HubMode", 1);
            RegistryHelper.SetDword(RegistryHive.CurrentUser, @"Software\Classes\CLSID\{f874310e-b6b7-47dc-bc84-b9e6b38f5903}", "System.IsPinnedToNameSpaceTree", 0);
            RegistryHelper.SetDword(RegistryHive.CurrentUser, @"Software\Classes\Wow6432Node\CLSID\{f874310e-b6b7-47dc-bc84-b9e6b38f5903}", "System.IsPinnedToNameSpaceTree", 0);
        }

        public static void RemoveGallery()
        {
            RegistryHelper.SetDword(RegistryHive.CurrentUser, @"Software\Classes\CLSID\{e88865ea-0e1c-4e20-9aa6-edcd0212c87c}", "System.IsPinnedToNameSpaceTree", 0);
            RegistryHelper.SetDword(RegistryHive.CurrentUser, @"Software\Classes\Wow6432Node\CLSID\{e88865ea-0e1c-4e20-9aa6-edcd0212c87c}", "System.IsPinnedToNameSpaceTree", 0);
        }

        public static void RemoveNetwork()
        {
            RegistryHelper.SetDword(RegistryHive.CurrentUser, @"Software\Classes\CLSID\{F02C1A0D-BE21-4350-88B0-7367FC96EF3C}", "System.IsPinnedToNameSpaceTree", 0);
        }

        public static void DarkTheme()
        {
            string personalize = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            RegistryHelper.SetDword(RegistryHive.CurrentUser, personalize, "AppsUseLightTheme", 0);
            RegistryHelper.SetDword(RegistryHive.CurrentUser, personalize, "SystemUsesLightTheme", 0);
        }

        public static void SetWallpaper()
        {
            string img = Path.Combine(WorkDir, "1.jpg");
            string targetDir = Path.Combine(SystemRoot, "Web", "Wallpaper", "Windows");
            string targetImg = Path.Combine(targetDir, "1.jpg");
            if (File.Exists(img))
            {
                if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);
                File.Copy(img, targetImg, true);
                RegistryHelper.SetString(RegistryHive.CurrentUser, @"Control Panel\Desktop", "Wallpaper", targetImg);
            }
        }

        public static void BlueIcons()
        {
            string blueDir = Path.Combine(WorkDir, "BlueIcon");
            if (Directory.Exists(blueDir))
            {
                string imageresMun = Path.Combine(SystemRoot, "SystemResources", "imageres.dll.mun");
                ProcessRunner.RunTrustedInstaller($"ren \"{imageresMun}\" imageres.dll.mun_bak");
                ProcessRunner.RunTrustedInstaller($"copy /y \"{Path.Combine(blueDir, "imageres.dll.mun")}\" \"{Path.Combine(SystemRoot, "SystemResources")}\"");

                ProcessRunner.RunCmd($"copy /y \"{Path.Combine(blueDir, "Blank.ico")}\" \"{SystemRoot}\"");
                ProcessRunner.RunCmd($"xcopy \"{Path.Combine(blueDir, "windows")}\" \"{SystemRoot}\" /E /I /Y /H /K /C /R /F");
                ProcessRunner.RunCmd($"xcopy \"{Path.Combine(blueDir, "x64")}\" \"{ProgramFiles}\" /E /I /Y /H /K /C /R /F");
                ProcessRunner.RunCmd($"xcopy \"{Path.Combine(blueDir, "x86")}\" \"{ProgramFilesX86}\" /E /I /Y /H /K /C /R /F");
                ProcessRunner.RunCmd($"xcopy \"{Path.Combine(blueDir, "users")}\" \"{Path.Combine(SystemDrive, "Users")}\" /E /I /Y /H /K /C /R /F");

                RegistryHelper.SetString(RegistryHive.LocalMachine, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Icons", "179", Path.Combine(SystemRoot, "Blank.ico,0"), RegistryValueKind.ExpandString);
                RegistryHelper.SetString(RegistryHive.ClassesRoot, @"CompressedFolder\DefaultIcon", "", Path.Combine(SystemRoot, "System32", "imageres.dll,165"), RegistryValueKind.ExpandString);
                RegistryHelper.SetString(RegistryHive.ClassesRoot, @"ArchiveFolder\DefaultIcon", "", Path.Combine(SystemRoot, "System32", "imageres.dll,165"), RegistryValueKind.ExpandString);
            }
        }

        public static void Icaros()
        {
            string icarosSrc = Path.Combine(WorkDir, "Icaros");
            string icarosDest = Path.Combine(ProgramFiles, "WinClean", "Preview");
            if (Directory.Exists(icarosSrc))
            {
                if (!Directory.Exists(icarosDest)) Directory.CreateDirectory(icarosDest);
                foreach (var f in Directory.GetFiles(icarosSrc))
                {
                    File.Copy(f, Path.Combine(icarosDest, Path.GetFileName(f)), true);
                }

                string clsid = "{c5aec3ec-e812-4677-a9a7-4fee1f9aa000}";
                RegistryHelper.SetString(RegistryHive.LocalMachine, $@"SOFTWARE\Classes\CLSID\{clsid}", "", "Icaros Thumbnail Provider");
                RegistryHelper.SetString(RegistryHive.LocalMachine, $@"SOFTWARE\Classes\CLSID\{clsid}\InProcServer32", "", Path.Combine(icarosDest, "IcarosThumbnailProvider.dll"));
                RegistryHelper.SetString(RegistryHive.LocalMachine, $@"SOFTWARE\Classes\CLSID\{clsid}\InProcServer32", "ThreadingModel", "Apartment");
                RegistryHelper.SetString(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Approved", clsid, "Icaros Thumbnail Provider");

                RegistryHelper.SetDword(RegistryHive.CurrentUser, @"Software\Icaros", "Cache", 2);
                RegistryHelper.SetDword(RegistryHive.CurrentUser, @"Software\Icaros", "FrameThresh", 20);
                RegistryHelper.SetDword(RegistryHive.CurrentUser, @"Software\Icaros", "UseCoverArt", 2);
                RegistryHelper.SetString(RegistryHive.CurrentUser, @"Software\Icaros\Cache", "Location", icarosDest);
                RegistryHelper.SetDword(RegistryHive.CurrentUser, @"Software\Icaros\Cache\Locations", icarosDest, 33554453);

                string[] exts = { ".3g2", ".3gp", ".3gp2", ".3gpp", ".ai", ".aiff", ".amv", ".ape", ".asf", ".avi", ".avif", ".bik", ".bmp", ".cb7", ".cbr", ".cbz", ".cur", ".dds", ".divx", ".dpg", ".dv", ".dvr-ms", ".eps", ".epub", ".evo", ".exr", ".f4v", ".flac", ".flv", ".gif", ".hdmov", ".hdr", ".heic", ".heif", ".indd", ".jpg", ".k3g", ".m1v", ".m2t", ".m2ts", ".m2v", ".m4b", ".m4p", ".m4v", ".mk3d", ".mka", ".mkv", ".mov", ".mp2v", ".mp3", ".mp4", ".mp4v", ".mpc", ".mpe", ".mpeg", ".mpg", ".mpv2", ".mpv4", ".mqv", ".mts", ".mxf", ".nsv", ".odp", ".ods", ".odt", ".ofr", ".ofs", ".ogg", ".ogm", ".ogv", ".opus", ".png", ".psd", ".psxprj", ".px", ".qt", ".ram", ".rm", ".rmvb", ".skm", ".spx", ".swf", ".tak", ".tga", ".tif", ".tiff", ".tp", ".tpr", ".trp", ".ts", ".tta", ".vob", ".wav", ".webm", ".webp", ".wm", ".wmv", ".wv", ".xvid" };
                foreach (var ext in exts)
                {
                    RegistryHelper.SetString(RegistryHive.LocalMachine, $@"Software\Classes\{ext}\ShellEx\{{e357fccd-a995-4576-b01f-234630154e96}}", "", clsid);
                    RegistryHelper.SetString(RegistryHive.LocalMachine, $@"Software\Classes\{ext}\ShellEx\{{BB2E617C-0920-11d1-9A0B-00C04FC2D6C1}}", "", clsid);
                }
            }
        }

        public static void TraySeconds()
        {
            RegistryHelper.SetDword(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ShowSecondsInSystemClock", 1);
        }

        public static void TrayDate()
        {
            RegistryHelper.SetString(RegistryHive.CurrentUser, @"Control Panel\International", "sShortDate", "ddd, dd.MM.yy");
        }

        public static void TaskbarEndTask()
        {
            RegistryHelper.SetDword(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\TaskbarDeveloperSettings", "TaskbarEndTask", 1);
        }

        public static void RemoveTaskbarIcons()
        {
            RegistryHelper.SetDword(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Search", "SearchboxTaskbarMode", 0);
            RegistryHelper.SetDword(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ShowTaskViewButton", 0);
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Dsh", "AllowNewsAndInterests", 0);
        }

        public static void HideRecommended()
        {
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\PolicyManager\current\device\Start", "HideRecommendedSection", 1);
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\PolicyManager\current\device\Education", "IsEducationEnvironment", 1);
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Explorer", "HideRecommendedSection", 1);
        }

        public static void StartSettingsIcon()
        {
            byte[] bytes = new byte[] { 0x86, 0x08, 0x73, 0x52, 0xAA, 0x51, 0x43, 0x42, 0x9F, 0x7B, 0x27, 0x76, 0x58, 0x46, 0x59, 0xD4 };
            RegistryHelper.SetBinary(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Start", "VisiblePlaces", bytes);
        }

        public static void WallpaperQuality()
        {
            RegistryHelper.SetDword(RegistryHive.CurrentUser, @"Control Panel\Desktop", "JPEGImportQuality", 0x64);
        }

        public static void RemoveLockScreen()
        {
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"Software\Policies\Microsoft\Windows\Personalization", "NoLockScreen", 1);
            RegistryHelper.SetDword(RegistryHive.LocalMachine, @"Software\Policies\Microsoft\Windows\Personalization", "NoLockScreenCamera", 1);
        }

        public static void NoIconShadow()
        {
            RegistryHelper.SetDword(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ListviewShadow", 0);
        }

        public static void ExplorerThisPC()
        {
            RegistryHelper.SetDword(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "LaunchTo", 0);
        }

        public static void ShowExtensions()
        {
            RegistryHelper.SetDword(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "HideFileExt", 0);
        }

        public static void CompressOS(Action<string>? statusCallback = null)
        {
            statusCallback?.Invoke("Включение сжатия CompactOS:always...");
            ProcessRunner.Run("compact.exe", "/CompactOS:always");

            string drive = Path.GetPathRoot(SystemRoot) ?? "C:\\";

            string[] targetFolders = new[]
            {
                Path.Combine(drive, "Program Files"),
                Path.Combine(drive, "Program Files (x86)"),
                Path.Combine(drive, "ProgramData"),
                Path.Combine(drive, "Users"),
                Path.Combine(drive, "Windows")
            };

            foreach (var folder in targetFolders)
            {
                if (Directory.Exists(folder))
                {
                    statusCallback?.Invoke($"Сжатие LZX: {folder}...");
                    ProcessRunner.RunCmd($"compact.exe /c /s:\"{folder}\" /exe:LZX /i /a /q /f");
                }
            }
        }

        public static void CompressDrive(Action<string>? statusCallback = null)
        {
            statusCallback?.Invoke("Включение сжатия CompactOS:always...");
            ProcessRunner.Run("compact.exe", "/CompactOS:always");

            statusCallback?.Invoke("Полное сжатие LZX системного диска C:\\...");
            ProcessRunner.RunCmd("compact.exe /c /s:\"%SystemDrive%\\\" /exe:LZX /i /a /q /f");
        }

        private static void SafeDeleteDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SafeDeleteDirectory error: {ex.Message}");
            }
        }

        #endregion
    }
}
