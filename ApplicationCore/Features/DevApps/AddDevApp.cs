using ApplicationCore.Common;
using Infrastructure.Repositories;

namespace ApplicationCore.Features.DevApps;

public class AddDevAppCommand
{
    public required string Path { get; set; } = string.Empty;
    public string Name { get; set; }
}

public interface IAddDevAppService
{
    Task<bool> HandleAsync(AddDevAppCommand command);
}

public class AddDevAppService(
    IDevAppRepository devAppRepository,
    INotificationMessageService notificationMessageService
) : IAddDevAppService
{
    private readonly IDevAppRepository devAppRepository = devAppRepository;
    private readonly INotificationMessageService notificationMessageService =
        notificationMessageService;

    public async Task<bool> HandleAsync(AddDevAppCommand command)
    {
        return await devAppRepository.Add(new() { Path = command.Path, Name = command.Name });
    }
}
