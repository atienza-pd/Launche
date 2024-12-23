using ApplicationCore.Common;
using ApplicationCore.Features.Projects;
using Infrastructure.Models;
using Moq;
using Xunit;

namespace Tests.ApplicationCoreTests.Features.Projects
{
    public class RemoveProjectFromGroupTests
    {
        [Fact]
        public async Task HandleAsync_RemoveProjectFromGroup_Success()
        {
            // Arrange
            var stubProjectRepository = new Mock<IProjectRepository>();
            stubProjectRepository.Setup(x => x.Edit(It.IsAny<Project>())).ReturnsAsync(true);
            stubProjectRepository
                .Setup(x => x.GetOne(It.IsAny<long>()))
                .ReturnsAsync(new Project());
            var stubNotificationMessageService = new Mock<INotificationMessageService>();
            var sut = new RemoveProjectFromGroupService(
                stubProjectRepository.Object,
                stubNotificationMessageService.Object
            );
            sut.Notify += (sender, args) => { };

            // Act
            var actual = await sut.HandleAsync(new());

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public async Task HandleAsync_NullProject_Failed()
        {
            // Arrange
            var stubProjectRepository = new Mock<IProjectRepository>();
            stubProjectRepository.Setup(x => x.Edit(It.IsAny<Project>())).ReturnsAsync(true);
            var stubNotificationMessageService = new Mock<INotificationMessageService>();
            var sut = new RemoveProjectFromGroupService(
                stubProjectRepository.Object,
                stubNotificationMessageService.Object
            );
            sut.Notify += (sender, args) => { };

            // Act
            var actual = await sut.HandleAsync(new());

            // Assert
            Assert.False(actual);
        }
    }
}
