using System.Diagnostics;
using System.Windows;
using WinClickWpf.Services;

namespace WinClickWpf.Views
{
    public partial class RebootDialog : Window
    {
        public RebootDialog()
        {
            InitializeComponent();
            TxtPrompt.Text = LocalizationService.GetString("RebootTitle");
            BtnYes.Content = LocalizationService.GetString("BtnYes");
            BtnNo.Content = LocalizationService.GetString("BtnNo");
        }

        private bool _isClosing;
        private void CloseAnimated()
        {
            if (_isClosing) return;
            _isClosing = true;

            var duration = TimeSpan.FromMilliseconds(150);
            var ease = new System.Windows.Media.Animation.CubicEase { EasingMode = System.Windows.Media.Animation.EasingMode.EaseIn };

            var fadeOut = new System.Windows.Media.Animation.DoubleAnimation(1.0, 0.0, new Duration(duration)) { EasingFunction = ease };
            var scaleAnim = new System.Windows.Media.Animation.DoubleAnimation(1.0, 0.95, new Duration(duration)) { EasingFunction = ease };

            fadeOut.Completed += (s, e) => Close();

            DialogScale.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleXProperty, scaleAnim);
            DialogScale.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleYProperty, scaleAnim);
            BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }

        private void OnYesClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "shutdown.exe",
                    Arguments = "/r /t 1 /f",
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });
            }
            catch { }

            CloseAnimated();
        }

        private void OnNoClick(object sender, RoutedEventArgs e)
        {
            CloseAnimated();
        }
    }
}
