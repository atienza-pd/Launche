using ApplicationCore.Common;
using ApplicationCore.Features.DevApps;
using ApplicationCore.Features.Git;
using ApplicationCore.Features.Groups;
using ApplicationCore.Features.Projects;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationCore.Features;

public static class CoreServiceCollection
{
    public static IServiceCollection AddApplicationFeatureServiceCollection(
        this IServiceCollection services
    ) =>
        services
            .AddSingleton<IAddDevAppService, AddDevAppService>()
            .AddSingleton<IEditDevAppService, EditDevAppService>()
            .AddSingleton<IDeleteDevAppService, DeleteDevAppService>()
            .AddSingleton<IGetAllDevAppService, GetAllDevAppService>()
            .AddSingleton<IGetOneDevAppService, GetOneDevAppService>()
            .AddSingleton<IAddProjectService, AddProjectService>()
            .AddSingleton<IEditProjectService, EditProjectService>()
            .AddSingleton<IDeleteProjectService, DeleteProjectService>()
            .AddSingleton<IGetAllProjectService, GetAllProjectService>()
            .AddSingleton<IGroupFeaturesCreator, GroupFeaturesCreator>()
            .AddSingleton<IDevAppFeaturesCreator, DevAppFeaturesCreator>()
            .AddSingleton<INotificationMessageService, NotificationMessageService>()
            .AddSingleton<IGitFeaturesCreator, GitFeaturesCreator>()
            .AddSingleton<IProjectFeaturesCreator, ProjectFeaturesCreator>();
}
