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
    private string _path = "";
    private ObservableCollection<IDEPathViewModel> devApps = [];
    private readonly IAddDevAppService devAppService;
    private readonly IEditDevAppService editDevAppService;
    private readonly IDeleteDevAppService deleteDevAppService;
    private readonly IGetAllDevAppService getAllDevAppService;
    private readonly IGetOneDevAppService getOneDevAppService;

    public async Task LoadDevAppsAsync()
    {
        var getAllDevAppServiceQuery = await getAllDevAppService.HandleAsync();
        DevApps = [.. getAllDevAppServiceQuery.DevApps];
    }
    public ObservableCollection<IDEPathViewModel> DevApps
    {
        get { return devApps; }
        set
        {
            devApps = value;
            OnPropertyChanged(nameof(this.DevApps));
        }
    }

    public DevAppsWindowViewModel(IAddDevAppService devAppService,
        IEditDevAppService editDevAppService,
        IDeleteDevAppService deleteDevAppService,
        IGetAllDevAppService getAllDevAppService,
        IGetOneDevAppService getOneDevAppService
    )
    {
        DeleteCommand = new RelayCommand<IDEPathViewModel>(DeleteItem);
        this.devAppService = devAppService;
        this.editDevAppService = editDevAppService;
        this.deleteDevAppService = deleteDevAppService;
        this.getAllDevAppService = getAllDevAppService;
        this.getOneDevAppService = getOneDevAppService;
    }

    public string Path
    {
        get { return _path; }
        set
        {
            _path = value;

            OnPropertyChanged(nameof(this.Path));
        }
    }

    private void DeleteItem(IDEPathViewModel item)
    {
        if (item != null)
        {
            DevApps.Remove(item);
        }
    }

}
