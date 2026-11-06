using Microsoft.Data.Sqlite;
using SistemaCursos.Models;

namespace SistemaCursos.Data;

public class CursoRepository
{
    public void Inserir(Curso curso)
    {
        using var connection = Database.GetConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Cursos (Nome, CargaHoraria, Descricao)
            VALUES ($nome, $cargaHoraria, $descricao);
            SELECT last_insert_rowid();
            """;
        command.Parameters.AddWithValue("$nome", curso.Nome);
        command.Parameters.AddWithValue("$cargaHoraria", curso.CargaHoraria);
        command.Parameters.AddWithValue("$descricao", curso.Descricao);
        curso.Id = Convert.ToInt32(command.ExecuteScalar());
    }

    public List<Curso> Listar()
    {
        var cursos = new List<Curso>();

        using var connection = Database.GetConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, Nome, CargaHoraria, Descricao
            FROM Cursos
            ORDER BY Nome;
            """;

        using var reader = command.ExecuteReader();
        while (reader.Read())
            cursos.Add(Mapear(reader));

        return cursos;
    }

    public Curso? BuscarPorId(int id)
    {
        using var connection = Database.GetConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, Nome, CargaHoraria, Descricao
            FROM Cursos
            WHERE Id = $id;
            """;
        command.Parameters.AddWithValue("$id", id);

        using var reader = command.ExecuteReader();
        return reader.Read() ? Mapear(reader) : null;
    }

    public bool Atualizar(Curso curso)
    {
        using var connection = Database.GetConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE Cursos
            SET Nome = $nome,
                CargaHoraria = $cargaHoraria,
                Descricao = $descricao
            WHERE Id = $id;
            """;
        command.Parameters.AddWithValue("$nome", curso.Nome);
        command.Parameters.AddWithValue("$cargaHoraria", curso.CargaHoraria);
        command.Parameters.AddWithValue("$descricao", curso.Descricao);
        command.Parameters.AddWithValue("$id", curso.Id);

        return command.ExecuteNonQuery() > 0;
    }

    public bool Excluir(int id)
    {
        using var connection = Database.GetConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Cursos WHERE Id = $id;";
        command.Parameters.AddWithValue("$id", id);

        return command.ExecuteNonQuery() > 0;
    }

    private static Curso Mapear(SqliteDataReader reader)
    {
        return new Curso
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Nome = reader.GetString(reader.GetOrdinal("Nome")),
            CargaHoraria = reader.GetInt32(reader.GetOrdinal("CargaHoraria")),
            Descricao = reader.GetString(reader.GetOrdinal("Descricao"))
        };
    }
}
