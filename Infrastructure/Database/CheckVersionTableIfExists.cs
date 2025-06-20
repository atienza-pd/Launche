﻿namespace Infrastructure.Database;

public interface ICheckVersionTableIfExists : IExecute<bool>;

public class CheckVersionTableIfExists(ICreateSqliteConnection createSqliteConnection)
    : ICheckVersionTableIfExists
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

    public bool Execute()
    {
        var checkVersionsTableIfExistsQuery =
            "SELECT name FROM sqlite_master WHERE type='table' AND name='Versions'";
        var connection = this.createSqliteConnection.Execute();
        using var command = connection.CreateCommand();

        command.CommandText = checkVersionsTableIfExistsQuery;

        using var reader = command.ExecuteReader();

        return reader.HasRows;
    }
}
