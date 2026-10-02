using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WinClickWpf.Models
{
    public class JunkwareItem : INotifyPropertyChanged
    {
        private bool _isSelected = true;
        private string _status = "Обнаружено";
        private string _statusColor = "#EBCB8B";

        public string Name { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string UninstallString { get; set; } = string.Empty;
        public string? QuietUninstallString { get; set; }
        public string RiskLevel { get; set; } = "High"; // High, Medium, Low
        public string Reason { get; set; } = string.Empty;
        public string RegistryPath { get; set; } = string.Empty;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged();
                }
            }
        }

        public string StatusColor
        {
            get => _statusColor;
            set
            {
                if (_statusColor != value)
                {
                    _statusColor = value;
                    OnPropertyChanged();
                }
            }
        }

        public string RiskBadgeColor => RiskLevel switch
        {
            "High" => "#BF616A",    // Red
            "Medium" => "#D08770",  // Orange
            _ => "#EBCB8B"          // Yellow
        };

        public string RiskBadgeText => RiskLevel switch
        {
            "High" => "Критический мусор",
            "Medium" => "Реклама / Bloatware",
            _ => "Сомнительный софт"
        };

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
