using ApplicationCore.Features.Projects;
using UI.Shared.Services;
using Xunit;

namespace ApplicationCoreTests.UI
{
    public class SelectedProjectServiceTests
    {
        [Fact]
        public void GetSelectedProject_Default_ReturnsNewInstance()
        {
            var sut = new SelectedProjectService();

            var result = sut.GetSelectedProject();

            Assert.NotNull(result);
            Assert.Equal(0, result.Id);
            Assert.Equal(string.Empty, result.Name);
            Assert.Equal(string.Empty, result.Path);
        }

        [Fact]
        public void SetSelectedProject_ThenGet_ReturnsSameReference()
        {
            var sut = new SelectedProjectService();
            var project = new ProjectViewModel
            {
                Id = 42,
                Name = "Example",
                Path = "c:/example",
                IDEPathId = 7,
                Filename = "example.sln",
                DevAppName = "VS",
            };

            sut.SetSelectedProject(project);
            var result = sut.GetSelectedProject();

            Assert.Same(project, result); // service stores reference
            Assert.Equal(42, result.Id);
            Assert.Equal("Example", result.Name);
            Assert.Equal("c:/example", result.Path);
            Assert.Equal(7, result.IDEPathId);
            Assert.Equal("example.sln", result.Filename);
        }

        [Fact]
        public void MutatingReturnedProject_AffectsStoredProject()
        {
            var sut = new SelectedProjectService();
            var project = new ProjectViewModel
            {
                Id = 1,
                Name = "Initial",
                Path = "c:/init",
            };
            sut.SetSelectedProject(project);

            // mutate after setting
            project.Name = "Changed";
            project.Path = "d:/changed";

            var result = sut.GetSelectedProject();
            Assert.Equal("Changed", result.Name);
            Assert.Equal("d:/changed", result.Path);
        }

        [Fact]
        public void Reset_SelectedProject()
        {
            var sut = new SelectedProjectService();
            var project = new ProjectViewModel
            {
                Id = 1,
                Name = "Initial",
                Path = "c:/init",
            };
            sut.SetSelectedProject(project);

            sut.Reset();

            var result = sut.GetSelectedProject();
            Assert.Equal(0, result.Id);
            Assert.Equal(string.Empty, result.Name);
            Assert.Equal(string.Empty, result.Path);
        }
    }
}
