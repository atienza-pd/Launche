using ApplicationCore.Features.Groups;
using Infrastructure.Repositories;
using Moq;
using Xunit;

namespace Tests.ApplicationCoreTests.Features.Groups;

public class GetAllGroupTests
{
    [Fact]
    public async Task HandleAsync_GetllAllGroup_Success()
    {
        // Arrange
        var stubGroupRepository = new Mock<IGroupRepository>();
        stubGroupRepository.Setup(x => x.GetAll()).ReturnsAsync([new()]);
        var sut = new GetAllGroupService(stubGroupRepository.Object);

        // Act
        var actual = await sut.HandleAsync(It.IsAny<GetAllGroupQuery>());

        // Assert
        Assert.NotEmpty(actual.Groups);
    }
}
