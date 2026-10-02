using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WinClickWpf.Models;
using WinClickWpf.Services;

namespace WinClickWpf.Views
{
    public partial class UwpSelectionDialog : Window
    {
        private List<UwpAppInfo> _allApps = new();
        private readonly ObservableCollection<UwpAppInfo> _filteredApps = new();

        public List<UwpAppInfo> SelectedApps => _allApps.Where(a => a.IsSelected).ToList();
        public bool HasConfirmed { get; private set; }

        public UwpSelectionDialog(List<UwpAppInfo>? initialApps = null)
        {
            InitializeComponent();
            ListApps.ItemsSource = _filteredApps;
            Loaded += UwpSelectionDialog_Loaded;
            MouseLeftButtonDown += (s, e) => { if (e.ButtonState == MouseButtonState.Pressed) DragMove(); };

            if (initialApps != null && initialApps.Count > 0)
            {
                _allApps = initialApps;
            }
        }

        private async void UwpSelectionDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (_allApps.Count == 0)
            {
                PnlLoading.Visibility = Visibility.Visible;
                ScrollApps.Visibility = Visibility.Collapsed;

                _allApps = await UwpManager.GetInstalledAppsAsync();
            }

            PnlLoading.Visibility = Visibility.Collapsed;
            ScrollApps.Visibility = Visibility.Visible;

            ApplyFilter();
            UpdateCount();
        }

        private void ApplyFilter()
        {
            string query = TxtSearch.Text?.Trim().ToLowerInvariant() ?? "";
            _filteredApps.Clear();

            foreach (var app in _allApps)
            {
                if (string.IsNullOrEmpty(query) ||
                    app.DisplayName.ToLowerInvariant().Contains(query) ||
                    app.PackageName.ToLowerInvariant().Contains(query))
                {
                    _filteredApps.Add(app);
                }
            }
        }

        private void UpdateCount()
        {
            int selected = _allApps.Count(a => a.IsSelected);
            int total = _allApps.Count;
            TxtStatusCount.Text = $"Выбрано: {selected} из {total}";
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
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

        private double _scrollVelocity;
        private System.Windows.Threading.DispatcherTimer? _scrollTimer;

        private void OnScrollViewerPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scroll)
            {
                e.Handled = true;
                _scrollVelocity += -e.Delta / 120.0 * 15.0;

                if (_scrollTimer == null)
                {
                    _scrollTimer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
                    _scrollTimer.Tick += (s, ev) =>
                    {
                        if (Math.Abs(_scrollVelocity) < 0.1)
                        {
                            _scrollTimer.Stop();
                            return;
                        }

                        double newOffset = scroll.VerticalOffset + _scrollVelocity;
                        if (newOffset < 0)
                        {
                            newOffset = 0;
                            _scrollVelocity = 0;
                        }
                        if (newOffset > scroll.ScrollableHeight)
                        {
                            newOffset = scroll.ScrollableHeight;
                            _scrollVelocity = 0;
                        }

                        scroll.ScrollToVerticalOffset(newOffset);
                        _scrollVelocity *= 0.8;
                    };
                }

                _scrollTimer.Start();
            }
        }

        private bool _isClosing;
        private void CloseAnimated(bool? result = null)
        {
            if (_isClosing) return;
            _isClosing = true;

            var duration = TimeSpan.FromMilliseconds(150);
            var ease = new CubicEase { EasingMode = EasingMode.EaseIn };

            var fadeOut = new DoubleAnimation(1.0, 0.0, new Duration(duration)) { EasingFunction = ease };
            var scaleAnim = new DoubleAnimation(1.0, 0.95, new Duration(duration)) { EasingFunction = ease };

            fadeOut.Completed += (s, e) =>
            {
                try { DialogResult = result; } catch { }
                Close();
            };

            DialogScale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnim);
            DialogScale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnim);
            BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            HasConfirmed = true;
            CloseAnimated(true);
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            CloseAnimated(false);
        }

        private void OnCloseClick(object sender, MouseButtonEventArgs e)
        {
            CloseAnimated(false);
        }
    }
}
