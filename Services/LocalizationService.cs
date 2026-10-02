namespace WinClickWpf.Services
{
    public enum AppLanguage
    {
        Russian,
        English
    }

    public static class LocalizationService
    {
        private static AppLanguage _currentLanguage = AppLanguage.Russian;

        public static AppLanguage CurrentLanguage => _currentLanguage;
        public static event Action? LanguageChanged;

        private static readonly Dictionary<string, string> RuStrings = new()
        {
            // App & Header
            ["AppTitle"] = "WinClick 2.0",
            ["TitleTooltip"] = "Двойной клик: Выбрать / Снять всё",
            ["BtnOptimize"] = "ОПТИМИЗИРОВАТЬ",
            ["OptimizingOverlay"] = "Применение выбранных оптимизаций...",
            ["PleaseWait"] = "Пожалуйста, подождите...",
            ["Ready"] = "Готово к оптимизации",

            // Quick Tools Bar
            ["ToolDiskCleaner"] = "Очистка диска",
            ["ToolDiskCleanerDesc"] = "Временные файлы, кэш",
            ["ToolSoftware"] = "Программы",
            ["ToolSoftwareDesc"] = "Установка в 1 клик",
            ["ToolStartup"] = "Автозапуск",
            ["ToolStartupDesc"] = "Менеджер автозагрузки",
            ["ToolCompactOs"] = "Compact OS",
            ["ToolCompactOsDesc"] = "Сжатие LZX / LZMS",
            ["ToolWinget"] = "Апдейтер Winget",
            ["ToolWingetDesc"] = "Обновление всех программ",
            ["ToolOptimizer"] = "Профили служб",
            ["ToolOptimizerDesc"] = "Игровой, Безопасный, Superlite",

            // Header Tooltips
            ["TipGamingHub"] = "🎮 Игровой хаб и задержки (MSI Mode, Таймеры, Сеть)",
            ["TipDoctor"] = "🩺 Доктор системы (SFC + DISM, Ремонт сети и Store)",
            ["TipPresets"] = "💾 Пресеты и точка восстановления (Backup)",
            ["TipSysInfo"] = "💻 Характеристики и инфо о ПК",
            ["TipJunk"] = "🛡️ Фильтр нежелательного ПО (Bloatware, Adware)",
            ["TipTheme"] = "🌓 Переключить тему (Тёмная / Светлая / Авто)",
            ["TipLang"] = "🌐 Сменить язык (RU / EN)",
            ["TipMinimize"] = "Свернуть",
            ["TipMaximize"] = "Развернуть / Восстановить",
            ["TipClose"] = "Закрыть",

            // Categories
            ["Cat_CleanMemory"] = "Очистка и память",
            ["Cat_Preinstalled"] = "Предустановленные приложения",
            ["Cat_Edge"] = "Браузер Edge и WebView2",
            ["Cat_Defender"] = "Защитник Windows",
            ["Cat_Components"] = "Компоненты Windows",
            ["Cat_Tasks"] = "Планировщик задач",
            ["Cat_Optimization"] = "Оптимизация параметров",
            ["Cat_WindowsUpdate"] = "Центр обновления Windows",
            ["Cat_UsefulTweaks"] = "Полезные твики",
            ["Cat_ContextMenu"] = "Контекстное меню",
            ["Cat_Drivers"] = "Драйверы",
            ["Cat_OtherComponents"] = "Другие компоненты",
            ["Cat_VisualTweaks"] = "Визуальные твики",

            // Custom UI elements
            ["UwpConfigBtn"] = "⚙ Выбрать отдельные UWP-приложения...",
            ["UwpConfigBtnSelected"] = "⚙ Выбрано UWP для удаления: {0}",
            ["DnsConfigBtn"] = "⚙ DNS: {0} ({1})",
            ["CategoryTooltip"] = "Нажмите, чтобы выбрать все пункты категории",

            // Reboot Dialog
            ["RebootTitle"] = "Настройка завершена!\n\nПерезагрузить ПК?",
            ["BtnYes"] = "ДА",
            ["BtnNo"] = "НЕТ",
            ["BtnSave"] = "СОХРАНИТЬ",
            ["BtnConfirm"] = "ПОДТВЕРДИТЬ",
            ["BtnCancel"] = "ОТМЕНА",
            ["BtnClose"] = "ЗАКРЫТЬ",

            // Tweaks - Clean and Memory
            ["Tweak_CompressOS"] = "Сжатие Compact OS LZX [LZMS] (5-10 мин на SSD)",
            ["Tweak_CompressDrive"] = "Сжатие диска C: LZX [LZMS] (макс. экономия)",
            ["Tweak_CompactOSFast"] = "Экспресс-сжатие ядра CompactOS",
            ["Tweak_CleanWinSxS"] = "Глубокая очистка WinSxS (DISM ResetBase)",
            ["Tweak_ClearStandbyList"] = "Очистить оперативную память (Standby List)",
            ["Tweak_DisableHibernate"] = "Отключить Гибернацию (освободить ОЗУ на диске)",
            ["Tweak_DisableReservedStorage"] = "Отключить Зарезервированное хранилище (~7 ГБ)",
            ["Tweak_RemoveUpdateFiles"] = "Удалить файлы и кэш обновлений",
            ["Tweak_RemoveStoreCache"] = "Удалить кэш Windows Store",
            ["Tweak_RemoveExplorerCache"] = "Удалить кэш эскизов Проводника",
            ["Tweak_RemoveJunkFolders"] = "Удалить лишние папки (Windows.old, PerfLogs)",
            ["Tweak_RemoveOldDrivers"] = "Удалить старые копии драйверов",
            ["Tweak_RemoveShellBags"] = "Удалить историю папок ShellBags",

            // Tweaks - Preinstalled Apps
            ["Tweak_RemoveAppx"] = "Удалить все UWP-приложения",
            ["Tweak_RemoveOneDrive"] = "Удалить OneDrive",
            ["Tweak_RemoveRemoteAssistant"] = "Удалить Помощника по удаленному подключению",
            ["Tweak_CleanStartMenu"] = "Удалить лишние папки приложений в Пуске",

            // Tweaks - Edge
            ["Tweak_RemoveEdge"] = "Удалить Microsoft Edge",
            ["Tweak_RemoveEdgeWebView"] = "Удалить Edge WebView2",

            // Tweaks - Defender
            ["Tweak_RemoveDefender"] = "Удалить Защитник Windows (DefenderKiller)",

            // Tweaks - Components
            ["Tweak_RemoveComponents"] = "Удалить все дополнительные компоненты",

            // Tweaks - Tasks
            ["Tweak_DisableTasks"] = "Отключить задачи телеметрии и проверок",

            // Tweaks - Optimization
            ["Tweak_DisableRestorePoints"] = "Отключить Точки восстановления",
            ["Tweak_DelayedServices"] = "Отложенный запуск автоматических служб",
            ["Tweak_SystemLog"] = "Минимизировать системные отчеты",
            ["Tweak_BoostIconCache"] = "Увеличить кэш иконок",
            ["Tweak_SvcSplit"] = "Увеличить порог разделения SVC",
            ["Tweak_FastFolders"] = "Ускорить открытие папок",
            ["Tweak_DisableVBS"] = "Отключить VBS и HVCI",
            ["Tweak_DisableGameDVR"] = "Отключить GameDVR",
            ["Tweak_UltimatePerformance"] = "Установить схему питания Максимальная производительность",
            ["Tweak_DisableResume"] = "Отключить функцию Возобновить",

            // Tweaks - Windows Update
            ["Tweak_DisableWUDrivers"] = "Запретить установку драйверов из ЦО",
            ["Tweak_DisableDefenderUpdates"] = "Запретить обновления удаления вредоносных программ",
            ["Tweak_PauseUpdates"] = "Установить паузу обновлений до 07.07.2077",
            ["Tweak_DisableAutoUpdates"] = "Запретить автоматические обновления",

            // Tweaks - Useful Tweaks
            ["Tweak_DisableUAC"] = "Отключить UAC",
            ["Tweak_EnableAdmin"] = "Сделать учетную запись Административной",
            ["Tweak_UnlockRegion"] = "Снять региональные ограничения",
            ["Tweak_KillFreezeApps"] = "Принудительно завершать программы при зависании",
            ["Tweak_DisableRemote"] = "Отключить Удаленный помощник",
            ["Tweak_DisableStickyKeys"] = "Отключить залипание клавиш",
            ["Tweak_TTL"] = "Скрыть реальный TTL",
            ["Tweak_DisableNotificationsAds"] = "Отключить лишние уведомления и рекомендации",
            ["Tweak_DNS"] = "Установить быстрый DNS на сетевые адаптеры",

            // Tweaks - Context Menu
            ["Tweak_TakeOwnership"] = "Добавить «Стать владельцем» (Take Ownership) в меню ПКМ",
            ["Tweak_OpenAdminTerminal"] = "Добавить «Открыть Терминал от Администратора» в меню ПКМ",
            ["Tweak_CopyPath"] = "Добавить «Копировать путь к файлу» в меню ПКМ",
            ["Tweak_ClassicContextMenu"] = "Классическое контекстное меню Windows 10 в Windows 11",

            // Tweaks - Drivers & Components
            ["Tweak_InstallDrivers"] = "Установить драйверы (Папка Drivers на Рабочем столе)",
            ["Tweak_InstallVC"] = "Установить Visual C++",
            ["Tweak_InstallDX"] = "Установить DirectX 9-11",

            // Tweaks - Visual Tweaks
            ["Tweak_RemoveHome"] = "Удалить пункт Главная в Проводнике",
            ["Tweak_RemoveGallery"] = "Удалить пункт Галерея в Проводнике",
            ["Tweak_RemoveNetwork"] = "Удалить пункт Сеть в Проводнике",
            ["Tweak_DarkTheme"] = "Установить темную тему системы",
            ["Tweak_SetWallpaper"] = "Установить кастомные обои",
            ["Tweak_BlueIcons"] = "Установить синие папки",
            ["Tweak_Icaros"] = "Установить дополнительные эскизы медиафайлов (Icaros)",
            ["Tweak_TraySeconds"] = "Установить секунды в трее",
            ["Tweak_TrayDate"] = "Установить дату в трее",
            ["Tweak_TaskbarEndTask"] = "Установить пункт Завершить задачу на Панели задач",
            ["Tweak_RemoveTaskbarIcons"] = "Удалить лишние значки на Панели задач",
            ["Tweak_HideRecommended"] = "Скрыть раздел Рекомендуем в меню Пуск",
            ["Tweak_StartSettingsIcon"] = "Установить значок Настройки в меню Пуск",
            ["Tweak_WallpaperQuality"] = "Удалить сжатие обоев Рабочего стола",
            ["Tweak_RemoveLockScreen"] = "Удалить экран блокировки",
            ["Tweak_NoIconShadow"] = "Удалить тени на значках Рабочего стола",
            ["Tweak_ExplorerThisPC"] = "Открывать Проводник в Этот компьютер",
            ["Tweak_ShowExtensions"] = "Показывать расширения файлов"
        };

        private static readonly Dictionary<string, string> EnStrings = new()
        {
            // App & Header
            ["AppTitle"] = "WinClick 2.0",
            ["TitleTooltip"] = "Double Click: Select / Deselect All",
            ["BtnOptimize"] = "OPTIMIZE NOW",
            ["OptimizingOverlay"] = "Applying selected optimizations...",
            ["PleaseWait"] = "Please wait...",
            ["Ready"] = "Ready to optimize",

            // Quick Tools Bar
            ["ToolDiskCleaner"] = "Disk Cleaner",
            ["ToolDiskCleanerDesc"] = "Temp files, cache",
            ["ToolSoftware"] = "Apps Installer",
            ["ToolSoftwareDesc"] = "1-Click silent setup",
            ["ToolStartup"] = "Startup Manager",
            ["ToolStartupDesc"] = "Manage startup items",
            ["ToolCompactOs"] = "Compact OS",
            ["ToolCompactOsDesc"] = "LZX / LZMS Compression",
            ["ToolWinget"] = "Winget Updater",
            ["ToolWingetDesc"] = "Update all software",
            ["ToolOptimizer"] = "Services Profiles",
            ["ToolOptimizerDesc"] = "Gaming, Safe, Superlite",

            // Header Tooltips
            ["TipGamingHub"] = "🎮 Gaming Hub & Latency (MSI Mode, Timers, Network)",
            ["TipDoctor"] = "🩺 System Doctor (SFC + DISM, Network & Store Repair)",
            ["TipPresets"] = "💾 Presets & Restore Point (Backup)",
            ["TipSysInfo"] = "💻 PC Specs & System Information",
            ["TipJunk"] = "🛡️ Bloatware & Adware Cleaner",
            ["TipTheme"] = "🌓 Toggle Theme (Dark / Light / Auto)",
            ["TipLang"] = "🌐 Switch Language (RU / EN)",
            ["TipMinimize"] = "Minimize",
            ["TipMaximize"] = "Maximize / Restore",
            ["TipClose"] = "Close",

            // Categories
            ["Cat_CleanMemory"] = "Cleanup and Memory",
            ["Cat_Preinstalled"] = "Preinstalled Applications",
            ["Cat_Edge"] = "Edge & WebView2 Browser",
            ["Cat_Defender"] = "Windows Defender",
            ["Cat_Components"] = "Windows Components",
            ["Cat_Tasks"] = "Task Scheduler",
            ["Cat_Optimization"] = "Settings Optimization",
            ["Cat_WindowsUpdate"] = "Windows Update",
            ["Cat_UsefulTweaks"] = "Useful Tweaks",
            ["Cat_ContextMenu"] = "Context Menu",
            ["Cat_Drivers"] = "Drivers",
            ["Cat_OtherComponents"] = "Other Components",
            ["Cat_VisualTweaks"] = "Visual Tweaks",

            // Custom UI elements
            ["UwpConfigBtn"] = "⚙ Select individual UWP apps...",
            ["UwpConfigBtnSelected"] = "⚙ Selected UWP apps to remove: {0}",
            ["DnsConfigBtn"] = "⚙ DNS: {0} ({1})",
            ["CategoryTooltip"] = "Click to select/deselect all tweaks in this category",

            // Reboot Dialog
            ["RebootTitle"] = "Optimization Complete!\n\nRestart PC now?",
            ["BtnYes"] = "YES",
            ["BtnNo"] = "NO",
            ["BtnSave"] = "SAVE",
            ["BtnConfirm"] = "CONFIRM",
            ["BtnCancel"] = "CANCEL",
            ["BtnClose"] = "CLOSE",

            // Tweaks - Clean and Memory
            ["Tweak_CompressOS"] = "Compress Compact OS LZX [LZMS] (5-10 min on SSD)",
            ["Tweak_CompressDrive"] = "Compress Drive C: LZX [LZMS] (Max disk saving)",
            ["Tweak_CompactOSFast"] = "Express CompactOS Kernel Compression",
            ["Tweak_CleanWinSxS"] = "Deep WinSxS Cleanup (DISM ResetBase)",
            ["Tweak_ClearStandbyList"] = "Clear RAM Standby List Memory",
            ["Tweak_DisableHibernate"] = "Disable Hibernation (frees RAM size on disk)",
            ["Tweak_DisableReservedStorage"] = "Disable Reserved Storage (~7 GB on C:)",
            ["Tweak_RemoveUpdateFiles"] = "Delete Windows Update cache and temp files",
            ["Tweak_RemoveStoreCache"] = "Clear Windows Store cache",
            ["Tweak_RemoveExplorerCache"] = "Clear File Explorer thumbnail cache",
            ["Tweak_RemoveJunkFolders"] = "Remove junk folders (Windows.old, PerfLogs)",
            ["Tweak_RemoveOldDrivers"] = "Delete old driver backups",
            ["Tweak_RemoveShellBags"] = "Clear ShellBags folder view history",

            // Tweaks - Preinstalled Apps
            ["Tweak_RemoveAppx"] = "Remove all built-in UWP apps",
            ["Tweak_RemoveOneDrive"] = "Completely remove Microsoft OneDrive",
            ["Tweak_RemoveRemoteAssistant"] = "Remove Quick Assist / Remote Assistant",
            ["Tweak_CleanStartMenu"] = "Clean unused app folders in Start Menu",

            // Tweaks - Edge
            ["Tweak_RemoveEdge"] = "Remove Microsoft Edge Browser",
            ["Tweak_RemoveEdgeWebView"] = "Remove Edge WebView2 Runtime",

            // Tweaks - Defender
            ["Tweak_RemoveDefender"] = "Remove Windows Defender (DefenderKiller)",

            // Tweaks - Components
            ["Tweak_RemoveComponents"] = "Remove optional Windows components",

            // Tweaks - Tasks
            ["Tweak_DisableTasks"] = "Disable telemetry and scheduled inspection tasks",

            // Tweaks - Optimization
            ["Tweak_DisableRestorePoints"] = "Disable System Restore Points",
            ["Tweak_DelayedServices"] = "Delayed auto-start for non-critical services",
            ["Tweak_SystemLog"] = "Minimize system reporting logs",
            ["Tweak_BoostIconCache"] = "Increase Explorer icon cache limit",
            ["Tweak_SvcSplit"] = "Increase Service Host splitting threshold",
            ["Tweak_FastFolders"] = "Accelerate folder navigation response",
            ["Tweak_DisableVBS"] = "Disable VBS & HVCI (boosts gaming FPS)",
            ["Tweak_DisableGameDVR"] = "Disable Xbox GameDVR background recording",
            ["Tweak_UltimatePerformance"] = "Enable Ultimate Performance Power Plan",
            ["Tweak_DisableResume"] = "Disable Windows Resume / Hibernate state",

            // Tweaks - Windows Update
            ["Tweak_DisableWUDrivers"] = "Exclude driver updates from Windows Update",
            ["Tweak_DisableDefenderUpdates"] = "Block Malicious Software Removal updates",
            ["Tweak_PauseUpdates"] = "Pause Windows Updates until 07.07.2077",
            ["Tweak_DisableAutoUpdates"] = "Disable automatic Windows updates",

            // Tweaks - Useful Tweaks
            ["Tweak_DisableUAC"] = "Disable User Account Control (UAC)",
            ["Tweak_EnableAdmin"] = "Grant full Administrator privileges to account",
            ["Tweak_UnlockRegion"] = "Unlock European/regional OS feature locks",
            ["Tweak_KillFreezeApps"] = "Auto-terminate frozen apps on shutdown",
            ["Tweak_DisableRemote"] = "Disable Remote Desktop and Assistant",
            ["Tweak_DisableStickyKeys"] = "Disable Sticky Keys shortcut prompt",
            ["Tweak_TTL"] = "Mask real network TTL (Bypass hotspot limits)",
            ["Tweak_DisableNotificationsAds"] = "Disable tips, tricks and suggested ad banners",
            ["Tweak_DNS"] = "Set Fast DNS on network adapters",

            // Tweaks - Context Menu
            ["Tweak_TakeOwnership"] = "Add 'Take Ownership' to right-click context menu",
            ["Tweak_OpenAdminTerminal"] = "Add 'Open Terminal as Admin' to right-click menu",
            ["Tweak_CopyPath"] = "Add 'Copy File Path' to right-click menu",
            ["Tweak_ClassicContextMenu"] = "Restore Windows 10 Classic Context Menu on Win 11",

            // Tweaks - Drivers & Components
            ["Tweak_InstallDrivers"] = "Install Drivers (from Desktop\\Drivers folder)",
            ["Tweak_InstallVC"] = "Install Visual C++ AIO Runtimes",
            ["Tweak_InstallDX"] = "Install DirectX 9-11 End-User Runtimes",

            // Tweaks - Visual Tweaks
            ["Tweak_RemoveHome"] = "Remove 'Home' from File Explorer sidebar",
            ["Tweak_RemoveGallery"] = "Remove 'Gallery' from File Explorer sidebar",
            ["Tweak_RemoveNetwork"] = "Remove 'Network' from File Explorer sidebar",
            ["Tweak_DarkTheme"] = "Enable System Dark Mode",
            ["Tweak_SetWallpaper"] = "Apply Custom Ultra Wallpaper",
            ["Tweak_BlueIcons"] = "Apply Sleek Blue Folder Icons",
            ["Tweak_Icaros"] = "Enable Extended Media Thumbnails (Icaros)",
            ["Tweak_TraySeconds"] = "Show seconds in Taskbar Clock",
            ["Tweak_TrayDate"] = "Show day of week in Taskbar Clock",
            ["Tweak_TaskbarEndTask"] = "Enable 'End Task' right-click option on Taskbar",
            ["Tweak_RemoveTaskbarIcons"] = "Hide search, task view and widget icons",
            ["Tweak_HideRecommended"] = "Hide 'Recommended' section in Start Menu",
            ["Tweak_StartSettingsIcon"] = "Pin Settings shortcut in Start Menu power area",
            ["Tweak_WallpaperQuality"] = "Disable desktop wallpaper JPEG compression",
            ["Tweak_RemoveLockScreen"] = "Disable Windows Lock Screen",
            ["Tweak_NoIconShadow"] = "Remove desktop icon drop shadows",
            ["Tweak_ExplorerThisPC"] = "Open File Explorer to 'This PC'",
            ["Tweak_ShowExtensions"] = "Show file extensions (.exe, .txt, .dll)"
        };

        public static void Initialize()
        {
            string saved = AppConfigService.Current.Language?.ToLower() ?? "ru";
            _currentLanguage = saved == "en" ? AppLanguage.English : AppLanguage.Russian;
        }

        public static void SetLanguage(AppLanguage lang)
        {
            _currentLanguage = lang;
            AppConfigService.Current.Language = lang == AppLanguage.English ? "en" : "ru";
            AppConfigService.Save();
            LanguageChanged?.Invoke();
        }

        public static void ToggleLanguage()
        {
            SetLanguage(_currentLanguage == AppLanguage.Russian ? AppLanguage.English : AppLanguage.Russian);
        }

        public static string GetString(string key, params object[] args)
        {
            var dict = _currentLanguage == AppLanguage.English ? EnStrings : RuStrings;
            if (dict.TryGetValue(key, out var val))
            {
                return args.Length > 0 ? string.Format(val, args) : val;
            }
            if (RuStrings.TryGetValue(key, out var fallback))
            {
                return args.Length > 0 ? string.Format(fallback, args) : fallback;
            }
            return key;
        }
    }
}
