using System.Windows.Input;

namespace UI.Shared
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute; // For parameterless actions
        private readonly Action<object?> _executeWithParameter; // For parameterized actions
        private readonly Func<bool>? _canExecute; // For parameterless CanExecute
        private readonly Func<object?, bool>? _canExecuteWithParameter; // For parameterized CanExecute

        public event EventHandler? CanExecuteChanged;

        // Constructor for parameterless commands
        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Constructor for parameterized commands
        public RelayCommand(Action<object?> executeWithParameter, Func<object?, bool>? canExecuteWithParameter = null)
        {
            _executeWithParameter = executeWithParameter ?? throw new ArgumentNullException(nameof(executeWithParameter));
            _canExecuteWithParameter = canExecuteWithParameter;
        }

        public bool CanExecute(object? parameter)
        {
            if (_canExecute != null)
            {
                return _canExecute();
            }

            if (_canExecuteWithParameter != null)
            {
                return _canExecuteWithParameter(parameter);
            }

            return true; // Default to true if no CanExecute logic is provided
        }

        public void Execute(object? parameter)
        {
            if (_execute != null && parameter == null)
            {
                _execute();
            }
            else
            {
                _executeWithParameter?.Invoke(parameter);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
