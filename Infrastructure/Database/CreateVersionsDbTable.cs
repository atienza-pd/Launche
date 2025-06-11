using System.Data.SQLite;

namespace Infrastructure.Database;

public interface ICreateVersionsDbTable : IExecute;

public class CreateVersionsDbTable(ICreateSqliteConnection createSqliteConnection)
    : ICreateVersionsDbTable
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

    public void Execute()
    {
        var connection = this.createSqliteConnection.Execute();

        string createTableSql =
            @"CREATE TABLE Versions (Id INTEGER PRIMARY KEY AUTOINCREMENT, Version TEXT)";
        using var command = new SQLiteCommand(createTableSql, connection);

        command.ExecuteNonQuery();
    }
}
