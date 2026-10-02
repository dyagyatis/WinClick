using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Microsoft.Win32;
using WinClickWpf.Models;
using WinClickWpf.Services;

namespace WinClickWpf.Views
{
    public partial class StartupManagerDialog : Window
    {
        private List<StartupItem> _allItems = new();
        private readonly ObservableCollection<StartupItem> _filteredItems = new();
        private StartupCategoryType _activeCategory = StartupCategoryType.Programs;
        private double _scrollVelocity;
        private DispatcherTimer? _scrollTimer;
        private bool _isClosing;

        public StartupManagerDialog()
        {
            InitializeComponent();
            ListStartup.ItemsSource = _filteredItems;

            Loaded += async (s, e) =>
            {
                Opacity = 0;
                var fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(200)));
                BeginAnimation(OpacityProperty, fadeIn);

                await LoadStartupItemsAsync();
            };
        }

        private async Task LoadStartupItemsAsync()
        {
            if (LoadingOverlay != null)
            {
                LoadingOverlay.Opacity = 1;
                LoadingOverlay.Visibility = Visibility.Visible;
            }

            _allItems = await StartupManagerService.GetStartupItemsAsync();
            UpdateCounts();
            ApplyFilter();

            if (LoadingOverlay != null)
            {
                var fadeOut = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(220)))
                {
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };
                fadeOut.Completed += (s, e) =>
                {
                    LoadingOverlay.Visibility = Visibility.Collapsed;
                };
                LoadingOverlay.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            }
        }

        private void UpdateCounts()
        {
            int progCount = _allItems.Count(i => i.Category == StartupCategoryType.Programs);
            int taskCount = _allItems.Count(i => i.Category == StartupCategoryType.Tasks);
            int servCount = _allItems.Count(i => i.Category == StartupCategoryType.Services);

            TxtCountPrograms.Text = progCount.ToString();
            TxtCountTasks.Text = taskCount.ToString();
            TxtCountServices.Text = servCount.ToString();
        }

        private void ApplyFilter()
        {
            string query = TxtSearch?.Text.Trim().ToLower() ?? "";
            var categoryItems = _allItems.Where(i => i.Category == _activeCategory);

            if (!string.IsNullOrWhiteSpace(query))
            {
                categoryItems = categoryItems.Where(i =>
                    i.Name.ToLower().Contains(query) ||
                    i.Publisher.ToLower().Contains(query) ||
                    i.Command.ToLower().Contains(query));
            }

            _filteredItems.Clear();
            foreach (var item in categoryItems)
            {
                _filteredItems.Add(item);
            }

            int enabledCount = _filteredItems.Count(i => i.IsEnabled);
            int disabledCount = _filteredItems.Count(i => !i.IsEnabled);
            TxtSummary.Text = $"Всего в разделе: {_filteredItems.Count} • Включено: {enabledCount} • Отключено: {disabledCount}";
        }

        private void OnTabProgramsClick(object sender, MouseButtonEventArgs e)
        {
            _activeCategory = StartupCategoryType.Programs;
            UpdateTabVisuals(TabPrograms);
            ApplyFilter();
        }

        private void OnTabTasksClick(object sender, MouseButtonEventArgs e)
        {
            _activeCategory = StartupCategoryType.Tasks;
            UpdateTabVisuals(TabTasks);
            ApplyFilter();
        }

        private void OnTabServicesClick(object sender, MouseButtonEventArgs e)
        {
            _activeCategory = StartupCategoryType.Services;
            UpdateTabVisuals(TabServices);
            ApplyFilter();
        }

        private void UpdateTabVisuals(Border activeTab)
        {
            var tabs = new[] { TabPrograms, TabTasks, TabServices };
            foreach (var tab in tabs)
            {
                if (tab == activeTab)
                {
                    tab.Background = new SolidColorBrush(Color.FromRgb(94, 129, 172));
                    tab.BorderBrush = null;
                    tab.BorderThickness = new Thickness(0);
                    if (tab.Child is StackPanel sp && sp.Children[0] is TextBlock tb)
                    {
                        tb.Foreground = Brushes.White;
                    }
                }
                else
                {
                    tab.Background = new SolidColorBrush(Color.FromRgb(30, 34, 40));
                    tab.BorderBrush = new SolidColorBrush(Color.FromArgb(0x33, 0x80, 0x80, 0x80));
                    tab.BorderThickness = new Thickness(1);
                    if (tab.Child is StackPanel sp && sp.Children[0] is TextBlock tb)
                    {
                        tb.Foreground = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA));
                    }
                }
            }
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private async void OnItemToggleClick(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb && cb.DataContext is StartupItem item)
            {
                bool targetState = cb.IsChecked == true;
                bool success = await StartupManagerService.ToggleItemAsync(item, targetState);
                if (!success)
                {
                    // Revert check
                    item.IsEnabled = !targetState;
                    MessageBox.Show("Не удалось изменить статус автозапуска. Возможно, требуются права администратора.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                ApplyFilter();
            }
        }

        private void OnOpenFileLocationClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is StartupItem item)
            {
                StartupManagerService.OpenFileLocation(item);
            }
        }

        private void OnSearchOnlineClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is StartupItem item)
            {
                StartupManagerService.SearchOnline(item);
            }
        }

        private async void OnDeleteItemClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is StartupItem item)
            {
                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить «{item.Name}» из автозагрузки?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    bool deleted = await StartupManagerService.DeleteItemAsync(item);
                    if (deleted)
                    {
                        _allItems.Remove(item);
                        UpdateCounts();
                        ApplyFilter();
                    }
                    else
                    {
                        MessageBox.Show("Не удалось удалить запись. Возможно, она защищена системой.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        private async void OnAddProgramClick(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Title = "Выберите исполняемый файл для автозапуска",
                Filter = "Программы и ярлыки (*.exe;*.lnk;*.bat)|*.exe;*.lnk;*.bat|Все файлы (*.*)|*.*",
                Multiselect = false
            };

            if (ofd.ShowDialog() == true)
            {
                string filePath = ofd.FileName;
                string progName = Path.GetFileNameWithoutExtension(filePath);

                bool added = StartupManagerService.AddNewStartupProgram(progName, filePath);
                if (added)
                {
                    await LoadStartupItemsAsync();
                    MessageBox.Show($"Программа «{progName}» успешно добавлена в автозагрузку!", "Автозапуск", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Не удалось добавить запись в реестр автозапуска.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void OnRefreshClick(object sender, RoutedEventArgs e)
        {
            await LoadStartupItemsAsync();
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
                            _scrollTimer = null;
                            return;
                        }
                        scroll.ScrollToVerticalOffset(scroll.VerticalOffset + _scrollVelocity);
                        _scrollVelocity *= 0.85;
                    };
                    _scrollTimer.Start();
                }
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
