namespace ApplicationCore.Features.Projects;

public interface IDeleteProjectService
{
    Task<bool> HandleAsync(long id);
}

public class DeleteProjectService(IProjectRepository projectRepository) : IDeleteProjectService
{
    private readonly IProjectRepository projectRepository = projectRepository;

    public async Task<bool> HandleAsync(long id)
    {
        var projects = await projectRepository.GetAll();

        if (projects.Count() == 1)
        {
            throw new ApplicationException("Single project should exists!");
        }

        return await this.projectRepository.Delete(id);
    }
}
