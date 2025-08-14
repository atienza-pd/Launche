using ApplicationCore.Common;
using ApplicationCore.Features.DevApps;
using ApplicationCore.Features.Projects;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using UI.Features;
using UI.Shared;

namespace UI.MainWindows
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ICommand MoveUpCommand { get; }
        public ICommand MoveDownCommand { get; }
        public ICommand ListItemClickCommand { get; }
        public ICommand OpenFolderWindowCommand { get; }
        public ICommand SearchEnterCommand { get; }
        public ICommand SearchDownCommand { get; }
        public RelayCommand OpenDevAppsWindowCommand { get; }
        public RelayCommand OpenProjectsWindowCommand { get; }

        // Notify the View to focus the ListView
        public event EventHandler? RequestFocusListView;

        private readonly IProjectService projectService;
        private readonly INotificationMessageService notificationMessageService;
        private readonly IServiceProvider serviceProvider;
        private ObservableCollection<ProjectViewModel> _projects;
        private string _search;
        private ProjectViewModel project;


        public ObservableCollection<ProjectViewModel> Projects
        {
            get { return _projects; }
            set
            {
                _projects = value;
                OnPropertyChanged(nameof(this.Projects));
            }
        }

        public ProjectViewModel? Project
        {
            get { return project; }
            set
            {
                project = value!;

                OnPropertyChanged(nameof(this.Project));

            }
        }

        public ProjectViewModel? SelectedProject
        {
            get { return project; }
            set
            {
                project = value!.Copy();

                OnPropertyChanged(nameof(this.Project));
                this.GetProject();
            }
        }

        public MainWindowViewModel(IProjectService projectService, INotificationMessageService notificationMessageService, IServiceProvider serviceProvider)
        {
            // initialize non-nullable fields
            _projects = [];
            project = new ProjectViewModel();
            _search = string.Empty;

            this.projectService = projectService;
            this.notificationMessageService = notificationMessageService;
            this.serviceProvider = serviceProvider;
            this.MoveUpCommand = new RelayCommand(MoveUpAsync);
            this.MoveDownCommand = new RelayCommand(MoveDownAsync);
            this.ListItemClickCommand = new RelayCommand(OnListItemClick);
            this.OpenFolderWindowCommand = new RelayCommand(OpenFolderWindow);
            this.SearchEnterCommand = new RelayCommand(OnSearchEnter);
            this.SearchDownCommand = new RelayCommand(OnSearchDown);
            this.OpenDevAppsWindowCommand = new RelayCommand(OpenDevAppWindow);
            this.OpenProjectsWindowCommand = new RelayCommand(OpenProjectsWindow);
        }

        private void OpenProjectsWindow()
        {
            var mainWindow = serviceProvider.GetService<ProjectsWindow>();

            mainWindow!.ShowDialog();
        }

        private void OpenDevAppWindow()
        {
            var mainWindow = serviceProvider.GetService<DevAppsWindow>();

            mainWindow!.ShowDialog();
        }

        private void OnListItemClick()
        {
            OpenProjectDevApp();
        }

        private void OnSearchEnter()
        {
            // If a project is selected, open it.
            if (project != null)
            {
                OpenProjectDevApp();
            }
        }

        private void OnSearchDown()
        {
            // Select the first item after search so user can continue with arrows/enter.
            if (Projects?.Count > 0)
            {
                SelectedProject = Projects[0];
            }

            // Ask the view to focus the ListView (can't set focus from VM)
            RequestFocusListView?.Invoke(this, EventArgs.Empty);
        }

        private void OpenFolderWindow()
        {
            try
            {
                if (!Directory.Exists(project.Path))
                {
                    this.notificationMessageService.Create(
                        "Directory not found!",
                        "Open Project",
                        NotificationType.Error
                    );
                    return;
                }
                ProcessStartInfo startInfo =
                    new()
                    {
                        FileName = "explorer.exe",
                        Arguments = project.Path,
                        UseShellExecute = true,
                    };

                using Process process = new();
                process.StartInfo = startInfo;
                process.Start();
            }
            catch (Exception ex)
            {
                notificationMessageService.Create(ex.Message, "Open Project", NotificationType.Error);
            }
        }

        private async void MoveDownAsync()
        {
            var result = await projectService.SortDown(project.SortId);
            
            if (!result)
            {
                return;
            }

            SearchProjects(this.Search);
        }

        private async void MoveUpAsync()
        {
            var result = await projectService.SortUp(project.SortId);
            if (!result)
            {
                return;
            }

            SearchProjects(this.Search);
        }

        public void OpenProjectDevApp()
        {
            if (project is null)
            {
                
                return;
            }

            try
            {
                if (!Directory.Exists(project.Path))
                {
                    this.notificationMessageService.Create(
                        "Directory not found!",
                        "Launch IDE Error",
                        NotificationType.Error
                    );
                    return;
                }

                if (project.HasFileName)
                {
                    this.OpenIDEWithFileName(project.FullPath, project.DevAppPath);
                    return;
                }

                OpenIDE(
                    new()
                    {
                        FileName = project.DevAppPath,
                        Arguments = project.FullPath,
                        UseShellExecute = true,
                    }
                );
            }
            catch (Exception ex)
            {
                notificationMessageService.Create(
                    ex.Message,
                    "Open Project Dev App",
                    NotificationType.Error
                );
            }
        }

        private void OpenIDEWithFileName(string fullFilePath, string devAppPath)
        {
            if (File.Exists(fullFilePath) is false)
            {
                notificationMessageService.Create(
                    "File not found!",
                    "Open Project Error",
                    NotificationType.Error
                );
                return;
            }

            OpenIDE(
                new()
                {
                    FileName = devAppPath,
                    Arguments = $"{fullFilePath}",
                    UseShellExecute = true,
                }
            );
        }

        private static void OpenIDE(ProcessStartInfo processInfo)
        {
            using Process process = new();
            process.StartInfo = processInfo;
            process.Start();
        }

        private async void GetProject()
        {

            this.Project = await projectService.GetOne(SelectedProject!.Id);
        }

        public string Search
        {
            get { return _search; }
            set
            {
                _search = value;
                OnPropertyChanged(nameof(this.Search));
                SearchProjects(_search);
            }
        }

        private async void SearchProjects(string search)
        {
            var result = await projectService.GetAll();
            this.Projects = [.. result.Projects.Where(x => x.FullName.Contains(search, StringComparison.CurrentCultureIgnoreCase))];
        }

        public void LoadProjects()
        {
            SearchProjects("");
        }
    }
}
