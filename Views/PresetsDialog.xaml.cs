using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Win32;
using WinClickWpf.Services;
using WinClickWpf.ViewModels;

namespace WinClickWpf.Views
{
    public partial class PresetsDialog : Window
    {
        private readonly MainViewModel _mainVm;
        private bool _isClosing;

        public PresetsDialog(MainViewModel vm)
        {
            _mainVm = vm;
            InitializeComponent();

            Loaded += (s, e) =>
            {
                Opacity = 0;
                var fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(200)));
                BeginAnimation(OpacityProperty, fadeIn);
            };
        }

        private async void OnCreateRestorePointClick(object sender, RoutedEventArgs e)
        {
            bool success = await PresetBackupService.CreateRestorePointAsync("WinClick 2.0 Auto Backup", msg => { });
            if (success)
            {
                MessageBox.Show("Контрольная точка восстановления Windows успешно создана!", "Точка восстановления", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Не удалось создать точку восстановления. Убедитесь, что защита системы включена в свойствах Windows.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void OnPresetGamingClick(object sender, RoutedEventArgs e)
        {
            PresetBackupService.ApplyQuickPreset(_mainVm, "gaming");
            MessageBox.Show("Пресет «Игровой ПК (Ultimate Gaming)» применен!", "Пресеты", MessageBoxButton.OK, MessageBoxImage.Information);
            CloseAnimated();
        }

        private void OnPresetPrivacyClick(object sender, RoutedEventArgs e)
        {
            PresetBackupService.ApplyQuickPreset(_mainVm, "privacy");
            MessageBox.Show("Пресет «Максимальная приватность» применен!", "Пресеты", MessageBoxButton.OK, MessageBoxImage.Information);
            CloseAnimated();
        }

        private void OnPresetOfficeClick(object sender, RoutedEventArgs e)
        {
            PresetBackupService.ApplyQuickPreset(_mainVm, "office");
            MessageBox.Show("Пресет «Офис и учеба» применен!", "Пресеты", MessageBoxButton.OK, MessageBoxImage.Information);
            CloseAnimated();
        }

        private void OnPresetCleanClick(object sender, RoutedEventArgs e)
        {
            PresetBackupService.ApplyQuickPreset(_mainVm, "clean");
            MessageBox.Show("Все галочки успешно сняты!", "Пресеты", MessageBoxButton.OK, MessageBoxImage.Information);
            CloseAnimated();
        }

        private void OnExportJsonClick(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                Title = "Сохранить профиль WinClick",
                Filter = "Файлы пресетов (*.json)|*.json",
                FileName = "WinClick_Preset.json"
            };

            if (sfd.ShowDialog() == true)
            {
                bool exported = PresetBackupService.ExportPresetToFile(_mainVm, sfd.FileName);
                if (exported)
                {
                    MessageBox.Show("Профиль настроек успешно экспортирован в JSON!", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Ошибка сохранения файла.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OnImportJsonClick(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Title = "Выберите файл профиля WinClick",
                Filter = "Файлы пресетов (*.json)|*.json"
            };

            if (ofd.ShowDialog() == true)
            {
                bool imported = PresetBackupService.ImportPresetFromFile(_mainVm, ofd.FileName);
                if (imported)
                {
                    MessageBox.Show("Профиль настроек успешно загружен!", "Импорт", MessageBoxButton.OK, MessageBoxImage.Information);
                    CloseAnimated();
                }
                else
                {
                    MessageBox.Show("Неверный формат файла пресета.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OnCloseClick(object sender, MouseButtonEventArgs e)
        {
            CloseAnimated();
        }

        private void OnCloseDialogClick(object sender, RoutedEventArgs e)
        {
            CloseAnimated();
        }

        private void CloseAnimated()
        {
            if (_isClosing) return;
            _isClosing = true;

            var fadeOut = new DoubleAnimation(1.0, 0.0, new Duration(TimeSpan.FromMilliseconds(160)))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };

            var scaleAnim = new DoubleAnimation(1.0, 0.95, new Duration(TimeSpan.FromMilliseconds(160)))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };

            fadeOut.Completed += (s, e) =>
            {
                DialogResult = true;
                Close();
            };

            DialogScale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnim);
            DialogScale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnim);
            BeginAnimation(OpacityProperty, fadeOut);
        }
    }
}
