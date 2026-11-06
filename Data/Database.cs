using Microsoft.Data.Sqlite;

namespace SistemaCursos.Data;

public static class Database
{
    private static readonly string DatabasePath =
        Path.Combine(AppContext.BaseDirectory, "cursos.db");

    private static readonly string ConnectionString =
        new SqliteConnectionStringBuilder
        {
            DataSource = DatabasePath,
            ForeignKeys = true
        }.ToString();

    public static SqliteConnection GetConnection()
    {
        return new SqliteConnection(ConnectionString);
    }

    public static void Initialize()
    {
        string scriptPath = Path.Combine(
            AppContext.BaseDirectory, "Scripts", "schema.sql");

        if (!File.Exists(scriptPath))
            throw new FileNotFoundException("Script SQL não encontrado.", scriptPath);

        string sql = File.ReadAllText(scriptPath);

        using var connection = GetConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }
}
