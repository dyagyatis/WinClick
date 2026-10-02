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
    public partial class DiskCleanerDialog : Window
    {
        private readonly ObservableCollection<CleanableCategory> _categories;
        private double _scrollVelocity;
        private DispatcherTimer? _scrollTimer;
        private bool _isCleaning;
        private bool _isClosing;

        public DiskCleanerDialog()
        {
            InitializeComponent();
            _categories = new ObservableCollection<CleanableCategory>(DiskCleanerService.GetDefaultCategories());
            ListCategories.ItemsSource = _categories;

            Loaded += async (s, e) =>
            {
                Opacity = 0;
                var fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(200)));
                BeginAnimation(OpacityProperty, fadeIn);

                await StartScanAsync();
            };
        }

        private async Task StartScanAsync()
        {
            TxtStatus.Text = "Сканирование временных файлов...";
            long total = 0;

            foreach (var cat in _categories)
            {
                await DiskCleanerService.ScanCategoryAsync(cat);
                if (cat.IsSelected) total += cat.Bytes;
            }

            UpdateTotalSummary();
            TxtStatus.Text = "Сканирование завершено.";
        }

        private void UpdateTotalSummary()
        {
            long total = _categories.Where(c => c.IsSelected).Sum(c => c.Bytes);
            if (total <= 0)
            {
                TxtTotalFound.Text = "Мусор не обнаружен (0 МБ)";
            }
            else if (total < 1024 * 1024)
            {
                TxtTotalFound.Text = $"Найдено для очистки: {total / 1024.0:F1} КБ";
            }
            else if (total < 1024L * 1024 * 1024)
            {
                TxtTotalFound.Text = $"Найдено для очистки: {total / (1024.0 * 1024):F1} МБ";
            }
            else
            {
                TxtTotalFound.Text = $"Найдено для очистки: {total / (1024.0 * 1024 * 1024):F2} ГБ";
            }
        }

        private void OnCategoryCheckChanged(object sender, RoutedEventArgs e)
        {
            UpdateTotalSummary();
        }

        private async void OnScanClick(object sender, RoutedEventArgs e)
        {
            if (_isCleaning) return;
            await StartScanAsync();
        }

        private async void OnCleanClick(object sender, RoutedEventArgs e)
        {
            var selected = _categories.Where(c => c.IsSelected && c.Bytes > 0).ToList();
            if (selected.Count == 0)
            {
                MessageBox.Show("Нет выбранных категорий с файлами для очистки.", "Очистка диска", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _isCleaning = true;
            BtnClean.IsEnabled = false;
            PnlProgress.Visibility = Visibility.Visible;
            TxtLog.Text = "=== Начало очистки диска ===\n";

            int totalCount = selected.Count;
            int done = 0;
            long totalCleaned = 0;

            for (int i = 0; i < totalCount; i++)
            {
                var cat = selected[i];
                done++;
                ProgClean.Value = (double)done / totalCount * 100.0;
                TxtStatus.Text = $"Очистка: {cat.Title}...";
                AppendLog($"\n>>> Очистка {cat.Title}...");

                long cleaned = await DiskCleanerService.CleanCategoryAsync(cat, msg =>
                {
                    Dispatcher.Invoke(() => AppendLog(msg));
                });

                totalCleaned += cleaned;
            }

            string freedStr = totalCleaned >= 1024L * 1024 * 1024
                ? $"{totalCleaned / (1024.0 * 1024 * 1024):F2} ГБ"
                : $"{totalCleaned / (1024.0 * 1024):F1} МБ";

            AppendLog($"\n=== Очистка успешно завершена! Освобождено: {freedStr} ===");
            TxtStatus.Text = $"Очистка завершена! Освобождено {freedStr}.";
            UpdateTotalSummary();

            _isCleaning = false;
            BtnClean.IsEnabled = true;
        }

        private void AppendLog(string msg)
        {
            TxtLog.Text += msg + "\n";
            ScrollLog.ScrollToEnd();
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
