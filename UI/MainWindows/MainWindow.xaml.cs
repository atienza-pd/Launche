using ApplicationCore.Features.Groups;
using ApplicationCore.Features.Projects;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UI.DevApps;
using UI.Features;
using UI.Features.Projects;
using UI.Windows.Group;

namespace UI.MainWindows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainWindowViewModel mainWindowViewModel;

    private readonly IDevAppsEventsService devAppsEventsService;
    private readonly IProjectWindowEventsService projectWindowEventsService;

    public MainWindow(
        MainWindowViewModel mainWindowViewModel,
        IDevAppsEventsService devAppsEventsService,
        IProjectWindowEventsService projectWindowEventsService
    )
    {
        InitializeComponent();

        DataContext = mainWindowViewModel;
        this.mainWindowViewModel = mainWindowViewModel;

        this.devAppsEventsService = devAppsEventsService;
        this.projectWindowEventsService = projectWindowEventsService;

        // Subscribe to VM request to focus list view
        this.mainWindowViewModel.RequestFocusListView += (_, __) => FocusOnListViewWhenArrowDown();
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        this.devAppsEventsService.OnDevAppsChanged += DevAppsEventsService_OnDevAppsChanged;
        this.projectWindowEventsService.OnProjectsChanged += ProjectWindowEventsService_OnProjectsChanged;
        this.mainWindowViewModel.LoadProjects();
        await this.mainWindowViewModel.LoadDevApps();

    }

    private void ComboBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (sender is ComboBox comboBox)
        {
            comboBox.IsDropDownOpen = true;
        }
    }

    private void DevAppsEventsService_OnDevAppsChanged(object? sender, EventArgs e)
    {
        this.mainWindowViewModel.LoadProjects();
    }

    private void ProjectWindowEventsService_OnProjectsChanged(object? sender, EventArgs e)
    {
        this.mainWindowViewModel.LoadProjects();
    }

    private void FocusOnListViewWhenArrowDown()
    {
        if (lvProjectPaths.Items.Count == 0)
        {
            return;
        }

        lvProjectPaths.Focus();
        var item = lvProjectPaths.Items[0];
        lvProjectPaths.SelectedItem = item;
        ListViewItem? firstListViewItem =
            lvProjectPaths.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;
        firstListViewItem?.Focus();
    }

    public void SelectNewlyAddedItem()
    {

        lvProjectPaths.SelectedItem = lvProjectPaths.Items[^1];
        lvProjectPaths.ScrollIntoView(this.lvProjectPaths.SelectedItem);
    }

    //public void SelectEditedItem()
    //{
    //    lvProjectPaths.SelectedItem = lvProjectPaths
    //        .Items.SourceCollection.Cast<ProjectViewModel>()
    //        .FirstOrDefault(projectPathsViewModel =>
    //            projectPathsViewModel.Id == MainWindowViewModel?.SelectedProjectPath?.Id
    //        );

    //    lvProjectPaths.ScrollIntoView(this.lvProjectPaths.SelectedItem);
    //}
}
