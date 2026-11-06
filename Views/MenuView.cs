namespace SistemaCursos.Views;

public class MenuView
{
    public int ExibirMenuPrincipal()
    {
        EntradaView.LimparTela();
        Console.WriteLine("==================================");
        Console.WriteLine(" SISTEMA DE GERENCIAMENTO DE CURSOS");
        Console.WriteLine("==================================");
        Console.WriteLine("1 - Gerenciar cursos");
        Console.WriteLine("2 - Gerenciar matrículas");
        Console.WriteLine("3 - Relatório de matrículas e cursos");
        Console.WriteLine("0 - Sair");
        return EntradaView.LerInteiro("\nEscolha uma opção: ", 0);
    }

    public void Pausar() => EntradaView.Pausar();
}
