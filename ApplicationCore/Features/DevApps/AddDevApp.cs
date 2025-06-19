using ApplicationCore.Common;
using Infrastructure.Repositories;

namespace ApplicationCore.Features.DevApps;

public class AddDevAppCommand
{
    public required string Path { get; set; } = string.Empty;
    public required string Name { get; set; }
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
        ArgumentNullException.ThrowIfNull(command);

        if (string.IsNullOrEmpty(command.Name))
        {
            throw new ApplicationException($"{nameof(command.Name)} is required!");
        }

        if (string.IsNullOrEmpty(command.Path))
        {
            throw new ApplicationException($"{nameof(command.Path)} is required!");
        }

        if (!command.Path.Contains(".exe", StringComparison.CurrentCultureIgnoreCase))
        {
            throw new ApplicationException($"{nameof(command.Path)} must be executable file!");
        }

        return await devAppRepository.Add(new() { Path = command.Path, Name = command.Name });
    }
}
