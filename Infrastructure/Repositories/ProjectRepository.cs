﻿using System.Data.SQLite;
using Infrastructure.Database;
using Infrastructure.Models;

namespace ApplicationCore.Features.Projects;

public interface IProjectRepository
{
    Task<Project> GetLast();
    Task<Project> GetOne(long id);
    Task<IEnumerable<Project>> GetAll();
    Task<bool> Add(Project param);
    Task<bool> Edit(Project param);
    Task<bool> Delete(long id);
    Task<bool> SortUp(int sortId);
    Task<bool> SortDown(int sortId);
}

public class ProjectRepository(ICreateSqliteConnection createSqliteConnection) : IProjectRepository
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;
    private const string TABLE = $"{nameof(Project)}s";

    public async Task<bool> Add(Project param)
    {
        var connection = createSqliteConnection.Execute();

        string createTableSql =
            @$"
                PRAGMA foreign_keys = ON; 
                INSERT INTO {TABLE}( Path , Name , IDEPathId, SortId,  Filename ) 
                    VALUES ( @path , @name , @idePath , @sortId, @fileName );
                SELECT last_insert_rowid();";

        using var command = new SQLiteCommand(createTableSql, connection);

        command.Parameters.AddWithValue("@path", param?.Path);
        command.Parameters.AddWithValue("@name", param?.Name);
        command.Parameters.AddWithValue("@idePath", param?.IDEPathId);
        command.Parameters.AddWithValue("@fileName", param?.Filename);
        command.Parameters.AddWithValue("@sortId", param?.SortId);

        var rows = await command.ExecuteNonQueryAsync();

        return rows != 0;
    }

    public async Task<bool> Edit(Project param)
    {
        var connection = createSqliteConnection.Execute();

        string createTableSql =
            @$"PRAGMA foreign_keys = ON;
            UPDATE {TABLE} SET  "
            + $"Path = @path, "
            + $"Name = @name, "
            + $"IDEPathId = @idePathId,"
            + $"Filename = @fileName,"
            + $"GroupId = @groupId "
            + $"WHERE Id = @id;";

        using var command = new SQLiteCommand(createTableSql, connection);

        command.Parameters.AddWithValue("@path", param.Path);
        command.Parameters.AddWithValue("@name", param.Name);
        command.Parameters.AddWithValue("@idePathId", param.IDEPathId);
        command.Parameters.AddWithValue("@fileName", param.Filename);
        command.Parameters.AddWithValue("@id", param.Id);
        command.Parameters.AddWithValue("@groupId", param.GroupId);

        var rows = await command.ExecuteNonQueryAsync();

        return rows != 0;
    }

    public async Task<bool> Delete(long id)
    {
        var connection = createSqliteConnection.Execute();

        string createTableSql = $"DELETE FROM {TABLE} WHERE Id = @id;";

        using var command = new SQLiteCommand(createTableSql, connection);

        command.Parameters.AddWithValue("@id", id);

        var rows = await command.ExecuteNonQueryAsync();
        return rows != 0;
    }

    public async Task<Project> GetLast()
    {
        var tableName = $"{nameof(Project)}s";
        var projectPath = new Project();
        var connection = createSqliteConnection.Execute();

        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {nameof(Project)}s ORDER BY Id DESC LIMIT 1;";
        using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            _ = int.TryParse(reader[nameof(Project.Id)]?.ToString(), out int id);
            var path = reader[nameof(Project.Path)]?.ToString() ?? "";
            var name = reader[nameof(Project.Name)]?.ToString() ?? "";
            var idePathId = int.Parse(reader[nameof(Project.IDEPathId)]?.ToString() ?? "0");
            var filename = reader[nameof(Project.Filename)]?.ToString() ?? "";

            projectPath.Id = id;
            projectPath.Path = path;
            projectPath.Name = name;
            projectPath.IDEPathId = idePathId;
            projectPath.Filename = filename;
        }

        return projectPath;
    }

    public async Task<IEnumerable<Project>> GetAll()
    {
        List<Project> projectPaths = [];
        var connection = createSqliteConnection.Execute();

        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {nameof(Project)}s ORDER BY SortId";
        using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            var id = int.Parse(reader[nameof(Project.Id)]?.ToString() ?? "0");
            var path = reader[nameof(Project.Path)]?.ToString() ?? "";
            var name = reader[nameof(Project.Name)]?.ToString() ?? "";
            var idePathId = reader[nameof(Project.IDEPathId)]?.ToString() ?? "";
            var sortId = reader[nameof(Project.SortId)]?.ToString() ?? "";
            var fileName = reader[nameof(Project.Filename)]?.ToString() ?? "";
            var isGroupId = int.TryParse(
                reader[nameof(Project.GroupId)]?.ToString(),
                out int groupId
            );

            projectPaths.Add(
                new()
                {
                    Id = id,
                    Path = path,
                    Name = name,
                    IDEPathId = int.Parse(idePathId),
                    SortId = int.Parse(sortId),
                    Filename = fileName,
                    GroupId = isGroupId ? groupId : null,
                }
            );
        }

        return projectPaths;
    }

    public async Task<Project> GetOne(long id)
    {
        var projectPath = new Project();
        var connection = createSqliteConnection.Execute();

        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {TABLE} WHERE ID = @id;";
        command.Parameters.AddWithValue("@id", id);
        using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            var path = reader[nameof(Project.Path)]?.ToString() ?? "";
            var name = reader[nameof(Project.Name)]?.ToString() ?? "";
            var idePathId = int.Parse(reader[nameof(Project.IDEPathId)]?.ToString() ?? "0");
            var filename = reader[nameof(Project.Filename)]?.ToString() ?? "";
            var isGroupId = int.TryParse(
                reader[nameof(Project.GroupId)]?.ToString(),
                out int groupId
            );

            projectPath.Id = id;
            projectPath.Path = path;
            projectPath.Name = name;
            projectPath.IDEPathId = idePathId;
            projectPath.Filename = filename;
            projectPath.GroupId = isGroupId ? groupId : null;
        }

        return projectPath;
    }

    public async Task<bool> SortUp(int sortId)
    {
        var connection = createSqliteConnection.Execute();

        string createTableSql =
            @$"
                update {TABLE}
                    set SortId = @SortIdTop + @SortIdDown - SortId
                    where SortId in (@SortIdTop, @SortIdDown)";

        using var command = new SQLiteCommand(createTableSql, connection);

        var top = sortId - 1;

        command.Parameters.AddWithValue("@SortIdTop", top);
        command.Parameters.AddWithValue("@SortIdDown", sortId);

        var rows = await command.ExecuteNonQueryAsync();
        return rows != 0;
    }

    public async Task<bool> SortDown(int sortId)
    {
        var connection = createSqliteConnection.Execute();

        string createTableSql =
            @$"
                update {TABLE}
                    set SortId = @SortIdTop + @SortIdDown - SortId
                    where SortId in (@SortIdTop, @SortIdDown)";

        using var command = new SQLiteCommand(createTableSql, connection);

        var top = sortId + 1;

        command.Parameters.AddWithValue("@SortIdTop", top);
        command.Parameters.AddWithValue("@SortIdDown", sortId);

        var rows = await command.ExecuteNonQueryAsync();
        return rows != 0;
    }
}
