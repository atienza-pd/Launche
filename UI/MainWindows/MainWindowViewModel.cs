using ApplicationCore.Features.DevApps;
using ApplicationCore.Features.Projects;
using System.Collections.ObjectModel;
using UI.Shared;

namespace UI.MainWindows
{
    public class MainWindowViewModel(IProjectService projectService) : ViewModelBase
    {
        private readonly IProjectService projectService = projectService;
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
                project = value;

                OnPropertyChanged(nameof(this.Project));

            }
        }

        public ProjectViewModel? SelectedProject
        {
            get { return project; }
            set
            {
                project = value.Copy();

                OnPropertyChanged(nameof(this.Project));
                this.GetProject();
            }
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
