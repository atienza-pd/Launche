﻿using ApplicationCore.Common;
using Infrastructure.Repositories;

namespace ApplicationCore.Features.DevApps;

public class AddDevAppCommand
{
    public required string Path { get; set; } = string.Empty;
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
        var result = await devAppRepository.Add(new() { Path = command.Path });
        if (result)
        {
            notificationMessageService.Create(
                "New Dev App was save!",
                "Add Dev App",
                NotificationType.Success
            );
        }
        return result;
    }
}
