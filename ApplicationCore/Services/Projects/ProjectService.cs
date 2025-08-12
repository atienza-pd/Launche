using ApplicationCore.Features.DevApps;
using ApplicationCore.Features.Git;
using ApplicationCore.Features.Groups;

namespace ApplicationCore.Features.Projects;

public interface IProjectService
{
    Task<ProjectViewModel> GetLast();
    Task<ProjectViewModel> GetOne(long id);
    Task<GetAllProjectViewModel> GetAll();
    Task<bool> Add(Project param);
    Task<bool> Edit(Project param);
    Task<bool> Delete(long id);
    Task<bool> SortUp(int sortId);
    Task<bool> SortDown(int sortId);
    Task<bool> RemoveProjectFromGroup(long projectId);
    Task<bool> AddProjectToGroup(long projectId, int groupId);
    Task<SearchProjectViewModel> Search(string search);

}

public class ProjectService(
    IProjectRepository projectRepository,
    IDevAppRepository devAppRepository,
    IGroupRepository groupRepository,
    IGitService gitService) : IProjectService
{
    private readonly IProjectRepository projectRepository = projectRepository;
    private readonly IDevAppRepository devAppRepository = devAppRepository;
    private readonly IGroupRepository groupRepository = groupRepository;
    private readonly IGitService gitService = gitService;

    public async Task<bool> Add(Project project)
    {
        ArgumentNullException.ThrowIfNull(project);

        int defaultDevAppId = project.IDEPathId ?? 0;

        if (string.IsNullOrWhiteSpace(project.Name))
        {
            throw new ApplicationException("Project Name must be provided");
        }

        if (string.IsNullOrWhiteSpace(project.Path))
        {
            throw new ApplicationException("Project Path must be provided");
        }

        if (project.IDEPathId is null)
        {
            var devApps = await devAppRepository.GetAll();
            defaultDevAppId = devApps.First().Id;
        }

        var projects = await projectRepository.GetAll();
        var lastSortId = projects.LastOrDefault()?.SortId ?? 0;

        return await this.projectRepository.Add(
            new()
            {
                Name = project.Name,
                Path = project.Path,
                IDEPathId = defaultDevAppId,
                Filename = project.Filename,
                SortId = lastSortId + 1,
            }
        );
    }

    public async Task<SearchProjectViewModel> Search(string criteria)
    {
        var projects = await projectRepository.GetAll();
        var groups = await groupRepository.GetAll();
        var devApps = await devAppRepository.GetAll();
        var filteredPaths = projects.Where(projectPath =>
            projectPath.Name.Contains(criteria, StringComparison.CurrentCultureIgnoreCase)
        );
        SearchProjectViewModel vm = new();

        if (criteria is not "")
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
                        EnableAddToGroup = false,
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

    public async Task<bool> Delete(long id)
    {
        var projects = await projectRepository.GetAll();

        if (projects.Count() == 1)
        {
            throw new ApplicationException("Single project should exists!");
        }

        return await this.projectRepository.Delete(id);
    }

    public async Task<bool> Edit(Project command)
    {
        int defaultDevAppId = command.IDEPathId ?? 0;

        var project = await projectRepository.GetOne(command.Id);

        ArgumentNullException.ThrowIfNull(project);

        if (command.Name is null || command.Name is "")
        {
            throw new ApplicationException("Project Name must be provided");

        }

        if (command.Path is null || command.Path is "")
        {
            throw new ApplicationException("Project Path must be provided");
        }

        if (command.IDEPathId is 0)
        {
            var devApps = await devAppRepository.GetAll();
            defaultDevAppId = devApps.First().Id;
        }

        project.Name = command.Name;
        project.Path = command.Path;
        project.IDEPathId = defaultDevAppId;
        project.Filename = command.Filename;

        return await this.projectRepository.Edit(project);
    }

    public async Task<GetAllProjectViewModel> GetAll()
    {
        var projects = await this.projectRepository.GetAll();
        var devApps = await this.devAppRepository.GetAll();
        var groups = await this.groupRepository.GetAll();
        return new()
        {
            Projects = projects.Select(
                (value, index) =>
                {
                    var position = index + 1;
                    return new ProjectViewModel
                    {
                        Id = value.Id,
                        Name = value.Name,
                        Path = value.Path,
                        IDEPathId = value.IDEPathId ?? 0,
                        SortId = value.SortId,
                        Filename = value.Filename,
                        GroupId = value.GroupId,
                        Index = position,
                        EnableMoveUp = position != 1,
                        EnableMoveDown = position != projects.Count(),
                        EnableAddToGroup = false,
                        GroupName = groups.FirstOrDefault(group => group.Id == value.GroupId)?.Name,
                        DevAppPath = devApps.First(devApp => devApp.Id == value.IDEPathId).Path,
                        DevAppName = devApps.First(devApp => devApp.Id == value.IDEPathId).Name,
                    };
                }
            ),
        };
    }

    public async Task<ProjectViewModel> GetLast()
    {
        var project = await this.projectRepository.GetLast();

        return new()
        {
            Id = project.Id,
            Name = project.Name,
            Path = project.Path,
            IDEPathId = project.IDEPathId,
            Filename = project.Filename,
        };
    }

    public async Task<ProjectViewModel> GetOne(long id)
    {
        var project = await this.projectRepository.GetOne(id);
        var devApp = await this.devAppRepository.GetById(project.IDEPathId ?? 0);
        var git = this.GetCurrentGitBranch(project.Path);

        return new()
        {
            Id = project.Id,
            Name = project.Name,
            Path = project.Path,
            IDEPathId = project.IDEPathId,
            Filename = project.Filename,
            DevAppName = devApp.Name,
            CurrentGitBranch = git,
            DevAppPath = devApp.Path,
            SortId = project.SortId,
        };
    }

    private string GetCurrentGitBranch(string path)
    {
        try
        {
            return gitService.GetCurrentBranch(path);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public async Task<bool> RemoveProjectFromGroup(long projectId)
    {
        var project = await projectRepository.GetOne(projectId);

        ArgumentNullException.ThrowIfNull(project);

        project.GroupId = null;

        return await projectRepository.Edit(project);
    }

    public async Task<bool> SortDown(int sortId)
    {
        return await projectRepository.SortDown(sortId);
    }

    public async Task<bool> SortUp(int sortId)
    {
        return await projectRepository.SortUp(sortId);
    }

    public async Task<bool> AddProjectToGroup(long projectId, int groupId)
    {
        var project = await projectRepository.GetOne(projectId);
        var group = await groupRepository.GetOne(groupId);

        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(group);


        project.GroupId = groupId;

        return await projectRepository.Edit(project);
    }
}

public class GetAllProjectViewModel
{
    public IEnumerable<ProjectViewModel> Projects { get; init; } = [];
    public int Count
    {
        get { return this.Projects.Count(); }
    }
};

public class SearchProjectViewModel
{
    public IEnumerable<ProjectViewModel> Projects { get; set; } = [];
    public bool EnableAddNewProject { get; internal set; }
}
