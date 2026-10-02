using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WinClickWpf.Services;

namespace WinClickWpf.Views
{
    public partial class GamingHubDialog : Window
    {
        private List<MsiDeviceItem> _msiDevices = new();
        private bool _isClosing;

        public GamingHubDialog()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                Opacity = 0;
                var fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(200)));
                BeginAnimation(OpacityProperty, fadeIn);

                LoadMsiDevices();
            };
        }

        private void LoadMsiDevices()
        {
            _msiDevices = GamingLatencyService.GetMsiDevices();
            ListMsiDevices.ItemsSource = _msiDevices;
        }

        private void OnTabMsiClick(object sender, MouseButtonEventArgs e)
        {
            ShowTab(PanelMsi, TabMsi);
        }

        private void OnTabTimersClick(object sender, MouseButtonEventArgs e)
        {
            ShowTab(PanelTimers, TabTimers);
        }

        private void OnTabNetworkClick(object sender, MouseButtonEventArgs e)
        {
            ShowTab(PanelNetwork, TabNetwork);
        }

        private void OnTabPowerClick(object sender, MouseButtonEventArgs e)
        {
            ShowTab(PanelPower, TabPower);
        }

        private void ShowTab(Grid activePanel, Border activeTab)
        {
            PanelMsi.Visibility = Visibility.Collapsed;
            PanelTimers.Visibility = Visibility.Collapsed;
            PanelNetwork.Visibility = Visibility.Collapsed;
            PanelPower.Visibility = Visibility.Collapsed;

            activePanel.Visibility = Visibility.Visible;

            var tabs = new[] { TabMsi, TabTimers, TabNetwork, TabPower };
            foreach (var tab in tabs)
            {
                if (tab == activeTab)
                {
                    tab.Background = new SolidColorBrush(Color.FromRgb(94, 129, 172));
                    tab.BorderBrush = null;
                    tab.BorderThickness = new Thickness(0);
                    if (tab.Child is TextBlock tb) tb.Foreground = Brushes.White;
                }
                else
                {
                    tab.Background = new SolidColorBrush(Color.FromRgb(30, 34, 40));
                    tab.BorderBrush = new SolidColorBrush(Color.FromArgb(0x33, 0x80, 0x80, 0x80));
                    tab.BorderThickness = new Thickness(1);
                    if (tab.Child is TextBlock tb) tb.Foreground = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA));
                }
            }
        }

        private void OnEnableMsiItemClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is MsiDeviceItem item)
            {
                bool success = GamingLatencyService.SetMsiMode(item, true, "High");
                if (success)
                {
                    LoadMsiDevices();
                    MessageBox.Show($"Режим MSI (High Priority) успешно активирован для «{item.Name}»!", "MSI Mode", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Не удалось изменить параметры MSI. Требуются права администратора.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void OnApplyTimersClick(object sender, RoutedEventArgs e)
        {
            await GamingLatencyService.ApplyBcdeditTimerTweaksAsync(msg => { });
            MessageBox.Show("Таймеры низкой задержки BCDedit успешно применены!\n(Рекомендуется перезагрузить компьютер)", "Игровые таймеры", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void OnApplyNetworkClick(object sender, RoutedEventArgs e)
        {
            await GamingLatencyService.ApplyGamingNetworkTweaksAsync(msg => { });
            MessageBox.Show("Сетевые твики онлайн-игр успешно применены!\n(Пинг и отклик оптимизированы)", "Сетевой стек", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void OnApplyPowerClick(object sender, RoutedEventArgs e)
        {
            await GamingLatencyService.ApplyUltimateGamingPowerPlanAsync(msg => { });
            MessageBox.Show("Схема электропитания Ultimate Gaming и Core Unparking успешно активированы!", "Электропитание", MessageBoxButton.OK, MessageBoxImage.Information);
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
