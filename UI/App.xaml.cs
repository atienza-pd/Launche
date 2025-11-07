using ApplicationCore.Common;
using ApplicationCore.Features.DevApps;
using ApplicationCore.Features.Git;
using ApplicationCore.Features.Groups;
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
using UI.Shared.Services;

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
            var migration = serviceProvider.GetService<IInitializedDatabaseMigration>();
            notificationService!.Notify += NotificationService_Notify;
            await migration!.Execute();
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
                .AddTransient<IProjectsWindow, ProjectsWindow>()
                .AddTransient<DevAppsWindowViewModel>()
                .AddTransient<ProjectsWindowViewModel>()
                .AddTransient<MainWindowViewModel>()
                .AddSingleton<IInitializedDatabaseMigration, InitializedDatabaseMigration>()

                .AddSingleton<INotificationMessageService, NotificationMessageService>()

                .AddSingleton<IDevAppsEventsService, DevAppsEventsService>()
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
                .AddSingleton<IProjectService, ProjectService>()
                .AddSingleton<IDevAppService, DevAppService>()
                .AddSingleton<IGitService, GitService>()
                .AddSingleton<IGroupService, GroupService>()

                .AddSingleton<ISelectedProjectService, SelectedProjectService>()
                .BuildServiceProvider();
        }
    }
}
