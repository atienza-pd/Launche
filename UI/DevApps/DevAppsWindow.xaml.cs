using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;
using UI.DevApps;

namespace UI;

/// <summary>
/// Interaction logic for DevAppsWindow.xaml
/// </summary>
public partial class DevAppsWindow : Window
{
    private readonly DevAppsWindowViewModel devAppsWindowView;

    public DevAppsWindow(DevAppsWindowViewModel devAppsWindowView)
    {
        InitializeComponent();

        DataContext = devAppsWindowView;
        this.devAppsWindowView = devAppsWindowView;
    }

    private void VsCodePathOpenDialogButton_Click(object sender, RoutedEventArgs e)
    {
        var openFolderDialog = new OpenFileDialog { Filter = "Executable Files | *.exe" };
        var result = openFolderDialog.ShowDialog() ?? false;

        if (!result)
        {
            return;
        }

        devAppsWindowView.DevApp = new() { Id = devAppsWindowView.DevApp?.Id ?? 0, Path = openFolderDialog.FileName };
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {

    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await devAppsWindowView.LoadDevAppsAsync();
    }
}

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
