namespace SistemaCursos.Models;

public class Curso
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int CargaHoraria { get; set; }
    public string Descricao { get; set; } = string.Empty;

    // Relacionamento 1:N no modelo de classes: um curso pode conter várias matrículas associadas
    public List<Matricula> Matriculas { get; set; } = [];
}
