using Infrastructure.Database;
using Infrastructure.Models;
using System.Data.SQLite;

namespace Infrastructure.Repositories
{
    public interface IDevAppRepository
    {
        Task<bool> Add(DevApp param);
        Task<bool> Edit(DevApp param);
        Task<bool> Delete(long id);
        Task<DevApp> GetById(int id);
        Task<IEnumerable<DevApp>> GetAll();
    }

    public class DevAppRepository(ICreateSqliteConnection createSqliteConnection)
        : IDevAppRepository
    {
        private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

        public async Task<bool> Add(DevApp param)
        {
            var tableName = $"IdePaths";
            var connection = createSqliteConnection.Execute();

            string createTableSql = $"INSERT INTO IdePaths ( Path ) VALUES ( @path );";
            using var command = new SQLiteCommand(createTableSql, connection);
            command.Parameters.AddWithValue("@path", param.Path);
            var rows = await command.ExecuteNonQueryAsync();

            return rows != 0;
        }

        public async Task<bool> Delete(long id)
        {
            var connection = createSqliteConnection.Execute();

            string createTableSql =
                @"
                PRAGMA foreign_keys = ON;
                DELETE FROM IdePaths WHERE Id = @id;";

            using var command = new SQLiteCommand(createTableSql, connection);

            command.Parameters.AddWithValue("@id", id);

            var rows = await command.ExecuteNonQueryAsync();
            return rows != 0;
        }

        public async Task<bool> Edit(DevApp param)
        {
            var tableName = $"IdePaths";
            var connection = createSqliteConnection.Execute();

            string createTableSql = $"UPDATE {tableName} SET  Path=@path WHERE Id = @id;";
            using var command = new SQLiteCommand(createTableSql, connection);
            command.Parameters.AddWithValue("@path", param.Path);
            command.Parameters.AddWithValue("@id", param.Id);
            var rows = await command.ExecuteNonQueryAsync();

            return rows != 0;
        }

        public async Task<IEnumerable<DevApp>> GetAll()
        {
            var tableName = $"IdePaths";
            var iDEPaths = new List<DevApp>();
            var connection = createSqliteConnection.Execute();

            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT * FROM {tableName}";
            using var reader = await command.ExecuteReaderAsync();
            while (reader.Read())
            {
                _ = int.TryParse(reader[nameof(DevApp.Id)]?.ToString(), out int id);
                var path = reader[nameof(DevApp.Path)]?.ToString() ?? "";

                iDEPaths.Add(new() { Id = id, Path = path });
            }

            return iDEPaths;
        }

        public async Task<DevApp> GetById(int id)
        {
            var tableName = $"IdePaths";
            var vsCodePath = new DevApp();
            var connection = createSqliteConnection.Execute();

            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT * FROM {tableName} LIMIT 1";
            using var reader = await command.ExecuteReaderAsync();
            while (reader.Read())
            {
                var path = reader[nameof(Infrastructure.Models.DevApp.Path)]?.ToString() ?? "";

                vsCodePath.Id = id;
                vsCodePath.Path = path;
            }

            return vsCodePath;
        }
    }
}
