namespace ApplicationCore.Features.Projects;

public class SortUpProjectCommand
{
    public int SortId { get; set; }
}

public interface ISortUpProjectService
{
    Task<bool> HandleAsync(SortUpProjectCommand command);
}

public class SortUpProjectService(IProjectRepository projectRepository) : ISortUpProjectService
{
    private readonly IProjectRepository projectRepository = projectRepository;

    public async Task<bool> HandleAsync(SortUpProjectCommand command)
    {
        return await projectRepository.SortUp(command.SortId);
    }
}
