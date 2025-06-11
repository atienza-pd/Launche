using System.Data.SQLite;

namespace Infrastructure.Database;

public interface IAddTableSchemaVersion : IExecute<int, int>;

public class AddTableSchemaVersion(ICreateSqliteConnection createSqliteConnection)
    : IAddTableSchemaVersion
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

    public int Execute(int number)
    {
        var connection = this.createSqliteConnection.Execute();

        string createTableSql = $"INSERT INTO Versions (Version) VALUES ({number})";
        using var command = new SQLiteCommand(createTableSql, connection);
        var rows = command.ExecuteNonQuery();

        return rows;
    }
}
