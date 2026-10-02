using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using WinClickWpf.Models;
using WinClickWpf.Services;

namespace WinClickWpf.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private bool _isOptimizing;
        private string _statusMessage = string.Empty;
        private double _progressPercentage;
        private bool _isUpdatingCompression;

        public ObservableCollection<TweakCategory> Categories { get; } = new();
        public List<UwpAppInfo> AllLoadedUwpApps { get; set; } = new();
        public List<UwpAppInfo> SelectedSelectiveUwpApps { get; set; } = new();

        public bool IsOptimizing
        {
            get => _isOptimizing;
            set
            {
                if (_isOptimizing != value)
                {
                    _isOptimizing = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanOptimize));
                }
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public double ProgressPercentage
        {
            get => _progressPercentage;
            set
            {
                if (Math.Abs(_progressPercentage - value) > 0.001)
                {
                    _progressPercentage = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsRemoveAllUwpChecked
        {
            get
            {
                var item = Categories.SelectMany(c => c.Items).FirstOrDefault(i => i.Tag == "/RemoveAppx");
                return item?.IsChecked ?? false;
            }
        }

        public string SelectiveUwpSummary
        {
            get
            {
                int count = SelectedSelectiveUwpApps.Count(a => a.IsSelected);
                return count > 0 
                    ? LocalizationService.GetString("UwpConfigBtnSelected", count) 
                    : LocalizationService.GetString("UwpConfigBtn");
            }
        }

        public string SelectedDnsSummary => LocalizationService.GetString("DnsConfigBtn", DnsManager.SelectedPreset.Name, DnsManager.SelectedPreset.PrimaryDns);

        public bool CanOptimize
        {
            get
            {
                if (IsOptimizing) return false;
                bool anyMainTweak = Categories.SelectMany(c => c.Items).Any(i => i.IsChecked);
                bool anySelectiveUwp = !IsRemoveAllUwpChecked && SelectedSelectiveUwpApps.Any(a => a.IsSelected);
                return anyMainTweak || anySelectiveUwp;
            }
        }

        public MainViewModel()
        {
            InitializeTweaks();
            LocalizationService.LanguageChanged += () =>
            {
                foreach (var cat in Categories)
                {
                    cat.RefreshLocalization();
                }
                OnPropertyChanged(nameof(SelectiveUwpSummary));
                OnPropertyChanged(nameof(SelectedDnsSummary));
                OnPropertyChanged(nameof(StatusMessage));
            };
        }

        private void InitializeTweaks()
        {
            var tweaks = new (string CatKey, string CatName, (string TitleKey, string Title, string Tag)[])[]
            {
                ("Cat_CleanMemory", "Очистка и память", new[]
                {
                    ("Tweak_CompressOS", "Сжатие Compact OS LZX [LZMS] (5-10 мин на SSD)", "/CompressOS"),
                    ("Tweak_CompressDrive", "Сжатие диска C: LZX [LZMS] (макс. экономия)", "/CompressDrive"),
                    ("Tweak_CompactOSFast", "Экспресс-сжатие ядра CompactOS", "/CompactOSFast"),
                    ("Tweak_CleanWinSxS", "Глубокая очистка WinSxS (DISM ResetBase)", "/CleanWinSxS"),
                    ("Tweak_ClearStandbyList", "Очистить оперативную память (Standby List)", "/ClearStandbyList"),
                    ("Tweak_DisableHibernate", "Отключить Гибернацию (освободить ОЗУ на диске)", "/DisableHibernate"),
                    ("Tweak_DisableReservedStorage", "Отключить Зарезервированное хранилище (~7 ГБ)", "/DisableReservedStorage"),
                    ("Tweak_RemoveUpdateFiles", "Удалить файлы и кэш обновлений", "/RemoveUpdateFiles"),
                    ("Tweak_RemoveStoreCache", "Удалить кэш Windows Store", "/RemoveStoreCache"),
                    ("Tweak_RemoveExplorerCache", "Удалить кэш эскизов Проводника", "/RemoveExplorerCache"),
                    ("Tweak_RemoveJunkFolders", "Удалить лишние папки (Windows.old, PerfLogs)", "/RemoveJunkFolders"),
                    ("Tweak_RemoveOldDrivers", "Удалить старые копии драйверов", "/RemoveOldDrivers"),
                    ("Tweak_RemoveShellBags", "Удалить историю папок ShellBags", "/RemoveShellBags")
                }),
                ("Cat_Preinstalled", "Предустановленные приложения", new[]
                {
                    ("Tweak_RemoveAppx", "Удалить все UWP-приложения", "/RemoveAppx"),
                    ("Tweak_RemoveOneDrive", "Удалить OneDrive", "/RemoveOneDrive"),
                    ("Tweak_RemoveRemoteAssistant", "Удалить Помощника по удаленному подключению", "/RemoveRemoteAssistant"),
                    ("Tweak_CleanStartMenu", "Удалить лишние папки приложений в Пуске", "/CleanStartMenu")
                }),
                ("Cat_Edge", "Браузер Edge и WebView2", new[]
                {
                    ("Tweak_RemoveEdge", "Удалить Microsoft Edge", "/RemoveEdge"),
                    ("Tweak_RemoveEdgeWebView", "Удалить Edge WebView2", "/RemoveEdgeWebView")
                }),
                ("Cat_Defender", "Защитник Windows", new[]
                {
                    ("Tweak_RemoveDefender", "Удалить Защитник Windows (DefenderKiller)", "/RemoveDefender")
                }),
                ("Cat_Components", "Компоненты Windows", new[]
                {
                    ("Tweak_RemoveComponents", "Удалить все дополнительные компоненты", "/RemoveComponents")
                }),
                ("Cat_Tasks", "Планировщик задач", new[]
                {
                    ("Tweak_DisableTasks", "Отключить задачи телеметрии и проверок", "/DisableTasks")
                }),
                ("Cat_Optimization", "Оптимизация параметров", new[]
                {
                    ("Tweak_DisableRestorePoints", "Отключить Точки восстановления", "/DisableRestorePoints"),
                    ("Tweak_DelayedServices", "Отложенный запуск автоматических служб", "/DelayedServices"),
                    ("Tweak_SystemLog", "Минимизировать системные отчеты", "/SystemLog"),
                    ("Tweak_BoostIconCache", "Увеличить кэш иконок", "/BoostIconCache"),
                    ("Tweak_SvcSplit", "Увеличить порог разделения SVC", "/SvcSplit"),
                    ("Tweak_FastFolders", "Ускорить открытие папок", "/FastFolders"),
                    ("Tweak_DisableVBS", "Отключить VBS и HVCI", "/DisableVBS"),
                    ("Tweak_DisableGameDVR", "Отключить GameDVR", "/DisableGameDVR"),
                    ("Tweak_UltimatePerformance", "Установить схему питания Максимальная производительность", "/UltimatePerformance"),
                    ("Tweak_DisableResume", "Отключить функцию Возобновить", "/DisableResume")
                }),
                ("Cat_WindowsUpdate", "Центр обновления Windows", new[]
                {
                    ("Tweak_DisableWUDrivers", "Запретить установку драйверов из ЦО", "/DisableWUDrivers"),
                    ("Tweak_DisableDefenderUpdates", "Запретить обновления удаления вредоносных программ", "/DisableDefenderUpdates"),
                    ("Tweak_PauseUpdates", "Установить паузу обновлений до 07.07.2077", "/PauseUpdates"),
                    ("Tweak_DisableAutoUpdates", "Запретить автоматические обновления", "/DisableAutoUpdates")
                }),
                ("Cat_UsefulTweaks", "Полезные твики", new[]
                {
                    ("Tweak_DisableUAC", "Отключить UAC", "/DisableUAC"),
                    ("Tweak_EnableAdmin", "Сделать учетную запись Административной", "/EnableAdmin"),
                    ("Tweak_UnlockRegion", "Снять региональные ограничения", "/UnlockRegion"),
                    ("Tweak_KillFreezeApps", "Принудительно завершать программы при зависании", "/KillFreezeApps"),
                    ("Tweak_DisableRemote", "Отключить Удаленный помощник", "/DisableRemote"),
                    ("Tweak_DisableStickyKeys", "Отключить залипание клавиш", "/DisableStickyKeys"),
                    ("Tweak_TTL", "Скрыть реальный TTL", "/TTL"),
                    ("Tweak_DisableNotificationsAds", "Отключить лишние уведомления и рекомендации", "/DisableNotificationsAds"),
                    ("Tweak_DNS", "Установить быстрый DNS на сетевые адаптеры", "/DNS")
                }),
                ("Cat_ContextMenu", "Контекстное меню", new[]
                {
                    ("Tweak_TakeOwnership", "Добавить «Стать владельцем» (Take Ownership) в меню ПКМ", "/TakeOwnership"),
                    ("Tweak_OpenAdminTerminal", "Добавить «Открыть Терминал от Администратора» в меню ПКМ", "/OpenAdminTerminal"),
                    ("Tweak_CopyPath", "Добавить «Копировать путь к файлу» в меню ПКМ", "/CopyPath"),
                    ("Tweak_ClassicContextMenu", "Классическое контекстное меню Windows 10 в Windows 11", "/ClassicContextMenu")
                }),
                ("Cat_Drivers", "Драйверы", new[]
                {
                    ("Tweak_InstallDrivers", "Установить драйверы (Папка Drivers на Рабочем столе)", "/InstallDrivers")
                }),
                ("Cat_OtherComponents", "Другие компоненты", new[]
                {
                    ("Tweak_InstallVC", "Установить Visual C++", "/InstallVC"),
                    ("Tweak_InstallDX", "Установить DirectX 9-11", "/InstallDX")
                }),
                ("Cat_VisualTweaks", "Визуальные твики", new[]
                {
                    ("Tweak_RemoveHome", "Удалить пункт Главная в Проводнике", "/RemoveHome"),
                    ("Tweak_RemoveGallery", "Удалить пункт Галерея в Проводнике", "/RemoveGallery"),
                    ("Tweak_RemoveNetwork", "Удалить пункт Сеть в Проводнике", "/RemoveNetwork"),
                    ("Tweak_DarkTheme", "Установить темную тему системы", "/DarkTheme"),
                    ("Tweak_SetWallpaper", "Установить кастомные обои", "/SetWallpaper"),
                    ("Tweak_BlueIcons", "Установить синие папки", "/BlueIcons"),
                    ("Tweak_Icaros", "Установить дополнительные эскизы медиафайлов (Icaros)", "/Icaros"),
                    ("Tweak_TraySeconds", "Установить секунды в трее", "/TraySeconds"),
                    ("Tweak_TrayDate", "Установить дату в трее", "/TrayDate"),
                    ("Tweak_TaskbarEndTask", "Установить пункт Завершить задачу на Панели задач", "/TaskbarEndTask"),
                    ("Tweak_RemoveTaskbarIcons", "Удалить лишние значки на Панели задач", "/RemoveTaskbarIcons"),
                    ("Tweak_HideRecommended", "Скрыть раздел Рекомендуем в меню Пуск", "/HideRecommended"),
                    ("Tweak_StartSettingsIcon", "Установить значок Настройки в меню Пуск", "/StartSettingsIcon"),
                    ("Tweak_WallpaperQuality", "Удалить сжатие обоев Рабочего стола", "/WallpaperQuality"),
                    ("Tweak_RemoveLockScreen", "Удалить экран блокировки", "/RemoveLockScreen"),
                    ("Tweak_NoIconShadow", "Удалить тени на значках Рабочего стола", "/NoIconShadow"),
                    ("Tweak_ExplorerThisPC", "Открывать Проводник в Этот компьютер", "/ExplorerThisPC"),
                    ("Tweak_ShowExtensions", "Показывать расширения файлов", "/ShowExtensions")
                })
            };

            foreach (var (catKey, catName, items) in tweaks)
            {
                var cat = new TweakCategory(catName, catKey);
                foreach (var (itemKey, title, tag) in items)
                {
                    var item = new TweakItem(title, tag, catName, itemKey);
                    item.PropertyChanged += Item_PropertyChanged;
                    cat.Items.Add(item);
                }
                Categories.Add(cat);
            }
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TweakItem.IsChecked))
            {
                if (sender is TweakItem item)
                {
                    if (item.Tag == "/RemoveAppx")
                    {
                        OnPropertyChanged(nameof(IsRemoveAllUwpChecked));
                        OnPropertyChanged(nameof(SelectiveUwpSummary));
                    }

                    if (item.IsChecked && !_isUpdatingCompression)
                    {
                        // Exclusive logic for CompressOS and CompressDrive
                        if (item.Tag == "/CompressOS")
                        {
                            _isUpdatingCompression = true;
                            var other = Categories.SelectMany(c => c.Items).FirstOrDefault(i => i.Tag == "/CompressDrive");
                            if (other != null) other.IsChecked = false;
                            _isUpdatingCompression = false;
                        }
                        else if (item.Tag == "/CompressDrive")
                        {
                            _isUpdatingCompression = true;
                            var other = Categories.SelectMany(c => c.Items).FirstOrDefault(i => i.Tag == "/CompressOS");
                            if (other != null) other.IsChecked = false;
                            _isUpdatingCompression = false;
                        }
                    }
                }

                OnPropertyChanged(nameof(CanOptimize));
            }
        }

        public void UpdateSelectiveUwpApps(List<UwpAppInfo> apps)
        {
            SelectedSelectiveUwpApps = apps;
            AllLoadedUwpApps = apps;
            OnPropertyChanged(nameof(SelectiveUwpSummary));
            OnPropertyChanged(nameof(CanOptimize));
        }

        public void UpdateDnsPreset(DnsPreset preset)
        {
            DnsManager.SelectedPreset = preset;
            OnPropertyChanged(nameof(SelectedDnsSummary));
        }

        public void ToggleSelectAll()
        {
            var allItems = Categories.SelectMany(c => c.Items).ToList();
            bool allSelected = allItems.All(i => i.IsChecked);
            bool targetState = !allSelected;

            foreach (var item in allItems)
            {
                if (targetState && (item.Tag == "/CompressDrive" || item.Tag == "/CompressOS"))
                {
                    item.IsChecked = false;
                }
                else
                {
                    item.IsChecked = targetState;
                }
            }
            OnPropertyChanged(nameof(CanOptimize));
            OnPropertyChanged(nameof(IsRemoveAllUwpChecked));
        }

        public async Task RunOptimizationAsync(Action onCompleted)
        {
            var selectedItems = Categories.SelectMany(c => c.Items).Where(i => i.IsChecked).ToList();
            var selectiveUwp = !IsRemoveAllUwpChecked ? SelectedSelectiveUwpApps.Where(a => a.IsSelected).ToList() : new List<UwpAppInfo>();

            int totalOperations = selectedItems.Count + (selectiveUwp.Count > 0 ? selectiveUwp.Count : 0);
            if (totalOperations == 0) return;

            IsOptimizing = true;
            ProgressPercentage = 0;

            await Task.Run(() =>
            {
                int currentOp = 0;

                // 1. Run main tweaks
                for (int i = 0; i < selectedItems.Count; i++)
                {
                    var item = selectedItems[i];
                    currentOp++;
                    double pct = (double)currentOp / totalOperations * 100.0;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ProgressPercentage = pct;
                        StatusMessage = $"[{currentOp}/{totalOperations}] {item.Title}";
                    });

                    TweakExecutor.ExecuteTweak(item.Tag, status =>
                    {
                        Application.Current.Dispatcher.Invoke(() => StatusMessage = status);
                    });
                }

                // 2. If RemoveAppx wasn't selected, but individual UWP apps were selected:
                if (!IsRemoveAllUwpChecked && selectiveUwp.Count > 0)
                {
                    for (int i = 0; i < selectiveUwp.Count; i++)
                    {
                        var app = selectiveUwp[i];
                        currentOp++;
                        double pct = (double)currentOp / totalOperations * 100.0;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ProgressPercentage = pct;
                            StatusMessage = $"[{currentOp}/{totalOperations}] Удаление UWP: {app.DisplayName}";
                        });

                        UwpManager.RemoveSelectedApps(new[] { app }, status =>
                        {
                            Application.Current.Dispatcher.Invoke(() => StatusMessage = status);
                        });
                    }
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    ProgressPercentage = 100;
                    StatusMessage = "Настройка завершена!";
                });
            });

            IsOptimizing = false;
            onCompleted?.Invoke();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
