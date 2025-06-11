using ApplicationCore.Features.Projects;
using Infrastructure.Models;
using Infrastructure.Repositories;
using Moq;
using Xunit;

namespace Tests.ApplicationCoreTests.Features.Projects
{
    public class SearchProjectTests
    {
        [Fact]
        public async Task HandleAsync_EmptyQuerySearch_Success()
        {
            // Arange
            var stubProjectRepository = new Mock<IProjectRepository>();
            stubProjectRepository.Setup(x => x.GetAll()).ReturnsAsync([new()]);
            var stubDevAppRepository = new Mock<IDevAppRepository>();
            stubDevAppRepository.Setup(x => x.GetAll()).ReturnsAsync([new DevApp()]);
            var stubGroupRepository = new Mock<IGroupRepository>();
            stubGroupRepository.Setup(x => x.GetAll()).ReturnsAsync([new()]);
            var sut = new SearchProjectService(
                stubProjectRepository.Object,
                stubGroupRepository.Object,
                stubDevAppRepository.Object
            );

            // Act
            var actual = await sut.HandleAsync(new() { Search = "" });

            // Assert
            Assert.NotEmpty(actual.Projects);
        }

        [Fact]
        public async Task HandleAsync_QuerySearch_Success()
        {
            // Arange
            var stubProjectRepository = new Mock<IProjectRepository>();
            stubProjectRepository.Setup(x => x.GetAll()).ReturnsAsync([new() { Name = "Search" }]);
            var stubDevAppRepository = new Mock<IDevAppRepository>();
            stubDevAppRepository.Setup(x => x.GetAll()).ReturnsAsync([new DevApp()]);
            var stubGroupRepository = new Mock<IGroupRepository>();
            stubGroupRepository.Setup(x => x.GetAll()).ReturnsAsync([new()]);
            var sut = new SearchProjectService(
                stubProjectRepository.Object,
                stubGroupRepository.Object,
                stubDevAppRepository.Object
            );

            // Act
            var actual = await sut.HandleAsync(new() { Search = "Search" });

            // Assert
            Assert.NotEmpty(actual.Projects);
        }
    }
}
