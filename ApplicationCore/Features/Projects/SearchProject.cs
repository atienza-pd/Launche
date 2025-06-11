﻿using Infrastructure.Repositories;

namespace ApplicationCore.Features.Projects;

public class SearchProjectViewModel
{
    public IEnumerable<ProjectViewModel> Projects { get; set; } = [];
    public bool EnableAddNewProject { get; internal set; }
}

public class SearchProjectQuery
{
    public string Search { get; set; } = "";
}

public interface ISearchProjectService
{
    Task<SearchProjectViewModel> HandleAsync(SearchProjectQuery query);
}

public class SearchProjectService(
    IProjectRepository projectRepository,
    IGroupRepository groupRepository,
    IDevAppRepository devAppRepository
) : ISearchProjectService
{
    private readonly IProjectRepository projectRepository = projectRepository;
    private readonly IGroupRepository groupRepository = groupRepository;
    private readonly IDevAppRepository devAppRepository = devAppRepository;

    public async Task<SearchProjectViewModel> HandleAsync(SearchProjectQuery query)
    {
        var projects = await projectRepository.GetAll();
        var groups = await groupRepository.GetAll();
        var devApps = await devAppRepository.GetAll();
        var filteredPaths = projects.Where(projectPath =>
            projectPath.Name.ToLower().Contains(query.Search.ToLower())
        );
        SearchProjectViewModel vm = new();

        if (query.Search is not "")
        {
            vm.EnableAddNewProject = false;
            vm.Projects = filteredPaths.Select(
                (project, index) =>
                {
                    return new ProjectViewModel
                    {
                        EnableMoveDown = false,
                        EnableMoveUp = false,
                        Filename = project.Filename,
                        Id = project.Id,
                        IDEPathId = project.IDEPathId,
                        Index = index + 1,
                        Name = project.Name,
                        Path = project.Path,
                        SortId = project.SortId,
                        GroupId = project.GroupId,
                        DevAppPath = devApps.First(devapp => devapp.Id == project.IDEPathId).Path,
                        GroupName = groups
                            .FirstOrDefault(group => group.Id == project.GroupId)
                            ?.Name,
                        EnableAddToGroup = true,
                    };
                }
            );
        }
        else
        {
            vm.EnableAddNewProject = true;
            vm.Projects = filteredPaths.Select(
                (project, index) =>
                {
                    var position = index + 1;
                    return new ProjectViewModel
                    {
                        EnableMoveUp = position != 1,
                        EnableMoveDown = position != projects.Count(),
                        Filename = project.Filename,
                        Id = project.Id,
                        IDEPathId = project.IDEPathId,
                        Index = index + 1,
                        Name = project.Name,
                        Path = project.Path,
                        SortId = project.SortId,
                        GroupId = project.GroupId,
                        DevAppPath = devApps.First(devapp => devapp.Id == project.IDEPathId).Path,
                        GroupName = groups
                            .FirstOrDefault(group => group.Id == project.GroupId)
                            ?.Name,
                        EnableAddToGroup = true,
                    };
                }
            );
        }

        return vm;
    }
}
