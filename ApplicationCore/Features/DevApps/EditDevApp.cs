using Infrastructure.Repositories;

namespace ApplicationCore.Features.DevApps;

public class EditDevAppCommand
{
    public required string Path { get; set; } = string.Empty;
    public required int Id { get; set; }
    public string Name { get; set; }
}

public interface IEditDevAppService
{
    Task<bool> HandleAsync(EditDevAppCommand command);
}

public class EditDevAppService(IDevAppRepository devAppRepository) : IEditDevAppService
{
    private readonly IDevAppRepository devAppRepository = devAppRepository;

    public async Task<bool> HandleAsync(EditDevAppCommand command)
    {
        return await devAppRepository.Edit(new() { Path = command.Path, Id = command.Id, Name = command.Name });
    }
}
