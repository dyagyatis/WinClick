using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WinClickWpf.Services;

namespace WinClickWpf.Views
{
    public partial class WingetUpdaterDialog : Window
    {
        private List<WingetPackageItem> _packages = new();
        private bool _isBusy;
        private bool _isClosing;

        public WingetUpdaterDialog()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                Opacity = 0;
                var fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(200)));
                BeginAnimation(OpacityProperty, fadeIn);

                ScanUpdatesAsync();
            };
        }

        private void SetBusy(bool busy, string status)
        {
            _isBusy = busy;
            BtnScan.IsEnabled = !busy;
            BtnUpgradeAll.IsEnabled = !busy;
            TxtStatus.Text = status;
        }

        private void AppendLog(string message)
        {
            Dispatcher.Invoke(() =>
            {
                TxtLog.Text += "\n" + message;
                ScrollLog.ScrollToEnd();
            });
        }

        private async void ScanUpdatesAsync()
        {
            if (_isBusy) return;

            TxtLog.Text = "Поиск доступных обновлений через Winget...";
            SetBusy(true, "Сканирование программ...");

            _packages = await WingetUpdaterService.GetAvailableUpdatesAsync();
            ListPackages.ItemsSource = _packages;

            SetBusy(false, $"Найдено обновлений: {_packages.Count}");
            TxtLog.Text = _packages.Count > 0
                ? $"Найдено {_packages.Count} доступных обновлений. Нажмите «ОБНОВИТЬ ВСЕ В 1 КЛИК»."
                : "Все установленные программы обновлены до последних версий!";
        }

        private void OnScanClick(object sender, RoutedEventArgs e)
        {
            ScanUpdatesAsync();
        }

        private async void OnUpgradeAllClick(object sender, RoutedEventArgs e)
        {
            if (_isBusy) return;

            var confirm = MessageBox.Show(
                "Запустить пакетное обновление всех установленных программ?\nПроцесс будет выполнен автоматически в фоновом режиме.",
                "Обновление программ",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            TxtLog.Text = "";
            SetBusy(true, "Выполняется обновление программ...");

            await WingetUpdaterService.UpgradeAllPackagesAsync(AppendLog);

            SetBusy(false, "Обновление завершено!");
            MessageBox.Show("Пакетное обновление программ завершено!", "Winget Updater", MessageBoxButton.OK, MessageBoxImage.Information);
            ScanUpdatesAsync();
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
