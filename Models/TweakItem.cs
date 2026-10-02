using System.ComponentModel;
using System.Runtime.CompilerServices;
using WinClickWpf.Services;

namespace WinClickWpf.Models
{
    public class TweakItem : INotifyPropertyChanged
    {
        private bool _isChecked;
        private string _title = string.Empty;

        public string Key { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        public string Title
        {
            get => !string.IsNullOrEmpty(Key) ? LocalizationService.GetString(Key) : _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged();
                }
            }
        }

        public TweakItem() { }

        public TweakItem(string title, string tag, string category = "", string key = "")
        {
            _title = title;
            Tag = tag;
            Category = category;
            Key = key;
        }

        public void RefreshLocalization()
        {
            OnPropertyChanged(nameof(Title));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
