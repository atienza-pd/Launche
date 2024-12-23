﻿using ApplicationCore.Common;

namespace ApplicationCore.Features.Projects;

public class RemoveProjectFromGroupCommand
{
    public long ProjectId { get; set; }
}

public interface IRemoveProjectFromGroupService
{
    event EventHandler<RemoveProjectFromGroupEventArgs> Notify;
    Task<bool> HandleAsync(RemoveProjectFromGroupCommand command);
}

public class RemoveProjectFromGroupEventArgs(long productId) : EventArgs
{
    public long ProductId { get; } = productId;
}

public class RemoveProjectFromGroupService(
    IProjectRepository projectRepository,
    INotificationMessageService notificationMessageService
) : IRemoveProjectFromGroupService
{
    private readonly IProjectRepository projectRepository = projectRepository;
    private readonly INotificationMessageService notificationMessageService =
        notificationMessageService;
    public event EventHandler<RemoveProjectFromGroupEventArgs>? Notify;

    public async Task<bool> HandleAsync(RemoveProjectFromGroupCommand command)
    {
        var project = await projectRepository.GetOne(command.ProjectId);

        if (project == null)
        {
            notificationMessageService.Create(
                "Project to remove not found!",
                "Remove Project from Group",
                NotificationType.Error
            );
            return false;
        }

        project.GroupId = null;

        var result = await projectRepository.Edit(project);
        if (result)
        {
            notificationMessageService.Create(
                "Project has been remove from group!",
                "Remove Project from Group",
                NotificationType.Error
            );

            this.Notify!.Invoke(this, new(project.Id));
        }

        return true;
    }
}
