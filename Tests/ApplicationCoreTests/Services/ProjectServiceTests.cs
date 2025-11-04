using ApplicationCore.Features.DevApps;
using ApplicationCore.Features.Git;
using ApplicationCore.Features.Groups;
using ApplicationCore.Features.Projects;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests.ApplicationCoreTests.Services
{
    public class ProjectServiceTests
    {
        private static ProjectService CreateSut(
            out Mock<IProjectRepository> projRepo,
            out Mock<IDevAppRepository> devRepo,
            out Mock<IGroupRepository> groupRepo,
            out Mock<IGitService> gitSvc)
        {
            projRepo = new Mock<IProjectRepository>();
            devRepo = new Mock<IDevAppRepository>();
            groupRepo = new Mock<IGroupRepository>();
            gitSvc = new Mock<IGitService>();
            return new ProjectService(projRepo.Object, devRepo.Object, groupRepo.Object, gitSvc.Object);
        }

        [Fact]
        public async Task GetAll_Maps_All_Fields()
        {
            // Arrange
            var sut = CreateSut(out var projRepo, out var devRepo, out var groupRepo, out var _);
            projRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<Project>
            {
                new() { Id = 1, Name = "P1", Path = "c:/p1", IDEPathId = 1, SortId = 1, GroupId = 10 },
                new() { Id = 2, Name = "P2", Path = "c:/p2", IDEPathId = 2, SortId = 2, GroupId = null }
            });
            devRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<DevApp>
            {
                new() { Id = 1, Name = "VS", Path = "c:/vs.exe" },
                new() { Id = 2, Name = "Code", Path = "c:/code.exe" }
            });
            groupRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<Group> { new() { Id = 10, Name = "Team" } });

            // Act
            var result = await sut.GetAll();

            // Assert
            Assert.Equal(2, result.Projects.Count());
            var first = result.Projects.First();
            Assert.Equal(1, first.Id);
            Assert.Equal("P1", first.Name);
            Assert.Equal("Team", first.GroupName);
            Assert.Equal("c:/vs.exe", first.DevAppPath);
            Assert.Equal("VS", first.DevAppName);
            Assert.True(first.EnableMoveDown);
            Assert.False(first.EnableMoveUp); // first item cannot move up
        }

        [Fact]
        public async Task GetOne_Maps_DevApp_And_GitBranch()
        {
            var sut = CreateSut(out var projRepo, out var devRepo, out var _, out var gitSvc);
            projRepo.Setup(r => r.GetOne(5)).ReturnsAsync(new Project { Id = 5, Name = "P", Path = "c:/p", IDEPathId = 2, SortId = 3, Filename = "x.sln" });
            devRepo.Setup(r => r.GetById(2)).ReturnsAsync(new DevApp { Id = 2, Name = "Code", Path = "c:/code.exe" });
            gitSvc.Setup(g => g.GetCurrentBranch("c:/p")).Returns("main");

            var vm = await sut.GetOne(5);

            Assert.Equal(5, vm.Id);
            Assert.Equal("Code", vm.DevAppName);
            Assert.Equal("c:/code.exe", vm.DevAppPath);
            Assert.Equal("main", vm.CurrentGitBranch);
        }

        [Fact]
        public async Task Add_WhenDevAppIdNull_UsesFirstDevApp_And_AppendsSortId()
        {
            var sut = CreateSut(out var projRepo, out var devRepo, out var _, out var _git);
            projRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<Project> { new() { SortId = 4 } });
            devRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<DevApp> { new() { Id = 7, Name = "VS", Path = "c:/vs.exe" } });
            projRepo.Setup(r => r.Add(It.IsAny<Project>())).ReturnsAsync(true);

            var ok = await sut.Add(new Project { Name = "P", Path = "c:/p", IDEPathId = null, Filename = "f.sln" });

            Assert.True(ok);
            projRepo.Verify(r => r.Add(It.Is<Project>(p => p.IDEPathId == 7 && p.SortId == 5 && p.Name == "P" && p.Path == "c:/p" && p.Filename == "f.sln")), Times.Once);
        }

        [Theory]
        [InlineData("", "c:/p")]
        [InlineData("P", "")]
        public async Task Add_Validation_Throws(string name, string path)
        {
            var sut = CreateSut(out var _, out var __, out var ___, out var ____);

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => sut.Add(new Project { Name = name, Path = path }));
            Assert.Contains("must be provided", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Edit_Updates_And_Sets_Default_DevApp_When_Zero()
        {
            var sut = CreateSut(out var projRepo, out var devRepo, out var _, out var _git);
            var existing = new Project { Id = 3, Name = "Old", Path = "c:/old", IDEPathId = 1, Filename = "a.sln" };
            projRepo.Setup(r => r.GetOne(3)).ReturnsAsync(existing);
            devRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<DevApp> { new() { Id = 9, Name = "VS", Path = "c:/vs.exe" } });
            projRepo.Setup(r => r.Edit(It.IsAny<Project>())).ReturnsAsync(true);

            var ok = await sut.Edit(new Project { Id = 3, Name = "New", Path = "c:/new", IDEPathId = 0, Filename = "b.sln" });

            Assert.True(ok);
            projRepo.Verify(r => r.Edit(It.Is<Project>(p => p.Id == 3 && p.Name == "New" && p.Path == "c:/new" && p.IDEPathId == 9 && p.Filename == "b.sln")), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenSingleProject_Throws()
        {
            var sut = CreateSut(out var projRepo, out var _, out var __, out var ___);
            projRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<Project> { new() { Id = 1 } });

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => sut.Delete(1));
            Assert.Contains("Single project should exists", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Delete_CallsRepository_WhenMultiple()
        {
            var sut = CreateSut(out var projRepo, out var _, out var __, out var ___);
            projRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<Project> { new() { Id = 1 }, new() { Id = 2 } });
            projRepo.Setup(r => r.Delete(2)).ReturnsAsync(true);

            var ok = await sut.Delete(2);

            Assert.True(ok);
            projRepo.Verify(r => r.Delete(2), Times.Once);
        }

        [Fact]
        public async Task Search_WithCriteria_DisablesAddNew_And_DisablesMoveFlags()
        {
            var sut = CreateSut(out var projRepo, out var devRepo, out var groupRepo, out var _git);
            projRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<Project>
            {
                new() { Id = 1, Name = "Alpha", Path = "c:/a", IDEPathId = 1, SortId = 1, GroupId = 10 },
                new() { Id = 2, Name = "Beta", Path = "c:/b", IDEPathId = 1, SortId = 2, GroupId = 11 },
            });
            devRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<DevApp> { new() { Id = 1, Name = "VS", Path = "c:/vs.exe" } });
            groupRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<Group> { new() { Id = 10, Name = "G10" }, new() { Id = 11, Name = "G11" } });

            var result = await sut.Search("a"); // matches Alpha only

            Assert.False(result.EnableAddNewProject);
            Assert.StrictEqual(2, result.Projects.Count());
            var item = result.Projects.First();
            Assert.False(item.EnableMoveDown);
            Assert.False(item.EnableMoveUp);
            Assert.Equal("G10", item.GroupName);
        }

        [Fact]
        public async Task Search_EmptyCriteria_EnablesAddNew_And_SetsMoveFlags()
        {
            var sut = CreateSut(out var projRepo, out var devRepo, out var groupRepo, out var _git);
            projRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<Project>
            {
                new() { Id = 1, Name = "A", Path = "c:/a", IDEPathId = 1, SortId = 1 },
                new() { Id = 2, Name = "B", Path = "c:/b", IDEPathId = 1, SortId = 2 },
            });
            devRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<DevApp> { new() { Id = 1, Name = "VS", Path = "c:/vs.exe" } });
            groupRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<Group>());

            var result = await sut.Search("");

            Assert.True(result.EnableAddNewProject);
            var arr = result.Projects.ToArray();
            Assert.False(arr[0].EnableMoveUp);
            Assert.True(arr[0].EnableMoveDown);
            Assert.True(arr[1].EnableMoveUp);
            Assert.False(arr[1].EnableMoveDown);
        }

        [Fact]
        public async Task RemoveProjectFromGroup_Sets_GroupId_Null_And_Edits()
        {
            var sut = CreateSut(out var projRepo, out var _dev, out var _grp, out var _git);
            projRepo.Setup(r => r.GetOne(3)).ReturnsAsync(new Project { Id = 3, GroupId = 10 });
            projRepo.Setup(r => r.Edit(It.IsAny<Project>())).ReturnsAsync(true);

            var ok = await sut.RemoveProjectFromGroup(3);

            Assert.True(ok);
            projRepo.Verify(r => r.Edit(It.Is<Project>(p => p.Id == 3 && p.GroupId == null)), Times.Once);
        }

        [Fact]
        public async Task SortUp_Down_Delegates_To_Repository()
        {
            var sut = CreateSut(out var projRepo, out var _dev, out var _grp, out var _git);
            projRepo.Setup(r => r.SortUp(5)).ReturnsAsync(true);
            projRepo.Setup(r => r.SortDown(6)).ReturnsAsync(true);

            Assert.True(await sut.SortUp(5));
            Assert.True(await sut.SortDown(6));
        }

        [Fact]
        public async Task AddProjectToGroup_Sets_Group_And_Edits()
        {
            var sut = CreateSut(out var projRepo, out var _dev, out var groupRepo, out var _git);
            projRepo.Setup(r => r.GetOne(3)).ReturnsAsync(new Project { Id = 3 });
            groupRepo.Setup(r => r.GetOne(7)).ReturnsAsync(new Group { Id = 7, Name = "G" });
            projRepo.Setup(r => r.Edit(It.IsAny<Project>())).ReturnsAsync(true);

            var ok = await sut.AddProjectToGroup(3, 7);

            Assert.True(ok);
            projRepo.Verify(r => r.Edit(It.Is<Project>(p => p.Id == 3 && p.GroupId == 7)), Times.Once);
        }

        [Fact]
        public async Task GetLast_Maps_Fields()
        {
            var sut = CreateSut(out var projRepo, out var _dev, out var _grp, out var _git);
            projRepo.Setup(r => r.GetLast()).ReturnsAsync(new Project { Id = 9, Name = "Z", Path = "c:/z", IDEPathId = 2, Filename = "z.sln" });

            var vm = await sut.GetLast();

            Assert.Equal(9, vm.Id);
            Assert.Equal("Z", vm.Name);
            Assert.Equal("c:/z", vm.Path);
            Assert.Equal(2, vm.IDEPathId);
            Assert.Equal("z.sln", vm.Filename);
        }
    }
}
