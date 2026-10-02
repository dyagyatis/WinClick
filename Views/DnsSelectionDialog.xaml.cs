using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WinClickWpf.Models;
using WinClickWpf.Services;

namespace WinClickWpf.Views
{
    public partial class DnsSelectionDialog : Window
    {
        public DnsPreset SelectedPreset { get; private set; }

        private double _scrollVelocity;
        private DispatcherTimer? _scrollTimer;

        public DnsSelectionDialog(DnsPreset? current = null)
        {
            InitializeComponent();
            SelectedPreset = current ?? DnsManager.SelectedPreset;
            ListPresets.ItemsSource = DnsManager.Presets;

            Loaded += (s, e) => UpdateSelectionUi();
            MouseLeftButtonDown += (s, e) => { if (e.ButtonState == MouseButtonState.Pressed) DragMove(); };
        }

        private void OnPresetItemClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is DnsPreset preset)
            {
                SelectedPreset = preset;
                UpdateSelectionUi();
            }
        }

        private void OnPresetItemMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                var item = border.DataContext as DnsPreset;
                if (item != SelectedPreset)
                {
                    border.Background = new SolidColorBrush(Color.FromRgb(26, 30, 38));
                    border.BorderBrush = new SolidColorBrush(Color.FromRgb(48, 54, 65));
                }
                var icon = FindVisualChild<Path>(border, "PresetIconPath");
                var name = FindVisualChild<TextBlock>(border, "TxtPresetName");
                if (icon != null) icon.Opacity = 1.0;
                if (name != null) name.Opacity = 1.0;
            }
        }

        private void OnPresetItemMouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                var item = border.DataContext as DnsPreset;
                if (item != SelectedPreset)
                {
                    border.Background = new SolidColorBrush(Color.FromRgb(22, 25, 30));
                    border.BorderBrush = new SolidColorBrush(Color.FromRgb(37, 42, 50));
                    var icon = FindVisualChild<Path>(border, "PresetIconPath");
                    var name = FindVisualChild<TextBlock>(border, "TxtPresetName");
                    if (icon != null) icon.Opacity = 0.7;
                    if (name != null) name.Opacity = 0.8;
                }
            }
        }

        private void UpdateSelectionUi()
        {
            TxtSelectedSummary.Text = $"Выбран: {SelectedPreset.Name}";

            // Update item borders and dots
            for (int i = 0; i < ListPresets.Items.Count; i++)
            {
                var container = ListPresets.ItemContainerGenerator.ContainerFromIndex(i);
                if (container != null)
                {
                    var border = FindVisualChild<Border>(container, "ItemBorder");
                    var dot = FindVisualChild<Ellipse>(container, "CheckDot");
                    var icon = FindVisualChild<Path>(container, "PresetIconPath");
                    var name = FindVisualChild<TextBlock>(container, "TxtPresetName");
                    var item = ListPresets.Items[i] as DnsPreset;

                    if (item == SelectedPreset)
                    {
                        if (border != null)
                        {
                            border.Background = new SolidColorBrush(Color.FromRgb(30, 36, 46));
                            border.BorderBrush = (Brush)FindResource("AccentBrush");
                        }
                        if (dot != null) dot.Visibility = Visibility.Visible;
                        if (icon != null) icon.Opacity = 1.0;
                        if (name != null) name.Opacity = 1.0;
                    }
                    else
                    {
                        if (border != null)
                        {
                            border.Background = new SolidColorBrush(Color.FromRgb(22, 25, 30));
                            border.BorderBrush = new SolidColorBrush(Color.FromRgb(37, 42, 50));
                        }
                        if (dot != null) dot.Visibility = Visibility.Collapsed;
                        if (icon != null) icon.Opacity = 0.7;
                        if (name != null) name.Opacity = 0.8;
                    }
                }
            }
        }

        private T? FindVisualChild<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typed && typed.Name == name) return typed;
                var desc = FindVisualChild<T>(child, name);
                if (desc != null) return desc;
            }
            return null;
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

        private bool _isClosing;
        private void CloseAnimated(bool? result = null)
        {
            if (_isClosing) return;
            _isClosing = true;

            var duration = TimeSpan.FromMilliseconds(150);
            var ease = new CubicEase { EasingMode = EasingMode.EaseIn };

            var fadeOut = new DoubleAnimation(1.0, 0.0, new Duration(duration))
            {
                EasingFunction = ease
            };
            var scaleAnim = new DoubleAnimation(1.0, 0.95, new Duration(duration))
            {
                EasingFunction = ease
            };

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
            DnsManager.SelectedPreset = SelectedPreset;
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
