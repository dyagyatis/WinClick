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
    public partial class SoftwareInstallerDialog : Window
    {
        private readonly List<SoftwareApp> _allApps = new();
        private readonly ObservableCollection<SoftwareApp> _filteredApps = new();
        private string _selectedCategory = "Все";
        private bool _isInstalling;

        private double _scrollVelocity;
        private DispatcherTimer? _scrollTimer;

        public SoftwareInstallerDialog()
        {
            InitializeComponent();
            ListCatalog.ItemsSource = _filteredApps;
            _allApps = SoftwareInstallerService.GetCatalog();

            Loaded += SoftwareInstallerDialog_Loaded;
            MouseLeftButtonDown += (s, e) => { if (e.ButtonState == MouseButtonState.Pressed) DragMove(); };
        }

        private void SoftwareInstallerDialog_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
            UpdateCount();
        }

        private void ApplyFilter()
        {
            string query = TxtSearch.Text?.Trim().ToLowerInvariant() ?? "";
            _filteredApps.Clear();

            foreach (var app in _allApps)
            {
                bool matchesCategory = _selectedCategory == "Все" || app.Category.Equals(_selectedCategory, StringComparison.OrdinalIgnoreCase);
                bool matchesQuery = string.IsNullOrEmpty(query) ||
                                     app.Name.ToLowerInvariant().Contains(query) ||
                                     app.Description.ToLowerInvariant().Contains(query);

                if (matchesCategory && matchesQuery)
                {
                    _filteredApps.Add(app);
                }
            }
        }

        private void UpdateCount()
        {
            int count = _allApps.Count(a => a.IsSelected);
            TxtStatusCount.Text = $"Выбрано: {count} {GetDeclension(count, "программа", "программы", "программ")}";
            BtnInstall.IsEnabled = count > 0 && !_isInstalling;
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

        private void OnCategoryFilterChanged(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.IsChecked == true)
            {
                _selectedCategory = rb.Content?.ToString() ?? "Все";
                ApplyFilter();
            }
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void OnSelectRecommendedClick(object sender, RoutedEventArgs e)
        {
            foreach (var app in _allApps)
            {
                app.IsSelected = app.IsRecommended;
            }
            UpdateCount();
        }

        private void OnSelectAllClick(object sender, RoutedEventArgs e)
        {
            foreach (var app in _filteredApps)
            {
                app.IsSelected = true;
            }
            UpdateCount();
        }

        private void OnDeselectAllClick(object sender, RoutedEventArgs e)
        {
            foreach (var app in _filteredApps)
            {
                app.IsSelected = false;
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

        private async void OnInstallClick(object sender, RoutedEventArgs e)
        {
            var selected = _allApps.Where(a => a.IsSelected).ToList();
            if (selected.Count == 0 || _isInstalling) return;

            _isInstalling = true;
            BtnInstall.IsEnabled = false;
            PnlLog.Visibility = Visibility.Visible;
            TxtLog.Text = "=== Начало процесса установки программ ===\n";

            int total = selected.Count;
            int current = 0;

            for (int i = 0; i < total; i++)
            {
                var app = selected[i];
                current++;
                ProgInstall.Value = (double)current / total * 100.0;
                app.Status = "Установка...";
                app.StatusColor = "#EBCB8B";

                AppendLog($"\n>>> [{current}/{total}] Установка {app.Name} ({app.WingetId})...");

                bool success = await SoftwareInstallerService.InstallAppAsync(app, msg =>
                {
                    Dispatcher.Invoke(() => AppendLog(msg));
                });

                if (success)
                {
                    app.Status = "Установлено";
                    app.StatusColor = "#A3BE8C";
                }
                else
                {
                    app.Status = "Ошибка";
                    app.StatusColor = "#BF616A";
                }
            }

            AppendLog("\n=== Все выбранные программы обработаны! ===");
            _isInstalling = false;
            BtnInstall.IsEnabled = true;
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
