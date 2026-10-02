using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WinClickWpf.Models
{
    public class CleanableCategory : INotifyPropertyChanged
    {
        private bool _isSelected = true;
        private long _bytes;
        private string _status = "";

        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string IconSvgPath { get; set; } = "";
        public List<string> TargetDirectories { get; set; } = new();

        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }

        public long Bytes
        {
            get => _bytes;
            set
            {
                _bytes = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FormattedSize));
            }
        }

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        public string FormattedSize
        {
            get
            {
                if (Bytes <= 0) return "0 МБ";
                if (Bytes < 1024 * 1024) return $"{Bytes / 1024.0:F1} КБ";
                if (Bytes < 1024L * 1024 * 1024) return $"{Bytes / (1024.0 * 1024):F1} МБ";
                return $"{Bytes / (1024.0 * 1024 * 1024):F2} ГБ";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
