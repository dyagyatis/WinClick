using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;

namespace WinClickWpf.Services
{
    public enum AppTheme
    {
        Dark,
        Light,
        System
    }

    public static class ThemeService
    {
        private static AppTheme _currentTheme = AppTheme.System;

        public static AppTheme CurrentTheme => _currentTheme;
        public static event Action? ThemeChanged;

        public static void Initialize()
        {
            string saved = AppConfigService.Current.Theme?.ToLower() ?? "system";
            _currentTheme = saved switch
            {
                "dark" => AppTheme.Dark,
                "light" => AppTheme.Light,
                _ => AppTheme.System
            };

            ApplyTheme(_currentTheme);
        }

        public static void SetTheme(AppTheme theme)
        {
            _currentTheme = theme;
            AppConfigService.Current.Theme = theme switch
            {
                AppTheme.Dark => "dark",
                AppTheme.Light => "light",
                _ => "system"
            };
            AppConfigService.Save();

            ApplyTheme(_currentTheme);
            ThemeChanged?.Invoke();
        }

        public static void ToggleNextTheme()
        {
            var next = _currentTheme switch
            {
                AppTheme.Dark => AppTheme.Light,
                AppTheme.Light => AppTheme.System,
                _ => AppTheme.Dark
            };
            SetTheme(next);
        }

        public static bool IsSystemDark()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                if (key != null)
                {
                    object? val = key.GetValue("AppsUseLightTheme");
                    if (val is int intVal)
                    {
                        return intVal == 0; // 0 = Dark, 1 = Light
                    }
                }
            }
            catch { }
            return true; // Default to dark if unable to detect
        }

        public static bool IsCurrentlyDark()
        {
            return _currentTheme switch
            {
                AppTheme.Dark => true,
                AppTheme.Light => false,
                _ => IsSystemDark()
            };
        }

        public static string GetThemeDisplayName()
        {
            bool isEn = LocalizationService.CurrentLanguage == AppLanguage.English;
            return _currentTheme switch
            {
                AppTheme.Dark => isEn ? "Dark" : "Тёмная",
                AppTheme.Light => isEn ? "Light" : "Светлая",
                _ => isEn ? "Auto (System)" : "Авто (Системная)"
            };
        }

        public static string GetThemeIcon()
        {
            return _currentTheme switch
            {
                AppTheme.Dark => "🌙",
                AppTheme.Light => "☀️",
                _ => "💻"
            };
        }

        public static void RefreshSystemTheme()
        {
            if (_currentTheme == AppTheme.System)
            {
                ApplyTheme(AppTheme.System);
                ThemeChanged?.Invoke();
            }
        }

        private static void ApplyTheme(AppTheme theme)
        {
            bool isDark = theme switch
            {
                AppTheme.Dark => true,
                AppTheme.Light => false,
                _ => IsSystemDark()
            };

            var res = Application.Current?.Resources;
            if (res == null) return;

            if (isDark)
            {
                // Dark Palette
                res["DarkBgBrush"] = new SolidColorBrush(Color.FromRgb(20, 22, 25)); // #141619
                res["DarkGrayBrush"] = new SolidColorBrush(Color.FromRgb(20, 22, 25)); // #141619
                res["CardBrush"] = new SolidColorBrush(Color.FromRgb(30, 34, 40)); // #1E2228
                res["CardBgBrush"] = new SolidColorBrush(Color.FromRgb(30, 34, 40)); // #1E2228
                res["CardBorderBrush"] = new SolidColorBrush(Color.FromArgb(51, 128, 128, 128)); // #33808080
                res["AccentBrush"] = new SolidColorBrush(Color.FromRgb(94, 129, 172)); // #5E81AC
                res["AccentHoverBrush"] = new SolidColorBrush(Color.FromRgb(76, 86, 106)); // #4C566A
                res["TextPrimaryBrush"] = new SolidColorBrush(Colors.White);
                res["TextSecondaryBrush"] = new SolidColorBrush(Color.FromRgb(136, 136, 136)); // #888888
                res["ListItemBgBrush"] = new SolidColorBrush(Color.FromRgb(22, 25, 30)); // #16191E
                res["HeaderBrush"] = new SolidColorBrush(Color.FromRgb(94, 129, 172)); // #5E81AC
            }
            else
            {
                // Light Palette
                res["DarkBgBrush"] = new SolidColorBrush(Color.FromRgb(243, 244, 246)); // #F3F4F6
                res["DarkGrayBrush"] = new SolidColorBrush(Color.FromRgb(243, 244, 246)); // #F3F4F6
                res["CardBrush"] = new SolidColorBrush(Color.FromRgb(255, 255, 255)); // #FFFFFF
                res["CardBgBrush"] = new SolidColorBrush(Color.FromRgb(255, 255, 255)); // #FFFFFF
                res["CardBorderBrush"] = new SolidColorBrush(Color.FromRgb(229, 231, 235)); // #E5E7EB
                res["AccentBrush"] = new SolidColorBrush(Color.FromRgb(37, 99, 235)); // #2563EB
                res["AccentHoverBrush"] = new SolidColorBrush(Color.FromRgb(29, 78, 216)); // #1D4ED8
                res["TextPrimaryBrush"] = new SolidColorBrush(Color.FromRgb(17, 24, 39)); // #111827
                res["TextSecondaryBrush"] = new SolidColorBrush(Color.FromRgb(75, 85, 99)); // #4B5563
                res["ListItemBgBrush"] = new SolidColorBrush(Color.FromRgb(249, 250, 251)); // #F9FAFB
                res["HeaderBrush"] = new SolidColorBrush(Color.FromRgb(37, 99, 235)); // #2563EB
            }
        }
    }
}
