using Infrastructure.Repositories;

namespace ApplicationCore.Features.Projects
{
    public record EditProjectCommand(
        long Id,
        string Name,
        string Path,
        int? IDEPathId,
        string FileName
    );

    public interface IEditProjectService
    {
        Task<bool> HandleAsync(EditProjectCommand param);
    }

    public class EditProjectService(
        IProjectRepository projectRepository,
        IDevAppRepository devAppRepository
    ) : IEditProjectService
    {
        private readonly IProjectRepository projectRepository = projectRepository;

        public async Task<bool> HandleAsync(EditProjectCommand command)
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
            project.Filename = command.FileName;

            return await this.projectRepository.Edit(project);
        }
    }
}
