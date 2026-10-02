using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WinClickWpf.Models
{
    public class SoftwareApp : INotifyPropertyChanged
    {
        private bool _isSelected;
        private string _status = "Ожидание";
        private string _statusColor = "#888888";

        public string Name { get; set; } = string.Empty;
        public string WingetId { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? DirectUrl { get; set; }
        public string? SilentArgs { get; set; }
        public bool IsRecommended { get; set; }

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

        public SoftwareApp() { }

        public SoftwareApp(string name, string wingetId, string category, string description = "", bool isRecommended = false, string? directUrl = null, string? silentArgs = null)
        {
            Name = name;
            WingetId = wingetId;
            Category = category;
            Description = description;
            IsRecommended = isRecommended;
            DirectUrl = directUrl;
            SilentArgs = silentArgs;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
