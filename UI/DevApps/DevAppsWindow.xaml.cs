using System.Windows;
using System.Windows.Controls;
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

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await devAppsWindowView.LoadDevAppsAsync();
    }

    private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (sender is ListView listView && listView.View is GridView gridView)
        {
            // Get the total width of the ListView
            double totalWidth = listView.ActualWidth;

            // Subtract some space for padding, scrollbars, etc.
            double availableWidth = totalWidth - 10; // Adjust as needed

            // Divide the available width between columns
            if (gridView.Columns.Count == 2) // Assuming 2 columns: Name and Actions
            {
                gridView.Columns[0].Width = availableWidth * 0.8; // 80% for Name column
                gridView.Columns[1].Width = availableWidth * 0.2; // 30% for Actions column
            }
        }
    }

}
