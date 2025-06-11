using System.Data.SQLite;
using System.Reflection;

namespace Infrastructure.Database;

public interface ICreateSqliteConnection : IExecute<SQLiteConnection>;

public class CreateSqliteConnection : ICreateSqliteConnection, IDisposable
{
    private SQLiteConnection _connection;

    public CreateSqliteConnection()
    {
        var fullPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = @$"{fullPath}\file.db";
        var connectionString = $"Data Source={path}";
        if (!System.IO.File.Exists(path))
        {
            Console.WriteLine("Creating Sync DB...");
            SQLiteConnection.CreateFile(path);
        }

        _connection = new SQLiteConnection(connectionString);
        _connection.Open();
    }

    public void Dispose()
    {
        _connection.Close();
        GC.SuppressFinalize(this);
    }

    public SQLiteConnection Execute()
    {
        return _connection;
    }
}
