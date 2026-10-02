using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WinClickWpf.Services;

namespace WinClickWpf.Views
{
    public partial class ServicesOptimizerDialog : Window
    {
        private bool _isClosing;

        public ServicesOptimizerDialog()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                Opacity = 0;
                var fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(200)));
                BeginAnimation(OpacityProperty, fadeIn);
            };
        }

        private void AppendLog(string message)
        {
            Dispatcher.Invoke(() =>
            {
                TxtLog.Text += "\n" + message;
                ScrollLog.ScrollToEnd();
            });
        }

        private async void OnProfileSafeClick(object sender, RoutedEventArgs e)
        {
            TxtLog.Text = "";
            await ServicesOptimizerService.ApplyServiceProfileAsync(ServiceProfileType.Safe, AppendLog);
            MessageBox.Show("Профиль «Безопасный» успешно применен!", "Службы Windows", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void OnProfileGamingClick(object sender, RoutedEventArgs e)
        {
            TxtLog.Text = "";
            await ServicesOptimizerService.ApplyServiceProfileAsync(ServiceProfileType.Gaming, AppendLog);
            MessageBox.Show("Профиль «Игровой» успешно применен!", "Службы Windows", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void OnProfileExtremeClick(object sender, RoutedEventArgs e)
        {
            var confirm = MessageBox.Show(
                "Применить профиль «Экстремальный Superlite»?\nБудут отключены службы SysMain, карт и инсайдеров.",
                "Экстремальный профиль",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes) return;

            TxtLog.Text = "";
            await ServicesOptimizerService.ApplyServiceProfileAsync(ServiceProfileType.Extreme, AppendLog);
            MessageBox.Show("Профиль «Экстремальный» успешно применен!", "Службы Windows", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void OnRestoreServicesClick(object sender, RoutedEventArgs e)
        {
            TxtLog.Text = "";
            await ServicesOptimizerService.RestoreAllServicesToDefaultAsync(AppendLog);
            MessageBox.Show("Службы успешно возвращены по умолчанию!", "Службы Windows", MessageBoxButton.OK, MessageBoxImage.Information);
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
