using System.Collections.ObjectModel;
using ApplicationCore.Features.Projects;
using ApplicationCore.Features.DevApps;
using ApplicationCore.Common;
using Moq;
using UI.Features.Projects;
using UI.Shared.Services;
using Xunit;

namespace ApplicationCoreTests.UI
{
    public class ProjectsWindowViewModelTests
    {
        private ProjectsWindowViewModel CreateSut(ProjectViewModel selectedProject,
            out Mock<ISelectedProjectService> selectedProjectService,
            out Mock<IProjectService> projectService,
            out Mock<INotificationMessageService> notificationService,
            out Mock<IProjectWindowEventsService> windowEventsService,
            out Mock<IDevAppService> devAppService)
        {
            selectedProjectService = new Mock<ISelectedProjectService>();
            projectService = new Mock<IProjectService>();
            notificationService = new Mock<INotificationMessageService>();
            windowEventsService = new Mock<IProjectWindowEventsService>();
            devAppService = new Mock<IDevAppService>();

            selectedProjectService.Setup(s => s.GetSelectedProject()).Returns(selectedProject);

            var vm = new ProjectsWindowViewModel(projectService.Object,
                notificationService.Object,
                windowEventsService.Object,
                devAppService.Object,
                selectedProjectService.Object);

            // ensure DevApps is non-null to prevent NullReference in Project setter logic
            vm.DevApps = new ObservableCollection<DevAppViewModel>();

            return vm;
        }

        [Fact]
        public void SetSelectedProject_WhenServiceReturnsProject_SetsProjectAndSelectedDevApp()
        {
            var project = new ProjectViewModel
            {
                Id = 10,
                Name = "TestProj",
                Path = "c:/test",
                IDEPathId = 5,
                Filename = "test.sln"
            };

            var vm = CreateSut(project,
                out var selectedProjectService,
                out var projectService,
                out var notificationService,
                out var windowEventsService,
                out var devAppService);

            // Provide matching DevApp so SelectedDevApp can be set in Project setter
            vm.DevApps = new ObservableCollection<DevAppViewModel>
            {
                new() { Id = 5, Name = "IDE", Path = "c:/ide.exe" }
            };

            vm.SetSelectedProject();

            Assert.NotNull(vm.Project);
            Assert.Equal(project.Id, vm.Project!.Id);
            Assert.Equal(project.Name, vm.Project.Name);
            Assert.False(ReferenceEquals(project, vm.Project)); // deep copy expected
            Assert.NotNull(vm.SelectedDevApp);
            Assert.Equal(5, vm.SelectedDevApp!.Id);
        }

        [Fact]
        public void SetSelectedProject_WhenServiceReturnsNull_DoesNotChangeProject()
        {
            var initial = new ProjectViewModel { Id = 1, Name = "Initial", Path = "c:/init" }; // existing state
            var vm = CreateSut(null!,
                out var selectedProjectService,
                out var projectService,
                out var notificationService,
                out var windowEventsService,
                out var devAppService);

            // Seed current project
            vm.Project = initial;

            // Override service to return null
            selectedProjectService.Setup(s => s.GetSelectedProject()).Returns((ProjectViewModel)null!);

            vm.SetSelectedProject();

            Assert.Equal(initial.Id, vm.Project!.Id);
            Assert.Equal(initial.Name, vm.Project.Name);
        }
    }
}
