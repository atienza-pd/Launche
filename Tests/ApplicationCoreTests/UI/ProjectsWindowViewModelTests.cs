using System.Collections.ObjectModel;
using System.Windows;
using ApplicationCore.Common;
using ApplicationCore.Features.DevApps;
using ApplicationCore.Features.Projects;
using Moq;
using UI.Features.Projects;
using UI.Shared.Services;
using Xunit;

namespace ApplicationCoreTests.UI
{
    public class ProjectsWindowViewModelTests
    {
        private sealed record Sut(
            ProjectsWindowViewModel Vm,
            Mock<IProjectService> ProjectService,
            Mock<INotificationMessageService> NotificationService,
            Mock<IProjectWindowEventsService> WindowEventsService,
            Mock<IDevAppService> DevAppService,
            Mock<ISelectedProjectService> SelectedProjectService
        );

        private static Sut CreateSut(ProjectViewModel? selectedProject = null)
        {
            selectedProject ??= new ProjectViewModel();
            var selectedProjectService = new Mock<ISelectedProjectService>();
            var projectService = new Mock<IProjectService>();
            var notificationService = new Mock<INotificationMessageService>();
            var windowEventsService = new Mock<IProjectWindowEventsService>();
            var devAppService = new Mock<IDevAppService>();

            selectedProjectService.Setup(s => s.GetSelectedProject()).Returns(selectedProject);

            var vm = new ProjectsWindowViewModel(
                projectService.Object,
                notificationService.Object,
                windowEventsService.Object,
                devAppService.Object,
                selectedProjectService.Object
            )
            {
                DevApps = new ObservableCollection<DevAppViewModel>(),
            };

            return new Sut(
                vm,
                projectService,
                notificationService,
                windowEventsService,
                devAppService,
                selectedProjectService
            );
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
                Filename = "test.sln",
            };
            var sut = CreateSut(project);
            var vm = sut.Vm;

            // Provide matching DevApp so SelectedDevApp can be set in Project setter
            vm.DevApps = new ObservableCollection<DevAppViewModel>
            {
                new()
                {
                    Id = 5,
                    Name = "IDE",
                    Path = "c:/ide.exe",
                },
            };

            vm.SetSelectedProject();

            Assert.NotNull(vm.Project);
            Assert.Equal(project.Id, vm.Project!.Id);
            Assert.Equal(project.Name, vm.Project.Name);
            Assert.False(ReferenceEquals(project, vm.Project)); // deep copy expected
            Assert.NotNull(vm.SelectedDevApp);
            Assert.Equal(5, vm.SelectedDevApp!.Id);
            Assert.Equal(Visibility.Visible, vm.DeleteButtonVisibility);
        }

        [Fact]
        public void SetSelectedProject_WhenServiceReturnsNull_HidesDeleteButton()
        {
            var sut = CreateSut(null!);
            sut.SelectedProjectService.Setup(s => s.GetSelectedProject())
                .Returns(new ProjectViewModel());
            var vm = sut.Vm;

            vm.SetSelectedProject();

            Assert.Equal(Visibility.Hidden, vm.DeleteButtonVisibility);
        }

        [Fact]
        public void AddNewCommand_Resets_Project()
        {
            var sut = CreateSut(
                new ProjectViewModel
                {
                    Id = 3,
                    Name = "A",
                    Path = "c:/a",
                }
            );
            var vm = sut.Vm;
            vm.Project = new ProjectViewModel
            {
                Id = 99,
                Name = "Old",
                Path = "c:/old",
            };
            vm.AddNewCommand.Execute(null);
            Assert.NotNull(vm.Project);
            Assert.Equal(0, vm.Project!.Id);
            Assert.True(string.IsNullOrEmpty(vm.Project.Name));
        }

        [Fact]
        public async Task LoadDevApps_Populates_DevApps()
        {
            var list = new[]
            {
                new DevAppViewModel
                {
                    Id = 1,
                    Name = "VS",
                    Path = "c:/vs.exe",
                },
                new DevAppViewModel
                {
                    Id = 2,
                    Name = "Rider",
                    Path = "c:/rider.exe",
                },
            };
            var sut = CreateSut(new ProjectViewModel());
            var vm = sut.Vm;
            sut.DevAppService.Setup(s => s.GetAll()).ReturnsAsync(list);
            await vm.LoadDevApps();
            Assert.Equal(2, vm.DevApps.Count);
            Assert.Contains(vm.DevApps, d => d.Name == "VS");
        }

