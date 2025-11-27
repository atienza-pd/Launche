using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ApplicationCore.Common;
using ApplicationCore.Features.DevApps;
using ApplicationCore.Features.Projects;
using UI.Shared;
using UI.Shared.Services;

namespace UI.Features.LaunchWith;

public class LaunchWithWindowViewModel : ViewModelBase
{
    public ICommand LaunchProjectDevAppCommand { get; }
    public ICommand ListItemDoubleClickCommand { get; }

    private readonly IDevAppService devAppService;
    private readonly ISelectedProjectService selectedProjectService;
    private readonly INotificationMessageService notificationMessageService;

    private ObservableCollection<DevAppViewModel> devApps = [];
    private ProjectViewModel selectedProject = new();

    public ObservableCollection<DevAppViewModel> DevApps
    {
        get { return devApps; }
        set
        {
            devApps = value;
            OnPropertyChanged(nameof(this.DevApps));
        }
    }

    public ProjectViewModel SelectedProject
    {
        get { return selectedProject; }
        set
        {
            selectedProject = value;
            OnPropertyChanged(nameof(this.SelectedProject));
        }
    }

    public LaunchWithWindowViewModel(
        IDevAppService devAppService,
        ISelectedProjectService selectedProjectService,
        INotificationMessageService notificationMessageService
    )
    {
        this.devAppService = devAppService;
        this.selectedProjectService = selectedProjectService;
        this.notificationMessageService = notificationMessageService;
        this.ListItemDoubleClickCommand = new RelayCommand(param =>
            LaunchProjectDevApp((DevAppViewModel)param!)
        );

        this.LaunchProjectDevAppCommand = new RelayCommand(param =>
            LaunchProjectDevApp((DevAppViewModel)param!)
        );
    }

    private void LaunchProjectDevApp(DevAppViewModel devAppViewModel)
    {
        var project = this.selectedProjectService.GetSelectedProject();

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
                var (success, message) = LaunchProjectExecutor.OpenIDEWithFileName(
                    project.FullPath,
                    devAppViewModel.Path
                );

                if (!success)
                {
                    notificationMessageService.Create(
                        message,
                        "Open Project Error",
                        NotificationType.Error
                    );
                }
                return;
            }

            LaunchProjectExecutor.OpenIDE(
                new()
                {
                    FileName = devAppViewModel.Path,
                    Arguments = project.FullPath,
                    UseShellExecute = true,
                }
            );
        }
        catch (Exception ex)
        {
            this.notificationMessageService.Create(
                ex.Message,
                "Launch IDE Error",
                NotificationType.Error
            );
        }
    }

    public async Task Init()
    {
        SelectedProject = this.selectedProjectService.GetSelectedProject() ?? new();
        DevApps = [.. await devAppService.GetAll()];
    }
}
