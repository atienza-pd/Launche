using ApplicationCore.Common;
using ApplicationCore.Features.DevApps;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

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

    private DevAppViewModel? _devApp = new();

    private ObservableCollection<DevAppViewModel> devApps = [];
    private readonly IAddDevAppService devAppService;
    private readonly IEditDevAppService editDevAppService;
    private readonly IDeleteDevAppService deleteDevAppService;
    private readonly IGetAllDevAppService getAllDevAppService;
    private readonly IAddDevAppService addDevAppService;
    private readonly IGetOneDevAppService getOneDevAppService;
    private readonly INotificationMessageService notificationMessageService;

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
            _devApp = value;

            OnPropertyChanged(nameof(this.DevApp));
        }
    }


    public DevAppsWindowViewModel(IAddDevAppService devAppService,
        IEditDevAppService editDevAppService,
        IDeleteDevAppService deleteDevAppService,
        IGetAllDevAppService getAllDevAppService,
        IAddDevAppService addDevAppService,
        IGetOneDevAppService getOneDevAppService,
        INotificationMessageService notificationMessageService
    )
    {
        DeleteCommand = new RelayCommand(param => DeleteItem((DevAppViewModel)param!));
        SaveCommand = new RelayCommand(SaveDevAppsAsync);
        AddNewCommand = new RelayCommand(AddNew);

        this.devAppService = devAppService;
        this.editDevAppService = editDevAppService;
        this.deleteDevAppService = deleteDevAppService;
        this.getAllDevAppService = getAllDevAppService;
        this.addDevAppService = addDevAppService;
        this.getOneDevAppService = getOneDevAppService;
        this.notificationMessageService = notificationMessageService;
    }

    private void AddNew()
    {
        this.DevApp = null;

    }

    public async Task LoadDevAppsAsync()
    {
        var getAllDevAppServiceQuery = await getAllDevAppService.HandleAsync();

        DevApps = [.. getAllDevAppServiceQuery.DevApps];
    }

    private async void SaveDevAppsAsync()
    {
        if (DevApp == null)
        {
            this.notificationMessageService.Create("Invalid Dev App data.",
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
                Path = this.DevApp.Path
            });
        }
        else
        {
            await this.editDevAppService.HandleAsync(new EditDevAppCommand
            {
                Path = this.DevApp.Path,
                Id = this.DevApp.Id
            });
        }

        await this.LoadDevAppsAsync();

    }

    private async void DeleteItem(DevAppViewModel item)
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
            }
        }
    }

}
