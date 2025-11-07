using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ApplicationCore.Common;
using ApplicationCore.Features.DevApps;
using ApplicationCore.Features.Projects;
using Microsoft.Win32;
using UI.Shared;
using UI.Shared.Services;

namespace UI.Features.Projects;

public class ProjectsWindowViewModel : ViewModelBase
{
    public event EventHandler? CloseWindowEvent;

    public ICommand DeleteCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand AddNewCommand { get; }
    public ICommand OpenDialogCommand { get; }

    public ObservableCollection<DevAppViewModel> DevApps
    {
        get { return devApps; }
        set
        {
            devApps = value;
            OnPropertyChanged(nameof(this.DevApps));
        }
    }

    public ProjectViewModel? Project
    {
        get { return project; }
        set
        {
            project = value!.Copy();

            OnPropertyChanged(nameof(this.Project));
            SelectedDevApp = DevApps.FirstOrDefault(x => x.Id == project!.IDEPathId);
        }
    }

    public DevAppViewModel? SelectedDevApp
    {
        get { return devApp; }
        set
        {
            devApp = value;

            OnPropertyChanged(nameof(this.SelectedDevApp));
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

    public Visibility DeleteButtonVisibility
    {
        get { return deleteButtonVisibility; }
        set
        {
            deleteButtonVisibility = value;
            OnPropertyChanged(nameof(this.DeleteButtonVisibility));
        }
    }

    private ProjectViewModel? project = new();
    private Visibility saveNotificationVisibility = Visibility.Hidden;
    private readonly IProjectService projectService;
    private readonly INotificationMessageService notificationMessageService;
    private readonly IProjectWindowEventsService projectWindowEventsService;
    private readonly IDevAppService devAppService;
    private readonly ISelectedProjectService selectedProjectService;
    private ObservableCollection<DevAppViewModel> devApps = [];
    private DevAppViewModel? devApp;
    private Visibility deleteButtonVisibility;

    public ProjectsWindowViewModel(
        IProjectService projectService,
        INotificationMessageService notificationMessageService,
        IProjectWindowEventsService projectWindowEventsService,
        IDevAppService devAppService,
        ISelectedProjectService selectedProjectService
    )
    {
        DeleteCommand = new RelayCommand(DeleteItem);
        SaveCommand = new RelayCommand(SaveAsync);
        AddNewCommand = new RelayCommand(AddNew);
        OpenDialogCommand = new RelayCommand(OpenDialog);
        this.projectService = projectService;
        this.notificationMessageService = notificationMessageService;
        this.projectWindowEventsService = projectWindowEventsService;
        this.devAppService = devAppService;
        this.selectedProjectService = selectedProjectService;
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
        string name = string.IsNullOrEmpty(Project!.Name)
            ? openFolderDialog.SafeFolderName
            : Project.Name;

        Project = new()
        {
            Id = Project?.Id ?? 0,
            Path = filePath,
            Name = name!,
            IDEPathId = SelectedDevApp?.Id,
        };
    }

    private void AddNew()
    {
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

            if (this.Project!.Id == 0)
            {
                await this.projectService.Add(
                    new()
                    {
                        Name = Project.Name,
                        Path = Project.Path,
                        IDEPathId = SelectedDevApp!.Id,
                        Filename = Project.Filename,
                    }
                );

                SaveNotificationVisibility = Visibility.Visible;
            }
            else
            {
                await this.projectService.Edit(
                    new()
                    {
                        Id = Project.Id,
                        Name = Project.Name,
                        Path = Project.Path,
                        IDEPathId = SelectedDevApp!.Id,
                        Filename = Project.Filename,
                    }
                );

                SaveNotificationVisibility = Visibility.Visible;
            }

            var projects = await projectService.GetAll();
            this.Project = projects.Projects.FirstOrDefault(p => p.Name == Project.Name);

            if (this.Project != null)
            {
                this.DeleteButtonVisibility = Visibility.Visible;
            }

            this.projectWindowEventsService.ProjectsChanged();

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
            this.notificationMessageService.Create(
                "Invalid Project data.",
                "Save Project",
                NotificationType.Error
            );

            return false;
        }

        if (String.IsNullOrEmpty(Project.Name) || String.IsNullOrWhiteSpace(Project.Name))
        {
            this.notificationMessageService.Create(
                "Name is required!",
                "Save Project",
                NotificationType.Error
            );

            return false;
        }

        if (String.IsNullOrEmpty(Project.Path) || String.IsNullOrWhiteSpace(Project.Path))
        {
            this.notificationMessageService.Create(
                "Path is required!",
                "Save Project",
                NotificationType.Error
            );

            return false;
        }

        return true;
    }

    private async void DeleteItem()
    {
        try
        {
            var projects = await projectService.GetAll();

            if (projects.Count == 1)
            {
                this.notificationMessageService.Create(
                    "Single project should exists!",
                    "Delete Project",
                    NotificationType.Error
                );

                return;
            }

            var result = await this.projectService.Delete(id: project!.Id);

            if (result)
            {
                Project = new();
                projectWindowEventsService.ProjectsChanged();
                this.CloseWindowEvent?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (Exception ex)
        {
            this.notificationMessageService.Create(
                ex.Message,
                "Delete Project",
                NotificationType.Error
            );
        }
    }

    public async Task LoadDevApps()
    {
        this.DevApps = [.. await devAppService.GetAll()];
    }

    public void SetSelectedProject()
    {
        var selecteProject = this.selectedProjectService.GetSelectedProject();

        if (selecteProject is null || selecteProject?.Id is 0)
        {
            DeleteButtonVisibility = Visibility.Hidden;
            return;
        }

        DeleteButtonVisibility = Visibility.Visible;
        this.Project = selecteProject;
    }
}
