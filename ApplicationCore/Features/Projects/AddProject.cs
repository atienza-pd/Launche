using Infrastructure.Repositories;

namespace ApplicationCore.Features.Projects
{
    public record AddProjectCommand(string Name, string Path, int? IDEPathId, string FileName);

    public interface IAddProjectService
    {
        Task<bool> HandleAsync(AddProjectCommand command);
    }

    public class AddProjectService(
        IProjectRepository projectRepository,
        IDevAppRepository devAppRepository
    ) : IAddProjectService
    {
        private readonly IProjectRepository projectRepository = projectRepository;
        private readonly IDevAppRepository devAppRepository = devAppRepository;

        public async Task<bool> HandleAsync(AddProjectCommand command)
        {
            ArgumentNullException.ThrowIfNull(command);

            int defaultDevAppId = command.IDEPathId ?? 0;

            if (string.IsNullOrWhiteSpace(command.Name))
            {
                throw new ApplicationException("Project Name must be provided");
            }

            if (string.IsNullOrWhiteSpace(command.Path))
            {
                throw new ApplicationException("Project Path must be provided");
            }

            if (command.IDEPathId is null)
            {
                var devApps = await devAppRepository.GetAll();
                defaultDevAppId = devApps.First().Id;
            }

            var projects = await projectRepository.GetAll();
            var lastSortId = projects.LastOrDefault()?.SortId ?? 0;

            return await this.projectRepository.Add(
                new()
                {
                    Name = command.Name,
                    Path = command.Path,
                    IDEPathId = defaultDevAppId,
                    Filename = command.FileName,
                    SortId = lastSortId + 1,
                }
            );
        }
    }
}
