using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WinClickWpf.Services;

namespace WinClickWpf.Views
{
    public partial class SystemDoctorDialog : Window
    {
        private bool _isBusy;
        private bool _isClosing;

        public SystemDoctorDialog()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                Opacity = 0;
                var fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(200)));
                BeginAnimation(OpacityProperty, fadeIn);
            };
        }

        private void SetBusy(bool busy)
        {
            _isBusy = busy;
            BtnSfcDism.IsEnabled = !busy;
            BtnResetNet.IsEnabled = !busy;
            BtnFixStore.IsEnabled = !busy;
        }

        private void AppendLog(string message)
        {
            Dispatcher.Invoke(() =>
            {
                TxtLog.Text += "\n" + message;
                ScrollLog.ScrollToEnd();
            });
        }

        private async void OnRunSfcDismClick(object sender, RoutedEventArgs e)
        {
            if (_isBusy) return;

            var confirm = MessageBox.Show(
                "Запустить полное сканирование и автовосстановление системных файлов (SFC + DISM)?\nПроцесс может занять 5-15 минут.",
                "Восстановление системы",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            TxtLog.Text = "";
            SetBusy(true);

            await SystemDoctorService.RunFullSystemCheckAsync(AppendLog);

            SetBusy(false);
            MessageBox.Show("Проверка и восстановление файлов системы завершены!", "Доктор системы", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void OnResetNetworkClick(object sender, RoutedEventArgs e)
        {
            if (_isBusy) return;

            TxtLog.Text = "";
            SetBusy(true);

            await SystemDoctorService.ResetNetworkStackAsync(AppendLog);

            SetBusy(false);
            MessageBox.Show("Сетевой стек успешно сброшен и обновлен!", "Сброс сети", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void OnRepairStoreClick(object sender, RoutedEventArgs e)
        {
            if (_isBusy) return;

            TxtLog.Text = "";
            SetBusy(true);

            await SystemDoctorService.RepairWindowsStoreAsync(AppendLog);

            SetBusy(false);
            MessageBox.Show("Microsoft Store успешно восстановлен!", "Ремонт Store", MessageBoxButton.OK, MessageBoxImage.Information);
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
