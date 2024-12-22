using ApplicationCore.Features.Projects;
using Infrastructure;
using Infrastructure.Models;
using Infrastructure.Repositories;
using Moq;
using Xunit;

namespace Tests.ApplicationCoreTests.Features.Projects
{
    public class GetAllProjectTests
    {
        [Fact]
        public async Task HandleAsync_GetAllProjects_Success()
        {
            // Arrange
            var stubProjectRepository = new Mock<IProjectRepository>();
            var stubDevAppRepository = new Mock<IDevAppRepository>();
            var stubGitRepository = new Mock<IGitService>();
            var stubGroupRepository = new Mock<IGroupRepository>();
            stubProjectRepository
                .Setup(x => x.GetAll())
                .ReturnsAsync([new Project() { Id = 1, GroupId = 1 }]);

            stubDevAppRepository.Setup(x => x.GetAll()).ReturnsAsync([new IDEPath()]);
            stubGroupRepository
                .Setup(x => x.GetAll())
                .ReturnsAsync([new Group() { Id = 1, Name = "Name" }]);

            var sut = new GetAllProjectService(
                stubProjectRepository.Object,
                stubDevAppRepository.Object,
                stubGitRepository.Object,
                stubGroupRepository.Object
            );

            // Act
            var actual = await sut.HandleAsync();

            // Assert
            Assert.NotEmpty(actual.Projects);
            Assert.Contains(1, actual.Projects.Select(x => x.Id));
            Assert.Contains("Name", actual.Projects.Select(x => x.GroupName));
        }
    }
}
