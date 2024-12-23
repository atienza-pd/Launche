using ApplicationCore.Common;
using ApplicationCore.Features.Projects;
using Infrastructure.Models;
using Moq;
using Xunit;

namespace Tests.ApplicationCoreTests.Features.Projects
{
    public class EditProjectServiceTests
    {
        [Fact]
        public async Task HandleAsync_EditProject_Success()
        {
            // Arrange
            var stubRepository = new Mock<IProjectRepository>();
            stubRepository.Setup(x => x.GetOne(It.IsAny<long>())).ReturnsAsync(new Project());
            stubRepository.Setup(x => x.Edit(It.IsAny<Project>())).ReturnsAsync(true);
            var stubNotificationMessageService = new Mock<INotificationMessageService>();
            var sut = new EditProjectService(
                stubRepository.Object,
                stubNotificationMessageService.Object
            );

            // Act
            var actual = await sut.HandleAsync(
                new(
                    Id: It.IsAny<long>(),
                    Name: "Name",
                    Path: "Path",
                    FileName: It.IsAny<string>(),
                    IDEPathId: 1
                )
            );

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public async Task HandleAsync_NullProject_Failed()
        {
            // Arrange
            var stubRepository = new Mock<IProjectRepository>();
            var stubNotificationMessageService = new Mock<INotificationMessageService>();
            var sut = new EditProjectService(
                stubRepository.Object,
                stubNotificationMessageService.Object
            );

            // Act
            var actual = await sut.HandleAsync(
                new(
                    Id: It.IsAny<int>(),
                    Name: "Name",
                    Path: "Path",
                    FileName: It.IsAny<string>(),
                    IDEPathId: 1
                )
            );

            // Assert
            Assert.False(actual);
        }

        [Theory]
        [InlineData("", "Path", 1)]
        [InlineData("Name", "", 1)]
        [InlineData("Name", "Path", 0)]
        public async Task HandleAsync_EditProject_Failed(string name, string path, int devAppId)
        {
            // Arrange
            var stubRepository = new Mock<IProjectRepository>();
            stubRepository.Setup(x => x.GetOne(It.IsAny<long>())).ReturnsAsync(new Project());
            stubRepository.Setup(x => x.Edit(It.IsAny<Project>())).ReturnsAsync(true);
            var stubNotificationMessageService = new Mock<INotificationMessageService>();
            var sut = new EditProjectService(
                stubRepository.Object,
                stubNotificationMessageService.Object
            );

            // Act
            var actual = await sut.HandleAsync(
                new(
                    Id: It.IsAny<long>(),
                    Name: name,
                    Path: path,
                    FileName: It.IsAny<string>(),
                    IDEPathId: devAppId
                )
            );

            // Assert
            Assert.False(actual);
        }
    }
}
