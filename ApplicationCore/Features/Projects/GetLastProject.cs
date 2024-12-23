namespace ApplicationCore.Features.Projects;

public interface IGetLastProjectService
{
    Task<ProjectViewModel> HandleAsync();
}

public class GetLastProject(IProjectRepository projectRepository) : IGetLastProjectService
{
    private readonly IProjectRepository projectRepository = projectRepository;

    public async Task<ProjectViewModel> HandleAsync()
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
}
