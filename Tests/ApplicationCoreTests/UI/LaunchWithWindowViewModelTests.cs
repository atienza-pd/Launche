using ApplicationCore.Common;
using ApplicationCore.Features.DevApps;
using ApplicationCore.Features.Projects;
using Moq;
using UI.Features.LaunchWith;
using UI.Shared.Services;
using Xunit;

namespace ApplicationCoreTests.UI
{
    public class LaunchWithWindowViewModelTests
    {
        private LaunchWithWindowViewModel CreateViewModel(
            Mock<IDevAppService> devAppServiceMock,
            ISelectedProjectService selectedProjectService,
            Mock<INotificationMessageService> notificationServiceMock
        )
        {
            return new LaunchWithWindowViewModel(
                devAppServiceMock.Object,
                selectedProjectService,
                notificationServiceMock.Object
            );
        }

        [Fact]
        public async Task Init_LoadsSelectedProjectAndDevApps()
        {
            var selectedProjectService = new SelectedProjectService();
            selectedProjectService.SetSelectedProject(
                new ProjectViewModel
                {
                    Id = 42,
                    Name = "Sample",
                    Path = "C:/projects/sample",
                    IDEPathId = 1,
                }
            );

            var devApps = new[]
            {
                new DevAppViewModel
                {
                    Id = 1,
                    Name = "VS Code",
                    Path = "code.exe",
                },
                new DevAppViewModel
                {
                    Id = 2,
                    Name = "Notepad",
                    Path = "notepad.exe",
                },
            };
            var devAppService = new Mock<IDevAppService>();
            devAppService.Setup(s => s.GetAll()).ReturnsAsync(devApps);
            var notificationService = new Mock<INotificationMessageService>();

            var vm = CreateViewModel(devAppService, selectedProjectService, notificationService);
            await vm.Init();

            Assert.Equal(42, vm.SelectedProject.Id);
            Assert.Equal("Sample", vm.SelectedProject.Name);
            Assert.Equal(devApps.Length, vm.DevApps.Count);
            Assert.True(vm.DevApps.Any(d => d.Name == "VS Code"));
        }

        [Fact]
        public void LaunchProjectDevAppCommand_DirectoryNotFound_CreatesErrorNotification()
        {
            var selectedProjectService = new SelectedProjectService();
            selectedProjectService.SetSelectedProject(
                new ProjectViewModel
                {
                    Id = 1,
                    Name = "MissingDirProject",
                    Path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()), // ensure non-existent
                }
            );

            var devAppService = new Mock<IDevAppService>();
            devAppService.Setup(s => s.GetAll()).ReturnsAsync(Array.Empty<DevAppViewModel>());
            var notificationService = new Mock<INotificationMessageService>();

            var vm = CreateViewModel(devAppService, selectedProjectService, notificationService);
            var devApp = new DevAppViewModel
            {
                Id = 9,
                Name = "Editor",
                Path = "editor.exe",
            };

            vm.LaunchProjectDevAppCommand.Execute(devApp);

            notificationService.Verify(
                n =>
                    n.Create(
                        It.Is<string>(m => m.StartsWith("Directory not found")),
                        "Launch IDE Error",
                        NotificationType.Error
                    ),
                Times.Once
            );
        }

        [Fact]
        public void LaunchProjectDevAppCommand_WithFileName_FileMissing_CreatesOpenProjectError()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir); // directory exists but file will not

            var selectedProjectService = new SelectedProjectService();
            selectedProjectService.SetSelectedProject(
                new ProjectViewModel
                {
                    Id = 2,
                    Name = "HasFileProject",
                    Path = tempDir,
                    Filename =
                        "nonexistent.txt" // ensures HasFileName == true and file absent
                    ,
                }
            );

            var devAppService = new Mock<IDevAppService>();
            devAppService.Setup(s => s.GetAll()).ReturnsAsync(Array.Empty<DevAppViewModel>());
            var notificationService = new Mock<INotificationMessageService>();

            var vm = CreateViewModel(devAppService, selectedProjectService, notificationService);
            var devApp = new DevAppViewModel
            {
                Id = 3,
                Name = "Editor",
                Path = "editor.exe",
            };

            vm.LaunchProjectDevAppCommand.Execute(devApp);

            notificationService.Verify(
                n =>
                    n.Create(
                        It.Is<string>(m => m.Contains("File not found")),
                        "Open Project Error",
                        NotificationType.Error
                    ),
                Times.Once
            );
        }

        [Fact]
        public void LaunchProjectDevAppCommand_WithoutFileName_ReachesOpenIDECodePath()
        {
            // Create a temporary directory that exists
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                var selectedProjectService = new SelectedProjectService();
                selectedProjectService.SetSelectedProject(
                    new ProjectViewModel
                    {
                        Id = 5,
                        Name = "DirectoryProject",
                        Path = tempDir,
                        Filename = "", // No filename, so HasFileName == false
                    }
                );

                var devAppService = new Mock<IDevAppService>();
                devAppService.Setup(s => s.GetAll()).ReturnsAsync(Array.Empty<DevAppViewModel>());
                var notificationService = new Mock<INotificationMessageService>();

                var vm = CreateViewModel(
                    devAppService,
                    selectedProjectService,
                    notificationService
                );
                var devApp = new DevAppViewModel
                {
                    Id = 4,
                    Name = "TestApp",
                    Path = "nonexistent.exe", // Non-existent app will cause exception
                };

                // Execute command - will try to launch process and catch exception
                vm.LaunchProjectDevAppCommand.Execute(devApp);

                // Verify error notification was created due to process launch failure
                // This confirms we reached the OpenIDE code path (lines 100-107)
                notificationService.Verify(
                    n => n.Create(It.IsAny<string>(), "Launch IDE Error", NotificationType.Error),
                    Times.Once
                );
            }
            finally
            {
                // Clean up
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir);
                }
            }
        }
    }
}
