// KutuphaneOtomasyonu.WPF.ViewModels/BaseViewModel.cs
using System.ComponentModel;
using System.Runtime.CompilerServices; // CallerMemberName için

namespace KutuphaneOtomasyonu.WPF.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        // INotifyPropertyChanged arayüzünün gerektirdiği olay
        public event PropertyChangedEventHandler PropertyChanged;

        // Bir özelliğin değeri değiştiğinde bu metodu çağırırız
        // [CallerMemberName] özelliği, metodun çağrıldığı yerdeki özelliğin adını otomatik olarak alır.
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                }
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged(nameof(ErrorMessage));
                }
            }
        }
    }
}