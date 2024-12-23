using ApplicationCore.Features.Projects;
using Xunit;

namespace Tests.ApplicationCoreTests.Features.Projects
{
    public class GetAllProjectViewModelTests
    {
        [Fact]
        public void Count_Projects_Success()
        {
            // Act
            var actual = new GetAllProjectViewModel { Projects = [new()] };

            // Assert
            Assert.Equal(1, actual.Count);
        }
    }
}
