using ApplicationCore.Features.DevApps;
using Moq;
using Xunit;

namespace Tests.ApplicationCoreTests.Features.DevApps
{
    public class DevAppServiceTests
    {
        [Fact]
        public async Task Add_ValidInput_CallsRepositoryAndReturnsTrue()
        {
            // Arrange
            var repo = new Mock<IDevAppRepository>();
            repo.Setup(r => r.Add(It.IsAny<DevApp>())).ReturnsAsync(true);
            var sut = new DevAppService(repo.Object);

            var input = new DevApp { Name = "VS Code", Path = "C:/Apps/code.exe" };

            // Act
            var result = await sut.Add(input);

            // Assert
            Assert.True(result);
            repo.Verify(r => r.Add(It.Is<DevApp>(d => d.Name == input.Name && d.Path == input.Path)), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Add_MissingName_Throws(string? name)
        {
            var repo = new Mock<IDevAppRepository>();
            var sut = new DevAppService(repo.Object);

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => sut.Add(new DevApp { Name = name!, Path = "c:/a.exe" }));
            Assert.Contains("Name is required!", ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Add_MissingPath_Throws(string? path)
        {
            var repo = new Mock<IDevAppRepository>();
            var sut = new DevAppService(repo.Object);

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => sut.Add(new DevApp { Name = "App", Path = path! }));
            Assert.Contains("Path is required!", ex.Message);
        }

        [Theory]
        [InlineData("c:/apps/app.txt")]
        [InlineData("c:/apps/app")]
        [InlineData("c:/apps/app.ex")]
        public async Task Add_PathNotExe_Throws(string path)
        {
            var repo = new Mock<IDevAppRepository>();
            var sut = new DevAppService(repo.Object);

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => sut.Add(new DevApp { Name = "App", Path = path }));
            Assert.Contains("must be executable file", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Edit_ValidInput_CallsRepositoryWithIdAndReturnsTrue()
        {
            var repo = new Mock<IDevAppRepository>();
            repo.Setup(r => r.Edit(It.IsAny<DevApp>())).ReturnsAsync(true);
            var sut = new DevAppService(repo.Object);

            var input = new DevApp { Id = 5, Name = "VS", Path = "c:/vs/DevEnv.exe" };

            var result = await sut.Edit(input);

            Assert.True(result);
            repo.Verify(r => r.Edit(It.Is<DevApp>(d => d.Id == 5 && d.Name == input.Name && d.Path == input.Path)), Times.Once);
        }

        [Fact]
        public async Task Delete_DelegatesToRepository()
        {
            var repo = new Mock<IDevAppRepository>();
            repo.Setup(r => r.Delete(10)).ReturnsAsync(true);
            var sut = new DevAppService(repo.Object);

            var result = await sut.Delete(10);

            Assert.True(result);
            repo.Verify(r => r.Delete(10), Times.Once);
        }

        [Fact]
        public async Task GetAll_MapsToViewModels()
        {
            var repo = new Mock<IDevAppRepository>();
            repo.Setup(r => r.GetAll()).ReturnsAsync(new List<DevApp>
            {
                new() { Id = 1, Name = "App1", Path = "c:/a1.exe" },
                new() { Id = 2, Name = "App2", Path = "c:/a2.exe" }
            });
            var sut = new DevAppService(repo.Object);

            var vms = await sut.GetAll();

            Assert.Equal(2, vms.Count());
            Assert.Contains(vms, vm => vm.Id == 1 && vm.Name == "App1" && vm.Path == "c:/a1.exe");
            Assert.Contains(vms, vm => vm.Id == 2 && vm.Name == "App2" && vm.Path == "c:/a2.exe");
        }

        [Fact]
        public async Task GetById_MapsEntityToDevApp()
        {
            var repo = new Mock<IDevAppRepository>();
            repo.Setup(r => r.GetById(7)).ReturnsAsync(new DevApp { Id = 7, Name = "App7", Path = "c:/a7.exe" });
            var sut = new DevAppService(repo.Object);

            var devApp = await sut.GetById(7);

            Assert.Equal(7, devApp.Id);
            Assert.Equal("App7", devApp.Name);
            Assert.Equal("c:/a7.exe", devApp.Path);
        }
    }
}
