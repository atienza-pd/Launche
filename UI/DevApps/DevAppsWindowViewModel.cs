using ApplicationCore.Common;
using ApplicationCore.Features.DevApps;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using UI.Shared;

namespace UI.DevApps;

public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class DevAppsWindowViewModel : ViewModelBase
{
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

    public DevAppViewModel? DevApp
    {
        get { return _devApp; }
        set
        {
            _devApp = value.Copy();

            OnPropertyChanged(nameof(this.DevApp));
        }
    }

    private string _search;

    public string Search
    {
        get { return _search; }
        set
        {
            _search = value;
            OnPropertyChanged(nameof(this.Search));
            SearchDevApps(_search);
        }
    }

    private Visibility visibility = Visibility.Hidden;

    public Visibility Visibility
    {
        get { return visibility; }
        set
        {
            visibility = value;
            OnPropertyChanged(nameof(this.Visibility));
        }
    }


    private DevAppViewModel? _devApp = new();

    private ObservableCollection<DevAppViewModel> devApps = [];
    private readonly IEditDevAppService editDevAppService;
    private readonly IDeleteDevAppService deleteDevAppService;
    private readonly IGetAllDevAppService getAllDevAppService;
    private readonly IAddDevAppService addDevAppService;
    private readonly IGetOneDevAppService getOneDevAppService;
    private readonly INotificationMessageService notificationMessageService;
    private readonly IDevAppsSubscriptionService devAppsSubscriptionService;

    public DevAppsWindowViewModel(IAddDevAppService devAppService,
        IEditDevAppService editDevAppService,
        IDeleteDevAppService deleteDevAppService,
        IGetAllDevAppService getAllDevAppService,
        IAddDevAppService addDevAppService,
        IGetOneDevAppService getOneDevAppService,
        INotificationMessageService notificationMessageService,
        IDevAppsSubscriptionService devAppsSubscriptionService
    )
    {
        DeleteCommand = new RelayCommand(param => DeleteItem((DevAppViewModel)param!));
        SaveCommand = new RelayCommand(SaveDevAppsAsync);
        AddNewCommand = new RelayCommand(AddNew);
        OpenDialogCommand = new RelayCommand(OpenDialog);

        this.editDevAppService = editDevAppService;
        this.deleteDevAppService = deleteDevAppService;
        this.getAllDevAppService = getAllDevAppService;
        this.addDevAppService = addDevAppService;
        this.getOneDevAppService = getOneDevAppService;
        this.notificationMessageService = notificationMessageService;
        this.devAppsSubscriptionService = devAppsSubscriptionService;
    }

    private async void SearchDevApps(string search)
    {
        var result = await getAllDevAppService.HandleAsync();
        this.DevApps = [.. result.DevApps.Where(x => x.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase))];
    }

    private void OpenDialog()
    {
        var openFolderDialog = new OpenFileDialog { Filter = "Executable Files | *.exe" };
        var result = openFolderDialog.ShowDialog() ?? false;

        if (!result)
        {
            return;
        }

        DevApp = new() { Id = DevApp?.Id ?? 0, Path = openFolderDialog.FileName, Name = DevApp?.Name! };
    }

    private void AddNew()
    {
        SearchDevApps("");
        this.DevApp = new();
    }

    public async Task LoadDevAppsAsync()
    {
        var getAllDevAppServiceQuery = await getAllDevAppService.HandleAsync();

        DevApps = [.. getAllDevAppServiceQuery.DevApps];
    }

    private async void SaveDevAppsAsync()
    {
        try
        {
            if (DevApp == null)
            {
                this.notificationMessageService.Create("Invalid Dev App data.",
                    "Save Dev App",
                    NotificationType.Error);

                return;
            }

            if (String.IsNullOrEmpty(DevApp.Name) || String.IsNullOrWhiteSpace(DevApp.Name))
            {
                this.notificationMessageService.Create("Name is required!",
                    "Save Dev App",
                    NotificationType.Error);

                return;
            }

            if (String.IsNullOrEmpty(DevApp.Path) || String.IsNullOrWhiteSpace(DevApp.Path))
            {
                this.notificationMessageService.Create("Path is required!",
                    "Save Dev App",
                    NotificationType.Error);

                return;
            }

            if (this.DevApp.Id == 0)
            {
                await this.addDevAppService.HandleAsync(new AddDevAppCommand
                {
                    Name = this.DevApp.Name,
                    Path = this.DevApp.Path
                });

                Visibility = Visibility.Visible;
            }
            else
            {
                await this.editDevAppService.HandleAsync(new EditDevAppCommand
                {
                    Name = this.DevApp.Name,
                    Path = this.DevApp.Path,
                    Id = this.DevApp.Id
                });

                Visibility = Visibility.Visible;
            }

            this.devAppsSubscriptionService.Create();
            int id = this.DevApp.Id;

            var result = await getAllDevAppService.HandleAsync();
            this.DevApps = [.. result.DevApps.Where(x => x.Name.Contains(Search ?? "", StringComparison.CurrentCultureIgnoreCase))];

            this.DevApp = DevApps.FirstOrDefault(x => x.Id == id) ?? new();

            await Task.Delay(3000);
            Visibility = Visibility.Hidden;

        }
        catch (Exception ex)
        {
            this.notificationMessageService.Create(ex.Message,
                    "Save Dev App",
                    NotificationType.Error);
        }
    }

    private async void DeleteItem(DevAppViewModel item)
    {
        try
        {
            if (item != null)
            {

                var result = await this.deleteDevAppService.HandleAsync(new DeleteDevAppCommand
                {
                    Id = item.Id
                });

                if (result)
                {
                    DevApps.Remove(item);
                    DevApp = new();
                    devAppsSubscriptionService.Create();
                }
            }
        }
        catch (Exception ex)
        {

            this.notificationMessageService.Create(ex.Message,
                    "Delete Dev App",
                    NotificationType.Error);
        }

    }

}
