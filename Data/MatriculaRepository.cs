using System.Globalization;
using Microsoft.Data.Sqlite;
using SistemaCursos.Models;

namespace SistemaCursos.Data;

public class MatriculaRepository
{
    public void Inserir(Matricula matricula)
    {
        using var connection = Database.GetConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Matriculas (NomeAluno, DataMatricula, CursoId)
            VALUES ($nomeAluno, $dataMatricula, $cursoId);
            SELECT last_insert_rowid();
            """;
        command.Parameters.AddWithValue("$nomeAluno", matricula.NomeAluno);
        command.Parameters.AddWithValue(
            "$dataMatricula", matricula.DataMatricula.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$cursoId", matricula.CursoId);
        matricula.Id = Convert.ToInt32(command.ExecuteScalar());
    }

    public List<Matricula> Listar()
    {
        var matriculas = new List<Matricula>();

        using var connection = Database.GetConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, NomeAluno, DataMatricula, CursoId
            FROM Matriculas
            ORDER BY Id;
            """;

        using var reader = command.ExecuteReader();
        while (reader.Read())
            matriculas.Add(Mapear(reader));

        return matriculas;
    }

    public Matricula? BuscarPorId(int id)
    {
        using var connection = Database.GetConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, NomeAluno, DataMatricula, CursoId
            FROM Matriculas
            WHERE Id = $id;
            """;
        command.Parameters.AddWithValue("$id", id);

        using var reader = command.ExecuteReader();
        return reader.Read() ? Mapear(reader) : null;
    }

    public bool Atualizar(Matricula matricula)
    {
        using var connection = Database.GetConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE Matriculas
            SET NomeAluno = $nomeAluno,
                DataMatricula = $dataMatricula,
                CursoId = $cursoId
            WHERE Id = $id;
            """;
        command.Parameters.AddWithValue("$nomeAluno", matricula.NomeAluno);
        command.Parameters.AddWithValue(
            "$dataMatricula", matricula.DataMatricula.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$cursoId", matricula.CursoId);
        command.Parameters.AddWithValue("$id", matricula.Id);

        return command.ExecuteNonQuery() > 0;
    }

    public bool Excluir(int id)
    {
        using var connection = Database.GetConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Matriculas WHERE Id = $id;";
        command.Parameters.AddWithValue("$id", id);

        return command.ExecuteNonQuery() > 0;
    }

    public List<Matricula> ListarPorCurso(int cursoId)
    {
        var matriculas = new List<Matricula>();

        using var connection = Database.GetConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, NomeAluno, DataMatricula, CursoId
            FROM Matriculas
            WHERE CursoId = $cursoId
            ORDER BY NomeAluno;
            """;
        command.Parameters.AddWithValue("$cursoId", cursoId);

        using var reader = command.ExecuteReader();
        while (reader.Read())
            matriculas.Add(Mapear(reader));

        return matriculas;
    }

    public List<Matricula> ListarComNomeDoCurso()
    {
        var resultado = new List<Matricula>();

        using var connection = Database.GetConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT
                m.Id,
                m.NomeAluno,
                m.DataMatricula,
                m.CursoId,
                c.Nome AS NomeCurso
            FROM Matriculas m
            INNER JOIN Cursos c ON c.Id = m.CursoId
            ORDER BY c.Nome, m.NomeAluno;
            """;

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var matricula = Mapear(reader);
            matricula.Curso = new Curso
            {
                Id = matricula.CursoId,
                Nome = reader.GetString(reader.GetOrdinal("NomeCurso"))
            };
            resultado.Add(matricula);
        }

        return resultado;
    }

    private static Matricula Mapear(SqliteDataReader reader)
    {
        return new Matricula
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            NomeAluno = reader.GetString(reader.GetOrdinal("NomeAluno")),
            DataMatricula = DateTime.ParseExact(
                reader.GetString(reader.GetOrdinal("DataMatricula")),
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture),
            CursoId = reader.GetInt32(reader.GetOrdinal("CursoId"))
        };
    }
}