        [Fact]
        public async Task SaveCommand_NewProject_CallsAddAndShowsNotification()
        {
            var sut = CreateSut(new ProjectViewModel());
            var vm = sut.Vm;
            vm.Project = new ProjectViewModel
            {
                Id = 0,
                Name = "New Project",
                Path = "c:/new",
            };
            vm.SelectedDevApp = new DevAppViewModel
            {
                Id = 1,
                Name = "VS",
                Path = "c:/vs.exe",
            };

            sut.ProjectService.Setup(s => s.Add(It.IsAny<Project>())).ReturnsAsync(true);
            sut.ProjectService.Setup(s => s.GetAll())
                .ReturnsAsync(
                    new GetAllProjectViewModel
                    {
                        Projects = new[]
                        {
                            new ProjectViewModel
                            {
                                Id = 7,
                                Name = "New",
                                Path = "c:/new",
                                IDEPathId = 1,
                            },
                        },
                    }
                );

            vm.SaveCommand.Execute(null);
            await Task.Delay(50); // allow async void to complete

            sut.ProjectService.Verify(
                s => s.Add(It.Is<Project>(p => p.Name == "New Project")),
                Times.Once
            );
            sut.WindowEventsService.Verify(s => s.ProjectsChanged(), Times.Once);
            Assert.Equal(Visibility.Visible, vm.SaveNotificationVisibility);
        }

        [Fact]
        public async Task SaveCommand_DuplicatedName_ShowsErrorNotification()
        {
            var sut = CreateSut(new ProjectViewModel());
            var vm = sut.Vm;
            vm.Project = new ProjectViewModel
            {
                Id = 0,
                Name = "New",
                Path = "c:/new",
            };
            vm.SelectedDevApp = new DevAppViewModel
            {
                Id = 1,
                Name = "VS",
                Path = "c:/vs.exe",
            };

            sut.ProjectService.Setup(s => s.Add(It.IsAny<Project>())).ReturnsAsync(true);
            sut.ProjectService.Setup(s => s.GetAll())
                .ReturnsAsync(
                    new GetAllProjectViewModel
                    {
                        Projects = new[]
                        {
                            new ProjectViewModel
                            {
                                Id = 7,
                                Name = "New",
                                Path = "c:/new",
                                IDEPathId = 1,
                            },
                        },
                    }
                );

            vm.SaveCommand.Execute(null);
            await Task.Delay(50); // allow async void to complete

            sut.NotificationService.Verify(
                n =>
                    n.Create(
                        It.Is<string>(msg => msg.Contains("Name already exists!")),
                        "Save Project",
                        NotificationType.Error
                    ),
                Times.AtLeastOnce
            );
        }

        [Fact]
        public async Task SaveCommand_ExistingProject_CallsEdit()
        {
            var sut = CreateSut(new ProjectViewModel());
            var vm = sut.Vm;
            vm.Project = new ProjectViewModel
            {
                Id = 11,
                Name = "Existing",
                Path = "c:/exist",
            };
            vm.SelectedDevApp = new DevAppViewModel
            {
                Id = 2,
                Name = "Rider",
                Path = "c:/rider.exe",
            };

            sut.ProjectService.Setup(s => s.Edit(It.IsAny<Project>())).ReturnsAsync(true);
            sut.ProjectService.Setup(s => s.GetAll())
                .ReturnsAsync(
                    new GetAllProjectViewModel
                    {
                        Projects = new[]
                        {
                            new ProjectViewModel
                            {
                                Id = 11,
                                Name = "Existing",
                                Path = "c:/exist",
                                IDEPathId = 2,
                            },
                        },
                    }
                );

            vm.SaveCommand.Execute(null);
            await Task.Delay(50);

            sut.ProjectService.Verify(s => s.Edit(It.Is<Project>(p => p.Id == 11)), Times.Once);
            sut.WindowEventsService.Verify(s => s.ProjectsChanged(), Times.Once);
            Assert.Equal(Visibility.Visible, vm.SaveNotificationVisibility);
        }

