using ApplicationCore.Features.DevApps;
using System.Windows;

namespace UI
{
    /// <summary>
    /// Interaction logic for DevAppsWindow.xaml
    /// </summary>
    public partial class DevAppsWindow : Window
    {
        public DevAppsWindow(
            IAddDevAppService devAppService,
            IEditDevAppService editDevAppService,
            IDeleteDevAppService deleteDevAppService,
            IGetAllDevAppService getAllDevAppService,
            IGetOneDevAppService getOneDevAppService)
        {
            InitializeComponent();
        }
    }
}
