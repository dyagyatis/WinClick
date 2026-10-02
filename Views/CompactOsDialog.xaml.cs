using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Win32;
using WinClickWpf.Services;

namespace WinClickWpf.Views
{
    public partial class CompactOsDialog : Window
    {
        private bool _isBusy;
        private bool _isClosing;

        public CompactOsDialog()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                Opacity = 0;
                var fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(200)));
                BeginAnimation(OpacityProperty, fadeIn);
            };
        }

        private void SetBusy(bool busy, string status)
        {
            _isBusy = busy;
            BtnMode1.IsEnabled = !busy;
            BtnMode2.IsEnabled = !busy;
            BtnMode3.IsEnabled = !busy;
            BtnMode4.IsEnabled = !busy;
            BtnMode5.IsEnabled = !busy;
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

        private async void OnMode1Click(object sender, RoutedEventArgs e)
        {
            if (_isBusy) return;

            var confirm = MessageBox.Show(
                "Запустить полное сжатие Compact OS LZX [LZMS]?\n\nБудут сжаты папки:\n• C:\\Program Files\n• C:\\Program Files (x86)\n• C:\\ProgramData\n• C:\\Users\n• C:\\Windows\n\nПроцесс займет 5-10 минут на SSD (до 30-60 минут на HDD). Продолжить?",
                "COMPACT OS LZX [LZMS]",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            TxtLog.Text = "";
            SetBusy(true, "Выполняется сжатие Compact OS LZX...");

            await CompactOsService.CompressFullLzxAsync(AppendLog);

            SetBusy(false, "Сжатие LZX завершено!");
            MessageBox.Show("Полное сжатие Compact OS LZX [LZMS] успешно завершено!", "Сжатие системы", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void OnMode2Click(object sender, RoutedEventArgs e)
        {
            if (_isBusy) return;

            TxtLog.Text = "";
            SetBusy(true, "Выполняется сжатие Compact OS Normal...");

            await CompactOsService.CompressNormalAsync(AppendLog);

            SetBusy(false, "Сжатие завершено!");
            MessageBox.Show("Стандартное сжатие Compact OS завершено!", "Сжатие системы", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void OnMode3Click(object sender, RoutedEventArgs e)
        {
            if (_isBusy) return;

            var ofd = new OpenFileDialog
            {
                Title = "Выберите любой файл внутри папки, которую хотите сжать",
                CheckFileExists = false,
                FileName = "Выберите эту папку"
            };

            if (ofd.ShowDialog() == true)
            {
                string? folder = System.IO.Path.GetDirectoryName(ofd.FileName);
                if (string.IsNullOrWhiteSpace(folder) || !System.IO.Directory.Exists(folder)) return;

                TxtLog.Text = "";
                SetBusy(true, $"Сжатие папки {System.IO.Path.GetFileName(folder)}...");

                await CompactOsService.CompressSpecificFolderAsync(folder, AppendLog);

                SetBusy(false, "Сжатие папки завершено!");
                MessageBox.Show($"Папка «{folder}» успешно сжата алгоритмом LZX!", "Сжатие папки", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void OnMode4Click(object sender, RoutedEventArgs e)
        {
            if (_isBusy) return;

            var confirm = MessageBox.Show(
                "Вы уверены, что хотите распаковать C:\\Windows?\nЭто восстановит исходный размер системных файлов.",
                "Распаковка C:\\Windows",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            TxtLog.Text = "";
            SetBusy(true, "Распаковка C:\\Windows...");

            await CompactOsService.UncompressWindowsAsync(AppendLog);

            SetBusy(false, "Распаковка завершена!");
            MessageBox.Show("Распаковка C:\\Windows успешно завершена!", "Распаковка", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void OnMode5Click(object sender, RoutedEventArgs e)
        {
            if (_isBusy) return;

            var ofd = new OpenFileDialog
            {
                Title = "Выберите любой файл внутри папки, которую хотите распаковать",
                CheckFileExists = false,
                FileName = "Выберите эту папку"
            };

            if (ofd.ShowDialog() == true)
            {
                string? folder = System.IO.Path.GetDirectoryName(ofd.FileName);
                if (string.IsNullOrWhiteSpace(folder) || !System.IO.Directory.Exists(folder)) return;

                TxtLog.Text = "";
                SetBusy(true, $"Распаковка папки {System.IO.Path.GetFileName(folder)}...");

                await CompactOsService.UncompressSpecificFolderAsync(folder, AppendLog);

                SetBusy(false, "Распаковка завершена!");
                MessageBox.Show($"Папка «{folder}» успешно распакована!", "Распаковка папки", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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
