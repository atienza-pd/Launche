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

        devAppsWindowView.Path = openFolderDialog.FileName;
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(devAppsWindowView.Path);
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await devAppsWindowView.LoadDevAppsAsync();
    }
}

public class RelayCommand<T>(Action<T> execute, Func<T, bool>? canExecute = null) : ICommand
{
    private readonly Action<T> _execute = execute;
    private readonly Func<T, bool>? _canExecute = canExecute;

    public bool CanExecute(object parameter) => _canExecute == null || _canExecute((T)parameter);

    public void Execute(object parameter) => _execute((T)parameter);

    public event EventHandler? CanExecuteChanged;
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
