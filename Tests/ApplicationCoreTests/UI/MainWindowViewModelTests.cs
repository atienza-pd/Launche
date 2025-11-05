using ApplicationCore.Features.Projects;
using Moq;
using UI.MainWindows;
using UI.Features;
using UI.Shared.Services;
using ApplicationCore.Common;
using Xunit;
using UI.Features.Projects;
using ApplicationCore.Features.DevApps;

namespace ApplicationCoreTests.UI
{
    public class MainWindowViewModelTests
    {
        private class TestableMainWindowViewModel : MainWindowViewModel
        {
            public bool ShowCalled { get; private set; }
            public IProjectsWindow? PassedWindow { get; private set; }

            public TestableMainWindowViewModel(IProjectService projectService,
                INotificationMessageService notificationMessageService,
                IServiceProvider serviceProvider,
                ISelectedProjectService selectedProjectService)
                : base(projectService, notificationMessageService, serviceProvider, selectedProjectService)
            { }

            protected override void ShowProjectsWindow(IProjectsWindow window)
            {
                ShowCalled = true;
                PassedWindow = window;
            }
        }

        [Fact]
        public void EditCommand_SetsSelectedProject_And_ShowsProjectsWindow()
        {
            var projectService = new Mock<IProjectService>();
            var notificationService = new Mock<INotificationMessageService>();
            var selectedProjectService = new SelectedProjectService();
            var windowEventsService = new Mock<IProjectWindowEventsService>();
            var devAppService = new Mock<IDevAppService>();
            devAppService.Setup(d => d.GetAll()).ReturnsAsync([]);
            projectService.Setup(p => p.GetOne(It.IsAny<long>()))
                .ReturnsAsync((long id) => new ProjectViewModel { Id = id, Name = "Proj", Path = "c:/p", IDEPathId = 2 });

            // Create a mock implementing IProjectsWindow without needing concrete WPF window
            var projectsWindowMock = new Mock<IProjectsWindow>();
            projectsWindowMock.Setup(w => w.ShowDialog()).Returns(true);

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(sp => sp.GetService(typeof(IProjectsWindow))).Returns(projectsWindowMock.Object);

            var vm = new TestableMainWindowViewModel(projectService.Object, notificationService.Object, serviceProvider.Object, selectedProjectService);
            var existing = new ProjectViewModel { Id = 7, Name = "Proj", Path = "c:/p", IDEPathId = 2 };
            vm.SelectedProject = existing;

            vm.EditCommand.Execute(null);

            var stored = selectedProjectService.GetSelectedProject();
            Assert.Equal(existing.Id, stored.Id);
            Assert.Equal(existing.Name, stored.Name);
            Assert.True(vm.ShowCalled);
            Assert.NotNull(vm.PassedWindow);
        }

        [Fact]
        public void EditCommand_DoesNothing_When_NoWindowResolved()
        {
            var projectService = new Mock<IProjectService>();
            projectService.Setup(p => p.GetOne(It.IsAny<long>()))
                .ReturnsAsync((long id) => new ProjectViewModel { Id = id, Name = "A", Path = "c:/a" });
            var notificationService = new Mock<INotificationMessageService>();
            var selectedProjectService = new SelectedProjectService();

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(sp => sp.GetService(typeof(IProjectsWindow))).Returns(null);

            var vm = new TestableMainWindowViewModel(projectService.Object, notificationService.Object, serviceProvider.Object, selectedProjectService);
            vm.SelectedProject = new ProjectViewModel { Id = 1, Name = "A", Path = "c:/a" };

            vm.EditCommand.Execute(null);

            Assert.False(vm.ShowCalled);
        }
    }
}
