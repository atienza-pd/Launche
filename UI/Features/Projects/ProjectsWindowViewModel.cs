﻿

using ApplicationCore.Common;
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

    private void OnPropertyChanged(string v)
    {
        throw new NotImplementedException();
    }

    private string _search;
    private ObservableCollection<ProjectViewModel> projects;
    private ProjectViewModel? project;
    private Visibility saveNotificationVisibility = Visibility.Hidden;
    private readonly IGetAllProjectService getAllProjectService;
    private readonly INotificationMessageService notificationMessageService;
    private readonly IEditProjectService editProjectService;
    private readonly IProjectWindowEventsService projectWindowEventsService;
    private readonly IAddProjectService addProjectService;

    public ProjectsWindowViewModel(IGetAllProjectService getAllProjectService,
        INotificationMessageService notificationMessageService,
        IEditProjectService editProjectService,
        IProjectWindowEventsService projectWindowEventsService,
        IAddProjectService addProjectService)
    {
        DeleteCommand = new RelayCommand(param => DeleteItem((ProjectViewModel)param!));
        SaveCommand = new RelayCommand(SaveDevAppsAsync);
        AddNewCommand = new RelayCommand(AddNew);
        OpenDialogCommand = new RelayCommand(OpenDialog);
        this.getAllProjectService = getAllProjectService;
        this.notificationMessageService = notificationMessageService;
        this.editProjectService = editProjectService;
        this.projectWindowEventsService = projectWindowEventsService;
        this.addProjectService = addProjectService;
    }

    private void OpenDialog()
    {
        var openFolderDialog = new OpenFileDialog { Filter = "Executable Files | *.exe" };
        var result = openFolderDialog.ShowDialog() ?? false;

        if (!result)
        {
            return;
        }
        Project = new() { Id = Project?.Id ?? 0, Path = openFolderDialog.FileName, Name = Project?.Name! };
    }

    private void AddNew()
    {
        SearchProjects("");
        this.Project = new();
    }

    private async void SaveDevAppsAsync()
    {
        try
        {
            if (Project == null)
            {
                this.notificationMessageService.Create("Invalid Project data.",
                    "Save Project",
                    NotificationType.Error);

                return;
            }

            if (String.IsNullOrEmpty(Project.Name) || String.IsNullOrWhiteSpace(Project.Name))
            {
                this.notificationMessageService.Create("Name is required!",
                    "Save Project",
                    NotificationType.Error);

                return;
            }

            if (String.IsNullOrEmpty(Project.Path) || String.IsNullOrWhiteSpace(Project.Path))
            {
                this.notificationMessageService.Create("Path is required!",
                    "Save Project",
                    NotificationType.Error);

                return;
            }

            if (this.Project.Id == 0)
            {
                await this.addProjectService.HandleAsync(new(Name: Project.Name, Path: Project.Path, IDEPathId: null, FileName: Project.Filename));

                SaveNotificationVisibility = Visibility.Visible;
            }
            else
            {
                await this.editProjectService.HandleAsync(new(Id: Project.Id, Name: Project.Name,
                    Path: Project.Path,
                    IDEPathId: Project.IDEPathId,
                    FileName: Project.Filename)
                    );

                SaveNotificationVisibility = Visibility.Visible;
            }

            this.projectWindowEventsService.ProjectsChanged();
            long id = this.Project.Id;

            var result = await getAllProjectService.HandleAsync();
            this.Projects = [.. result.Projects.Where(x => x.Name.Contains(Search ?? "", StringComparison.CurrentCultureIgnoreCase))];

            this.Project = Projects.FirstOrDefault(x => x.Id == id) ?? new();

            await Task.Delay(3000);
            SaveNotificationVisibility = Visibility.Hidden;

        }
        catch (Exception ex)
        {
            this.notificationMessageService.Create(ex.Message,
                    "Save Dev App",
                    NotificationType.Error);
        }

    }

    private void DeleteItem(ProjectViewModel devAppViewModel)
    {
        throw new NotImplementedException();
    }

    private async void SearchProjects(string search)
    {
        var result = await getAllProjectService.HandleAsync();
        this.Projects = [.. result.Projects.Where(x => x.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase))];
    }

    public void LoadProjects()
    {
        this.SearchProjects("");
    }
}
