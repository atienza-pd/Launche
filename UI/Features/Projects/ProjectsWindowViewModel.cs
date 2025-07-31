

using ApplicationCore.Common;
using ApplicationCore.Features.DevApps;
using ApplicationCore.Features.Projects;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using UI.Shared;

namespace UI.Features.Projects;

public class ProjectsWindowViewModel : ViewModelBase
{
    public ICommand DeleteCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand AddNewCommand { get; }
    public ICommand OpenDialogCommand { get; }

    public ObservableCollection<ProjectViewModel> Projects
    {
        get { return projects; }
        set
        {
            projects = value;
            OnPropertyChanged(nameof(this.Projects));
        }
    }

    public ProjectViewModel? Project
    {
        get { return project; }
        set
        {
            project = value.Copy();

            OnPropertyChanged(nameof(this.Project));
        }
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

    public Visibility SaveNotificationVisibility
    {
        get { return saveNotificationVisibility; }
        set
        {
            saveNotificationVisibility = value;
            OnPropertyChanged(nameof(this.SaveNotificationVisibility));
        }
    }


    private string _search;
    private ObservableCollection<ProjectViewModel> projects;
    private ProjectViewModel? project = new();
    private Visibility saveNotificationVisibility = Visibility.Hidden;
    private readonly IProjectService projectService;
    private readonly INotificationMessageService notificationMessageService;
    private readonly IProjectWindowEventsService projectWindowEventsService;

    public ProjectsWindowViewModel(IProjectService projectService,
        INotificationMessageService notificationMessageService,
        IProjectWindowEventsService projectWindowEventsService
        )
    {
        DeleteCommand = new RelayCommand(param => DeleteItem((ProjectViewModel)param!));
        SaveCommand = new RelayCommand(SaveAsync);
        AddNewCommand = new RelayCommand(AddNew);
        OpenDialogCommand = new RelayCommand(OpenDialog);
        this.projectService = projectService;
        this.notificationMessageService = notificationMessageService;
        this.projectWindowEventsService = projectWindowEventsService;
    }

    private void OpenDialog()
    {
        var openFolderDialog = new OpenFolderDialog();
        var result = openFolderDialog.ShowDialog() ?? false;

        if (!result)
        {
            return;
        }

        string filePath = openFolderDialog.FolderName;
        string name = string.IsNullOrEmpty(Project.Name) ? openFolderDialog.SafeFolderName : Project.Name;

        Project = new() { Id = Project?.Id ?? 0, Path = filePath, Name = name! };
    }

    private void AddNew()
    {
        SearchProjects("");
        this.Project = new();
    }

    private async void SaveAsync()
    {
        try
        {
            if (!ValidateFields())
            {
                return;
            }

            if (this.Project.Id == 0)
            {
                await this.projectService.Add(new() { Name = Project.Name, Path = Project.Path, IDEPathId = null, Filename = Project.Filename });

                SaveNotificationVisibility = Visibility.Visible;
            }
            else
            {
                await this.projectService.Edit(new()
                {
                    Id = Project.Id,
                    Name = Project.Name,
                    Path = Project.Path,
                    IDEPathId = Project.IDEPathId,
                    Filename = Project.Filename
                }
                    );

                SaveNotificationVisibility = Visibility.Visible;
            }

            this.projectWindowEventsService.ProjectsChanged();
            long id = this.Project.Id;

            var result = await projectService.GetAll();
            this.Projects = [.. result.Projects.Where(x => x.Name.Contains(Search ?? "", StringComparison.CurrentCultureIgnoreCase))];

            this.Project = Projects.FirstOrDefault(x => x.Id == id) ?? new();

            await Task.Delay(3000);
            SaveNotificationVisibility = Visibility.Hidden;

        }
        catch (Exception ex)
        {
            this.notificationMessageService.Create(
                ex.Message,
                "Save Dev App",
                NotificationType.Error
            );
        }

    }

    private bool ValidateFields()
    {
        if (Project == null)
        {
            this.notificationMessageService.Create("Invalid Project data.",
                "Save Project",
                NotificationType.Error);

            return false;
        }

        if (String.IsNullOrEmpty(Project.Name) || String.IsNullOrWhiteSpace(Project.Name))
        {
            this.notificationMessageService.Create("Name is required!",
                "Save Project",
                NotificationType.Error);

            return false;
        }

        if (String.IsNullOrEmpty(Project.Path) || String.IsNullOrWhiteSpace(Project.Path))
        {
            this.notificationMessageService.Create("Path is required!",
                "Save Project",
                NotificationType.Error);

            return false;
        }

        return true;
    }

    private async void DeleteItem(ProjectViewModel item)
    {
        try
        {
            if (Projects.Count == 1)
            {
                this.notificationMessageService.Create("Single project should exists!",
                   "Delete Project",
                   NotificationType.Error);

                return;
            }

            if (item is null)
            {
                this.notificationMessageService.Create("No selected Project to be deleted!",
                   "Delete Project",
                   NotificationType.Error);

                return;
            }

            var result = await this.projectService.Delete(id: item.Id);

            if (result)
            {
                Projects.Remove(item);
                Project = new();
                projectWindowEventsService.ProjectsChanged();
            }
        }
        catch (Exception ex)
        {

            this.notificationMessageService.Create(ex.Message,
                    "Delete Project",
                    NotificationType.Error);
        }
    }

    private async void SearchProjects(string search)
    {
        var result = await projectService.GetAll();
        this.Projects = [.. result.Projects.Where(x => x.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase))];
    }

    public void LoadProjects()
    {
        this.SearchProjects("");
    }
}