        [Fact]
        public async Task SaveCommand_WhenValidateFieldsThrows_ShowsValidateProjectError()
        {
            // Arrange: cause projectService.GetAll inside ValidateFields to throw
            var sut = CreateSut(new ProjectViewModel());
            var vm = sut.Vm;
            vm.Project = new ProjectViewModel
            {
                Id = 0,
                Name = "Throwing",
                Path = "c:/throw",
            };
            vm.SelectedDevApp = new DevAppViewModel
            {
                Id = 1,
                Name = "VS",
                Path = "c:/vs.exe",
            };

            sut.ProjectService.Setup(s => s.GetAll()).ThrowsAsync(new Exception("Boom"));

            // Act
            vm.SaveCommand.Execute(null);
            await Task.Delay(50); // allow async void handler to run

            // Assert: notification created with Validate Project title and exception message
            sut.NotificationService.Verify(
                n =>
                    n.Create(
                        It.Is<string>(msg => msg.Contains("Boom")),
                        "Validate Project",
                        NotificationType.Error
                    ),
                Times.Once
            );
            // Ensure add/edit were never called due to validation failure
            sut.ProjectService.Verify(ps => ps.Add(It.IsAny<Project>()), Times.Never);
            sut.ProjectService.Verify(ps => ps.Edit(It.IsAny<Project>()), Times.Never);
        }

        [Fact]
        public void SaveCommand_InvalidProject_ShowsValidationErrors()
        {
            var sut = CreateSut(new ProjectViewModel());
            var vm = sut.Vm;
            vm.Project = new ProjectViewModel
            {
                Id = 0,
                Name = "",
                Path = "",
            };
            vm.SaveCommand.Execute(null);
            sut.ProjectService.Verify(s => s.Add(It.IsAny<Project>()), Times.Never);
            sut.NotificationService.Verify(
                n =>
                    n.Create(
                        It.Is<string>(msg =>
                            msg.Contains("Name is required") || msg.Contains("Path is required")
                        ),
                        "Save Project",
                        NotificationType.Error
                    ),
                Times.AtLeastOnce
            );
        }

        [Fact]
        public async Task DeleteCommand_WithMultipleProjects_DeletesAndCloses()
        {
            var sut = CreateSut(new ProjectViewModel());
            var vm = sut.Vm;
            vm.Project = new ProjectViewModel
            {
                Id = 3,
                Name = "Del",
                Path = "c:/del",
            };
            sut.ProjectService.Setup(s => s.GetAll())
                .ReturnsAsync(
                    new GetAllProjectViewModel
                    {
                        Projects = new[]
                        {
                            new ProjectViewModel
                            {
                                Id = 1,
                                Name = "Other",
                                Path = "c:/other",
                            },
                            new ProjectViewModel
                            {
                                Id = 3,
                                Name = "Del",
                                Path = "c:/del",
                            },
                        },
                    }
                );
            sut.ProjectService.Setup(s => s.Delete(3)).ReturnsAsync(true);

            bool closed = false;
            vm.CloseWindowEvent += (_, _) => closed = true;

            vm.DeleteCommand.Execute(null);
            await Task.Delay(50);

            sut.ProjectService.Verify(s => s.Delete(3), Times.Once);
            sut.WindowEventsService.Verify(s => s.ProjectsChanged(), Times.Once);
            Assert.True(closed);
        }

        [Fact]
        public async Task DeleteCommand_SingleProject_ShowsErrorAndDoesNotDelete()
        {
            var sut = CreateSut(new ProjectViewModel());
            var vm = sut.Vm;
            vm.Project = new ProjectViewModel
            {
                Id = 1,
                Name = "Only",
                Path = "c:/only",
            };
            sut.ProjectService.Setup(s => s.GetAll())
                .ReturnsAsync(
                    new GetAllProjectViewModel
                    {
                        Projects = new[]
                        {
                            new ProjectViewModel
                            {
                                Id = 1,
                                Name = "Only",
                                Path = "c:/only",
                            },
                        },
                    }
                );

            vm.DeleteCommand.Execute(null);
            await Task.Delay(50);

            sut.ProjectService.Verify(s => s.Delete(It.IsAny<long>()), Times.Never);
            sut.WindowEventsService.Verify(s => s.ProjectsChanged(), Times.Never);
            sut.NotificationService.Verify(
                n =>
                    n.Create(
                        It.Is<string>(msg => msg.Contains("Single project should exists")),
                        "Delete Project",
                        NotificationType.Error
                    ),
                Times.Once
            );
        }
    }
}
