using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WinClickWpf.Models
{
    public enum StartupCategoryType
    {
        Programs,
        Tasks,
        Services
    }

    public class StartupItem : INotifyPropertyChanged
    {
        private bool _isEnabled = true;
        private string _status = "Включено";

        public string Name { get; set; } = "";
        public string Command { get; set; } = "";
        public string Publisher { get; set; } = "Неизвестно";
        public string LocationType { get; set; } = "Реестр (HKCU)";
        public string RegistryPath { get; set; } = "";
        public string OriginalValue { get; set; } = "";
        public string CleanFilePath { get; set; } = "";
        public string TaskPath { get; set; } = "";
        public string ServiceName { get; set; } = "";
        public bool IsCurrentUser { get; set; } = true;
        public StartupCategoryType Category { get; set; } = StartupCategoryType.Programs;

        public string Impact { get; set; } = "Низкое";
        public string ImpactColor => Impact switch
        {
            "Высокое" => "#BF616A",
            "Среднее" => "#EBCB8B",
            _ => "#A3BE8C"
        };

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                Status = _isEnabled ? "Включено" : "Отключено";
                OnPropertyChanged();
                OnPropertyChanged(nameof(StatusColor));
            }
        }

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        public string StatusColor => IsEnabled ? "#A3BE8C" : "#BF616A";

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
