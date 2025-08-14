using ApplicationCore.Features.Git;
using Moq;
using Xunit;

namespace Tests.ApplicationCoreTests.Features.Gits
{
    public class GitServiceTests
    {
        [Fact]
        public void GetCurrentBranch_ReturnsBranch_FromRepository()
        {
            // Arrange
            var repo = new Mock<IGitRepository>();
            var path = "C:/src/project";
            repo.Setup(r => r.GetCurrentBranch(path)).Returns("main");
            var sut = new GitService(repo.Object);

            // Act
            var branch = sut.GetCurrentBranch(path);

            // Assert
            Assert.Equal("main", branch);
            repo.Verify(r => r.GetCurrentBranch(path), Times.Once);
        }

        [Fact]
        public void GetCurrentBranch_WhenRepositoryThrows_WrapsInGitServiceException()
        {
            // Arrange
            var repo = new Mock<IGitRepository>();
            var path = "C:/src/project";
            var inner = new InvalidOperationException("no repo");
            repo.Setup(r => r.GetCurrentBranch(path)).Throws(inner);
            var sut = new GitService(repo.Object);

            // Act
            var ex = Assert.Throws<GitServiceException>(() => sut.GetCurrentBranch(path));

            // Assert
            Assert.Contains("No Git Branch Found!", ex.Message);
            Assert.Same(inner, ex.InnerException);
        }
    }
}
