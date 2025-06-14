﻿using ApplicationCore.Common;
using Infrastructure.Repositories;

namespace ApplicationCore.Features.Projects;

public class AddProjectToGroupCommand
{
    public long ProjectId { get; set; }
    public long GroupId { get; set; }
}

public interface IAddProjectToGroupService
{
    event EventHandler Notify;
    Task<bool> HandleAsync(AddProjectToGroupCommand command);
}

public class AddProjectToGroupService(
    IGroupRepository groupRepository,
    IProjectRepository projectRepository,
    INotificationMessageService notificationMessageService
) : IAddProjectToGroupService
{
    private readonly IProjectRepository projectRepository = projectRepository;

    public event EventHandler? Notify;

    public async Task<bool> HandleAsync(AddProjectToGroupCommand command)
    {
        var project = await projectRepository.GetOne(command.ProjectId);
        var group = await groupRepository.GetOne(command.GroupId);
        if (project == null)
        {
            notificationMessageService.Create(
                "Project not found when adding to a group!",
                "Add Project to Group",
                NotificationType.Error
            );
            return false;
        }

        if (group == null)
        {
            notificationMessageService.Create(
                "Project not found when adding to a group!",
                "Add Project to Group",
                NotificationType.Error
            );
            return false;
        }

        project.GroupId = command.GroupId;

        var result = await projectRepository.Edit(project);

        if (result)
        {
            notificationMessageService.Create(
                "Project has been added to group!",
                "Add Project to Group",
                NotificationType.Success
            );

            this.Notify!.Invoke(this, EventArgs.Empty);
        }

        return result;
    }
}
