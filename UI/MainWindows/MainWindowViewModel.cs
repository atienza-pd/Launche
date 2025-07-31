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

        public ObservableCollection<ProjectViewModel> Projects
        {
            get { return _projects; }
            set
            {
                _projects = value;
                OnPropertyChanged(nameof(this.Projects));
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

        private async void SearchProjects(string search)
        {
            var result = await projectService.GetAll();
            this.Projects = [.. result.Projects.Where(x => x.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase))];
        }

        public void LoadProjects()
        {
            SearchProjects("");
        }
    }
}
