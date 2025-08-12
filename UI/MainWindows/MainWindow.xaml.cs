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

    private GroupModalWindow? groupModalWindow;
    private readonly List<Group> groups = [];
    private readonly MainWindowViewModel mainWindowViewModel;
    private readonly IServiceProvider serviceProvider;
    private readonly IDevAppsEventsService devAppsEventsService;
    private readonly IProjectWindowEventsService projectWindowEventsService;

    public string DevAppFilePath { get; set; } = "";

    public ListView ProjectPathsListView => this.lvProjectPaths;



    public ProjectViewModel SelectedProject => (ProjectViewModel)lvProjectPaths.SelectedItem;

    public MainWindow() => InitializeComponent();

    public MainWindow(
        MainWindowViewModel mainWindowViewModel,
        IServiceProvider serviceProvider,
        IDevAppsEventsService devAppsEventsService,
        IProjectWindowEventsService projectWindowEventsService
    )
    {
        InitializeComponent();

        DataContext = mainWindowViewModel;
        this.mainWindowViewModel = mainWindowViewModel;
        this.serviceProvider = serviceProvider;
        this.devAppsEventsService = devAppsEventsService;
        this.projectWindowEventsService = projectWindowEventsService;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        this.devAppsEventsService.OnDevAppsChanged += DevAppsEventsService_OnDevAppsChanged;
        this.projectWindowEventsService.OnProjectsChanged += ProjectWindowEventsService_OnProjectsChanged;
        this.mainWindowViewModel.LoadProjects();

    }

    private void DevAppsEventsService_OnDevAppsChanged(object? sender, EventArgs e)
    {
        this.mainWindowViewModel.LoadProjects();
    }

    private void ProjectWindowEventsService_OnProjectsChanged(object? sender, EventArgs e)
    {
        this.mainWindowViewModel.LoadProjects();
    }

    public void FocusOnListViewWhenArrowDown()
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


    private void ProjectPathsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        //if (lvProjectPaths.SelectedIndex == -1)
        //{
        //    return;
        //}

        //SelectProjectEvent.Invoke(this, EventArgs.Empty);
    }

    

    private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private void MnuOpenFolderWindow_Click(object sender, RoutedEventArgs e)
    {

    }

    private void txtSearch_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Down)
        {
            return;
        }


    }

    private void lvProjectPaths_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }


    }

    private void mnuAddToGroup_Click(object sender, RoutedEventArgs e)
    {

    }

    private void MenuItem_ShowDevApps_Click(object sender, RoutedEventArgs e)
    {
        var mainWindow = serviceProvider.GetService<DevAppsWindow>();

        mainWindow!.ShowDialog();
    }

    private void MenuItemManageProjects_Click(object sender, RoutedEventArgs e)
    {
        var mainWindow = serviceProvider.GetService<ProjectsWindow>();

        mainWindow!.ShowDialog();
    }
}
