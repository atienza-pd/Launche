using ApplicationCore.Features.DevApps;
using ApplicationCore.Features.Projects;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using UI.Shared;

namespace Infrastructure.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {

        public ICommand RefreshDevAppsCommand { get; }


        public MainWindowViewModel()
        {
            this.ProjectPathModels = [];
            this.IdePathsModels = [];
            this.SelectedProjectPath = null;
            this.SelectedIdePath = new();
            this.search = "";
            this.RefreshDevAppsCommand = new RelayCommand(RefreshDevApps);
        }

        private void RefreshDevApps()
        {
            MessageBox.Show("Refresh");
        }

        private bool _enableAddNewProject = true;

        public bool EnableAddNewProject
        {
            get { return _enableAddNewProject; }
            set
            {
                _enableAddNewProject = value;
                PropertyChanged?.Invoke(
                    this,
                    new PropertyChangedEventArgs(nameof(this.EnableAddNewProject))
                );
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private ObservableCollection<ProjectViewModel>? projectPathModels;

        public ObservableCollection<ProjectViewModel>? ProjectPathModels
        {
            get { return projectPathModels; }
            set
            {
                projectPathModels = value;
                PropertyChanged?.Invoke(
                    this,
                    new PropertyChangedEventArgs(nameof(this.ProjectPathModels))
                );
            }
        }

        private ObservableCollection<DevAppViewModel>? idePathsModels;

        public ObservableCollection<DevAppViewModel>? IdePathsModels
        {
            get { return idePathsModels; }
            set
            {
                idePathsModels = value;
                PropertyChanged?.Invoke(
                    this,
                    new PropertyChangedEventArgs(nameof(this.IdePathsModels))
                );
            }
        }

        private ProjectViewModel? selectedProjectPath;

        public ProjectViewModel? SelectedProjectPath
        {
            get { return selectedProjectPath; }
            set
            {
                selectedProjectPath = value;
                PropertyChanged?.Invoke(
                    this,
                    new PropertyChangedEventArgs(nameof(this.SelectedProjectPath))
                );
            }
        }

        private DevAppViewModel? selectedIdePath;

        public DevAppViewModel? SelectedIdePath
        {
            get { return selectedIdePath; }
            set
            {
                selectedIdePath = value;
                PropertyChanged?.Invoke(
                    this,
                    new PropertyChangedEventArgs(nameof(this.SelectedIdePath))
                );
            }
        }

        private string search;
        public string Search
        {
            get { return search; }
            set
            {
                search = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Search)));
            }
        }
    }
}
