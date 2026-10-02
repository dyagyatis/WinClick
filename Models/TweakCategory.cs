using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WinClickWpf.Services;

namespace WinClickWpf.Models
{
    public class TweakCategory : INotifyPropertyChanged
    {
        private string _name = string.Empty;

        public string Key { get; set; } = string.Empty;

        public string Name
        {
            get => !string.IsNullOrEmpty(Key) ? LocalizationService.GetString(Key) : _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<TweakItem> Items { get; set; } = new();

        public TweakCategory() { }

        public TweakCategory(string name, string key = "")
        {
            _name = name;
            Key = key;
        }

        public void RefreshLocalization()
        {
            OnPropertyChanged(nameof(Name));
            foreach (var item in Items)
            {
                item.RefreshLocalization();
            }
        }

        public void ToggleAll()
        {
            bool allSelected = Items.Count > 0 && Items.All(x => x.IsChecked);
            bool targetState = !allSelected;
            foreach (var item in Items)
            {
                item.IsChecked = targetState;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
