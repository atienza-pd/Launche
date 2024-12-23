using ApplicationCore.Features.Projects;
using Moq;
using Xunit;

namespace Tests.ApplicationCoreTests.Features.Projects
{
    public class SortDownProjectTests
    {
        [Fact]
        public async Task HandleAsync_SortDownProject_Success()
        {
            // Arrange
            var stubProjectRepository = new Mock<IProjectRepository>();
            stubProjectRepository.Setup(x => x.SortDown(It.IsAny<int>())).ReturnsAsync(true);
            var sut = new SortDownProjectService(stubProjectRepository.Object);

            // Act
            var actual = await sut.HandleAsync(new());

            // Assert
            Assert.True(actual);
        }
    }
}
