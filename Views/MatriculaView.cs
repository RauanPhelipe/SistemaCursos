using SistemaCursos.Models;

namespace SistemaCursos.Views;

public class MatriculaView
{
    public int ExibirMenu()
    {
        EntradaView.LimparTela();
        Console.WriteLine("======= GERENCIAR MATRÍCULAS =======");
        Console.WriteLine("1 - Cadastrar matrícula");
        Console.WriteLine("2 - Listar matrículas");
        Console.WriteLine("3 - Consultar matrícula por ID");
        Console.WriteLine("4 - Atualizar matrícula");
        Console.WriteLine("5 - Excluir matrícula");
        Console.WriteLine("6 - Listar matrículas de um curso");
        Console.WriteLine("0 - Voltar");
        return EntradaView.LerInteiro("\nEscolha uma opção: ", 0);
    }

    public string LerNomeAluno() => EntradaView.LerTexto("Nome do aluno: ");
    public DateTime LerData() => EntradaView.LerData("Data da matrícula (dd/mm/aaaa): ");
    public int LerId() => EntradaView.LerInteiro("Informe o ID da matrícula: ");
    public int LerCursoId() => EntradaView.LerInteiro("Informe o ID do curso: ");

    public void ExibirMatriculas(IEnumerable<Matricula> matriculas)
    {
        var lista = matriculas.ToList();
        Console.WriteLine("\n========== MATRÍCULAS ==========");

        if (lista.Count == 0)
        {
            Console.WriteLine("Nenhuma matrícula encontrada.");
            return;
        }

        foreach (var m in lista)
        {
            Console.WriteLine(
                $"ID: {m.Id} | Aluno: {m.NomeAluno} | " +
                $"Data: {m.DataMatricula:dd/MM/yyyy} | Curso ID: {m.CursoId}");
        }
    }

    public void ExibirMatricula(Matricula matricula)
    {
        Console.WriteLine(
            $"ID: {matricula.Id} | Aluno: {matricula.NomeAluno} | " +
            $"Data: {matricula.DataMatricula:dd/MM/yyyy} | Curso ID: {matricula.CursoId}");
    }

    public void ExibirRelatorio(IEnumerable<Matricula> matriculas)
    {
        var lista = matriculas.ToList();
        Console.WriteLine("\n===== RELATÓRIO: MATRÍCULAS POR CURSO =====");

        if (lista.Count == 0)
        {
            Console.WriteLine("Nenhuma matrícula cadastrada.");
            return;
        }

        foreach (var m in lista)
        {
            string nomeCurso = m.Curso?.Nome ?? $"ID {m.CursoId}";
            Console.WriteLine(
                $"Matrícula: {m.Id} | Aluno: {m.NomeAluno} | " +
                $"Curso: {nomeCurso} | Data: {m.DataMatricula:dd/MM/yyyy}");
        }
    }

    public void Mensagem(string mensagem) => Console.WriteLine(mensagem);

    public void Pausar() => EntradaView.Pausar();
}
