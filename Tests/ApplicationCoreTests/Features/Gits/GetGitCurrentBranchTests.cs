﻿using ApplicationCore.Features.Git;
using Infrastructure;
using Moq;
using Xunit;

namespace Tests.ApplicationCoreTests.Features.Gits;

public class GetGitCurrentBranchTests
{
    [Fact]
    public void HandleAsync_GetOneDevApp_Success()
    {
        // Arrange
        var stubService = new Mock<IGitRepository>();

        stubService.Setup(x => x.GetCurrentBranch(It.IsAny<string>())).Returns("Branch");

        var sut = new GetCurrentGitBranchService(stubService.Object);

        // Act
        var actual = sut.Handle(new());

        // Assert
        Assert.Equal("Branch", actual);
    }
}
