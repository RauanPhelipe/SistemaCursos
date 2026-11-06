namespace SistemaCursos.Models;

public class Matricula
{
    public int Id { get; set; }
    public string NomeAluno { get; set; } = string.Empty;
    public DateTime DataMatricula { get; set; }
    public int CursoId { get; set; }

    // Referência à entidade relacionada: uma matrícula pertence a um curso.
    public Curso? Curso { get; set; }
}
