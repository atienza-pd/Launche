﻿namespace Infrastructure.Database;

public interface ICheckVersionIfExists : IExecute<bool, int>;

public class CheckVersionIfExists(ICreateSqliteConnection createSqliteConnection)
    : ICheckVersionIfExists
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

    public bool Execute(int versionNumber)
    {
        var checkVersionsIfExistsQuery =
            $"SELECT Version FROM Versions WHERE Version='{versionNumber}'";
        var connection = this.createSqliteConnection.Execute();
        using var command = connection.CreateCommand();

        command.CommandText = checkVersionsIfExistsQuery;

        using var reader = command.ExecuteReader();

        return reader.HasRows;
    }
}
