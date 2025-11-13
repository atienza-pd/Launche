using ApplicationCore.Features.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Shared.Services
{
    public interface ISelectedProjectService
    {
        ProjectViewModel GetSelectedProject();
        void Reset();
        void SetSelectedProject(ProjectViewModel project);
    }

    public class SelectedProjectService : ISelectedProjectService
    {
        private ProjectViewModel selectedProject = new();

        public ProjectViewModel GetSelectedProject()
        {
            return this.selectedProject;
        }

        public void Reset()
        {
            this.selectedProject = new();
        }

        public void SetSelectedProject(ProjectViewModel project)
        {
            this.selectedProject = project;
        }
    }
}
