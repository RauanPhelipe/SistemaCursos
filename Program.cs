using SistemaCursos.Controllers;
using SistemaCursos.Data;
using SistemaCursos.Views;

try
{
    Database.Initialize();

    var cursoController = new CursoController();
    var matriculaController = new MatriculaController();
    var menu = new MenuView();

    bool continuar = true;

    while (continuar)
    {
        int opcao = menu.ExibirMenuPrincipal();

        switch (opcao)
        {
            case 1:
                cursoController.ExecutarMenu();
                break;
            case 2:
                matriculaController.ExecutarMenu();
                break;
            case 3:
                matriculaController.ListarComNomeDoCurso();
                break;
            case 0:
                continuar = false;
                Console.WriteLine("\nPrograma encerrado. Até mais!");
                break;
            default:
                Console.WriteLine("\nOpção inválida.");
                menu.Pausar();
                break;
        }
    }
}
catch (FileNotFoundException ex)
{
    Console.WriteLine("\nNão foi possível iniciar o sistema.");
    Console.WriteLine($"Arquivo de esquema SQL não encontrado: {ex.FileName}");
    Console.WriteLine("Certifique-se de compilar o projeto para copiar 'Scripts/schema.sql' para a pasta de execução.");
}
catch (Exception ex)
{
    Console.WriteLine("\nErro durante a execução do sistema.");
    Console.WriteLine($"Detalhes: {ex.Message}");
}
