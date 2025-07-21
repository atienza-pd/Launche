using ApplicationCore;
using ApplicationCore.Common;
using ApplicationCore.Features.DevApps;
using ApplicationCore.Features.Git;
using ApplicationCore.Features.Projects;
using Infrastructure;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using UI.DevApps;
using UI.Features;
using UI.Features.Projects;
using UI.MainWindows;

namespace UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var serviceProvider = GetCurrentServiceProvider();
            var mainWindow = serviceProvider.GetService<MainWindow>();
            var notificationService = serviceProvider.GetService<INotificationMessageService>();
            var startup = serviceProvider.GetService<IStartup>();
            notificationService!.Notify += NotificationService_Notify;
            await startup!.Init();
            mainWindow?.Show();
        }

        private void NotificationService_Notify(object? sender, NotificationMessageEventArgs e)
        {
            var messageBoxIcon = MessageBoxImage.None;

            switch (e.NotificationType)
            {
                case NotificationType.None:
                    messageBoxIcon = MessageBoxImage.None;
                    break;
                case NotificationType.Success:
                    messageBoxIcon = MessageBoxImage.Information;
                    break;
                case NotificationType.Warning:
                    messageBoxIcon = MessageBoxImage.Warning;
                    break;
                case NotificationType.Error:
                    messageBoxIcon = MessageBoxImage.Error;
                    break;
                case NotificationType.Information:
                    messageBoxIcon = MessageBoxImage.Information;
                    break;
            }

            MessageBox.Show(e.Message, e.Title, MessageBoxButton.OK, messageBoxIcon);
        }

        public static ServiceProvider GetCurrentServiceProvider()
        {
            var services = new ServiceCollection();

            return services
                .AddSingleton<MainWindow>()
                .AddTransient<DevAppsWindow>()
                .AddTransient<ProjectsWindow>()
                .AddTransient<DevAppsWindowViewModel>()
                .AddTransient<ProjectsWindowViewModel>()
                .AddSingleton<IAddDevAppService, AddDevAppService>()
                .AddSingleton<IEditDevAppService, EditDevAppService>()
                .AddSingleton<IDeleteDevAppService, DeleteDevAppService>()
                .AddSingleton<IGetAllDevAppService, GetAllDevAppService>()
                .AddSingleton<IGetOneDevAppService, GetOneDevAppService>()
                .AddSingleton<IAddProjectService, AddProjectService>()
                .AddSingleton<IEditProjectService, EditProjectService>()
                .AddSingleton<IDeleteProjectService, DeleteProjectService>()
                .AddSingleton<IGetAllProjectService, GetAllProjectService>()
                .AddSingleton<IInitializedDatabaseMigration, InitializedDatabaseMigration>()
                .AddSingleton<IDevAppFeaturesCreator, DevAppFeaturesCreator>()
                .AddSingleton<INotificationMessageService, NotificationMessageService>()
                .AddSingleton<IProjectFeaturesCreator, ProjectFeaturesCreator>()
                .AddSingleton<IDevAppsSubscriptionService, DevAppsSubscriptionService>()
                .AddSingleton<IProjectWindowEventsService, ProjectWindowEventsService>()
                .AddSingleton<IAddTableSchemaVersion, AddTableSchemaVersion>()
                .AddSingleton<ICheckVersionIfExists, CheckVersionIfExists>()
                .AddSingleton<ICheckVersionTableIfExists, CheckVersionTableIfExists>()
                .AddSingleton<ICreateSqliteConnection, CreateSqliteConnection>()
                .AddSingleton<ICreateVersionsDbTable, CreateVersionsDbTable>()
                .AddSingleton<IInitializedDatabaseMigration, InitializedDatabaseMigration>()
                .AddSingleton<IGitRepository, GitRepository>()
                .AddSingleton<IGroupRepository, GroupRepository>()
                .AddSingleton<IDevAppRepository, DevAppRepository>()
                .AddSingleton<IProjectRepository, ProjectRepository>()
                .BuildServiceProvider();
        }
    }
}
