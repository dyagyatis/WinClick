using System.Text;
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
    public partial class SystemInfoDialog : Window
    {
        private SystemSpecs? _specs;
        private double _scrollVelocity;
        private DispatcherTimer? _scrollTimer;
        private bool _isClosing;

        public SystemInfoDialog()
        {
            InitializeComponent();

            Loaded += async (s, e) =>
            {
                Opacity = 0;
                var fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(200)));
                BeginAnimation(OpacityProperty, fadeIn);

                _specs = await SystemInfoService.GetSpecsAsync();

                TxtOs.Text = $"{_specs.OsName} ({_specs.OsArchitecture})";
                TxtBuild.Text = _specs.OsBuild;
                TxtPcUser.Text = $"ПК: {_specs.PcName}  •  Пользователь: {_specs.UserName}  •  Время работы: {_specs.Uptime}";

                TxtCpu.Text = _specs.CpuName;
                TxtCpuCores.Text = _specs.CpuCores;

                TxtGpu.Text = _specs.GpuName;
                TxtRam.Text = _specs.RamTotal;
                TxtStorage.Text = _specs.StorageInfo;
            };
        }

        private void OnCopySpecsClick(object sender, RoutedEventArgs e)
        {
            if (_specs == null) return;

            var sb = new StringBuilder();
            sb.AppendLine("=== Спецификация компьютера (WinClick 2.0) ===");
            sb.AppendLine($"ОС: {_specs.OsName} ({_specs.OsArchitecture}) {_specs.OsBuild}");
            sb.AppendLine($"Процессор: {_specs.CpuName} ({_specs.CpuCores})");
            sb.AppendLine($"Видеокарта: {_specs.GpuName}");
            sb.AppendLine($"Оперативная память: {_specs.RamTotal}");
            sb.AppendLine($"Дисковое пространство:\n{_specs.StorageInfo}");
            sb.AppendLine($"Материнская плата: {_specs.Motherboard}");
            sb.AppendLine($"ПК: {_specs.PcName} | Пользователь: {_specs.UserName}");

            try
            {
                Clipboard.SetText(sb.ToString());
                MessageBox.Show("Характеристики компьютера успешно скопированы в буфер обмена!", "WinClick", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch { }
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
