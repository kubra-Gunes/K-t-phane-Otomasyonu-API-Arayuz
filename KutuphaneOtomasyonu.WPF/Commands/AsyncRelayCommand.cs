// KutuphaneOtomasyonu.WPF.Commands/AsyncRelayCommand.cs
using System;
using System.Threading.Tasks;
using System.Windows.Input;

using KutuphaneOtomasyonu.WPF.Helpers;

namespace KutuphaneOtomasyonu.WPF.Commands
{
    public class AsyncRelayCommand : ICommand, IRaiseCanExecuteChanged
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;
        private bool _isExecuting;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return !_isExecuting && (_canExecute == null || _canExecute());
        }

        public async void Execute(object parameter)
        {
            _isExecuting = true;
            RaiseCanExecuteChanged(); // CanExecute durumunu güncelle
            try
            {
                await _execute();
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged(); // CanExecute durumunu güncelle
            }
        }

        // Yeni eklenen metod
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public async Task ExecuteAsync(object parameter) // Added for direct async execution
        {
            if (CanExecute(parameter))
            {
                _isExecuting = true;
                RaiseCanExecuteChanged(); // CanExecute durumunu güncelle
                try
                {
                    await _execute();
                }
                finally
                {
                    _isExecuting = false;
                    RaiseCanExecuteChanged(); // CanExecute durumunu güncelle
                }
            }
        }
    }
    // AsyncRelayCommand<T> sınıfı da benzer şekilde güncellenebilir
    public class AsyncRelayCommand<T> : ICommand, IRaiseCanExecuteChanged

    {
        private readonly Func<T, Task> _execute;
        private readonly Predicate<T> _canExecute;
        private bool _isExecuting;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public AsyncRelayCommand(Func<T, Task> execute, Predicate<T> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return !_isExecuting && (_canExecute == null || _canExecute((T)parameter));
        }

        public async void Execute(object parameter)
        {
            _isExecuting = true;
            RaiseCanExecuteChanged(); // CanExecute durumunu güncelle
            try
            {
                await _execute((T)parameter);
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged(); // CanExecute durumunu güncelle
            }
        }

        // Yeni eklenen metod
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}