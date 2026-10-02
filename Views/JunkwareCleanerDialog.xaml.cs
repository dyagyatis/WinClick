using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using WinClickWpf.Models;
using WinClickWpf.Services;

namespace WinClickWpf.Views
{
    public partial class JunkwareCleanerDialog : Window
    {
        private readonly ObservableCollection<JunkwareItem> _items = new();
        private bool _isUninstalling;

        private double _scrollVelocity;
        private DispatcherTimer? _scrollTimer;

        public JunkwareCleanerDialog()
        {
            InitializeComponent();
            ListJunk.ItemsSource = _items;
            Loaded += JunkwareCleanerDialog_Loaded;
            MouseLeftButtonDown += (s, e) => { if (e.ButtonState == MouseButtonState.Pressed) DragMove(); };
        }

        private async void JunkwareCleanerDialog_Loaded(object sender, RoutedEventArgs e)
        {
            await RunScanAsync();
        }

        private async Task RunScanAsync()
        {
            _items.Clear();
            PnlLoading.Visibility = Visibility.Visible;
            PnlEmpty.Visibility = Visibility.Collapsed;
            ScrollJunk.Visibility = Visibility.Collapsed;
            TxtStatusCount.Text = "Сканирование...";
            BtnUninstall.IsEnabled = false;

            var detected = await JunkwareScannerService.ScanInstalledJunkwareAsync();

            PnlLoading.Visibility = Visibility.Collapsed;

            if (detected.Count == 0)
            {
                PnlEmpty.Visibility = Visibility.Visible;
                ScrollJunk.Visibility = Visibility.Collapsed;
                TxtStatusCount.Text = "Обнаружено: 0 программ";
                BtnUninstall.IsEnabled = false;
            }
            else
            {
                PnlEmpty.Visibility = Visibility.Collapsed;
                ScrollJunk.Visibility = Visibility.Visible;

                foreach (var item in detected)
                {
                    _items.Add(item);
                }

                UpdateCount();
            }
        }

        private void UpdateCount()
        {
            int count = _items.Count(a => a.IsSelected);
            int total = _items.Count;
            TxtStatusCount.Text = $"Выбрано: {count} из {total} {GetDeclension(total, "программы", "программ", "программ")}";
            BtnUninstall.IsEnabled = count > 0 && !_isUninstalling;
        }

        private string GetDeclension(int number, string nominativ, string genetivSingular, string genetivPlural)
        {
            int n = Math.Abs(number) % 100;
            int n1 = n % 10;
            if (n > 10 && n < 20) return genetivPlural;
            if (n1 > 1 && n1 < 5) return genetivSingular;
            if (n1 == 1) return nominativ;
            return genetivPlural;
        }

        private async void OnRescanClick(object sender, RoutedEventArgs e)
        {
            if (_isUninstalling) return;
            await RunScanAsync();
        }

        private void OnSelectAllClick(object sender, RoutedEventArgs e)
        {
            foreach (var item in _items)
            {
                item.IsSelected = true;
            }
            UpdateCount();
        }

        private void OnDeselectAllClick(object sender, RoutedEventArgs e)
        {
            foreach (var item in _items)
            {
                item.IsSelected = false;
            }
            UpdateCount();
        }

        private void OnItemCheckChanged(object sender, RoutedEventArgs e)
        {
            UpdateCount();
        }

        private void OnScrollViewerPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scroll)
            {
                e.Handled = true;
                _scrollVelocity += -e.Delta / 120.0 * 15.0;

                if (_scrollTimer == null)
                {
                    _scrollTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
                    _scrollTimer.Tick += (s, ev) =>
                    {
                        if (Math.Abs(_scrollVelocity) < 0.1)
                        {
                            _scrollTimer.Stop();
                            return;
                        }

                        double newOffset = scroll.VerticalOffset + _scrollVelocity;
                        if (newOffset < 0) { newOffset = 0; _scrollVelocity = 0; }
                        if (newOffset > scroll.ScrollableHeight) { newOffset = scroll.ScrollableHeight; _scrollVelocity = 0; }

                        scroll.ScrollToVerticalOffset(newOffset);
                        _scrollVelocity *= 0.8;
                    };
                }

                _scrollTimer.Start();
            }
        }

        private async void OnUninstallClick(object sender, RoutedEventArgs e)
        {
            var selected = _items.Where(a => a.IsSelected).ToList();
            if (selected.Count == 0 || _isUninstalling) return;

            var res = MessageBox.Show(
                $"Вы уверены, что хотите удалить выбранные программы ({selected.Count} шт.)?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (res != MessageBoxResult.Yes) return;

            _isUninstalling = true;
            BtnUninstall.IsEnabled = false;
            PnlLog.Visibility = Visibility.Visible;
            TxtLog.Text = "=== Начало удаления нежелательного ПО ===\n";

            int total = selected.Count;
            int current = 0;

            for (int i = 0; i < total; i++)
            {
                var item = selected[i];
                current++;
                ProgUninstall.Value = (double)current / total * 100.0;
                item.Status = "Удаление...";
                item.StatusColor = "#EBCB8B";

                AppendLog($"\n>>> [{current}/{total}] Удаление {item.Name}...");

                bool success = await JunkwareScannerService.UninstallJunkwareAsync(item, msg =>
                {
                    Dispatcher.Invoke(() => AppendLog(msg));
                });

                if (success)
                {
                    item.Status = "Удалено";
                    item.StatusColor = "#A3BE8C";
                }
                else
                {
                    item.Status = "Ошибка";
                    item.StatusColor = "#BF616A";
                }
            }

            AppendLog("\n=== Обработка завершена! Рекомендуется обновить список. ===");
            _isUninstalling = false;
            BtnUninstall.IsEnabled = true;
        }

        private void AppendLog(string message)
        {
            TxtLog.Text += message + "\n";
            ScrollLog.ScrollToEnd();
        }

        private bool _isClosing;
        private void CloseAnimated()
        {
            if (_isClosing) return;
            _isClosing = true;

            var duration = TimeSpan.FromMilliseconds(150);
            var ease = new CubicEase { EasingMode = EasingMode.EaseIn };

            var fadeOut = new DoubleAnimation(1.0, 0.0, new Duration(duration)) { EasingFunction = ease };
            var scaleAnim = new DoubleAnimation(1.0, 0.95, new Duration(duration)) { EasingFunction = ease };

            fadeOut.Completed += (s, e) => Close();

            DialogScale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnim);
            DialogScale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnim);
            BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }

        private void OnCloseClick(object sender, MouseButtonEventArgs e)
        {
            CloseAnimated();
        }

        private void OnCloseDialogClick(object sender, RoutedEventArgs e)
        {
            CloseAnimated();
        }
    }
}
