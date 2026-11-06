using SistemaCursos.Models;

namespace SistemaCursos.Views;

public class CursoView
{
    public int ExibirMenu()
    {
        EntradaView.LimparTela();
        Console.WriteLine("========== GERENCIAR CURSOS ==========");
        Console.WriteLine("1 - Cadastrar curso");
        Console.WriteLine("2 - Listar cursos");
        Console.WriteLine("3 - Consultar curso por ID");
        Console.WriteLine("4 - Atualizar curso");
        Console.WriteLine("5 - Excluir curso");
        Console.WriteLine("0 - Voltar");
        return EntradaView.LerInteiro("\nEscolha uma opção: ", 0);
    }

    public Curso LerDadosCurso()
    {
        Console.WriteLine("\n--- Dados do curso ---");
        return new Curso
        {
            Nome = EntradaView.LerTexto("Nome: "),
            CargaHoraria = EntradaView.LerInteiro("Carga horária (horas): "),
            Descricao = EntradaView.LerTexto("Descrição: ")
        };
    }

    public void ExibirCursos(IEnumerable<Curso> cursos)
    {
        var lista = cursos.ToList();
        Console.WriteLine("\n========== CURSOS CADASTRADOS ==========");

        if (lista.Count == 0)
        {
            Console.WriteLine("Nenhum curso cadastrado.");
            return;
        }

        foreach (var curso in lista)
            ExibirCurso(curso);
    }

    public void ExibirCurso(Curso curso)
    {
        Console.WriteLine(
            $"ID: {curso.Id} | Nome: {curso.Nome} | " +
            $"Carga horária: {curso.CargaHoraria}h | Descrição: {curso.Descricao}");
    }

    public int LerId() => EntradaView.LerInteiro("Informe o ID do curso: ");

    public void Mensagem(string mensagem) => Console.WriteLine(mensagem);

    public void Pausar() => EntradaView.Pausar();
}
