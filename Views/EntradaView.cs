using System.Globalization;

namespace SistemaCursos.Views;

public static class EntradaView
{
    private static readonly string[] FormatosData = ["dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd"];

    public static void LimparTela()
    {
        try
        {
            if (!Console.IsInputRedirected && !Console.IsOutputRedirected)
                Console.Clear();
        }
        catch (IOException)
        {
            // Evita crash em ambientes com entrada/saída redirecionada ou sem console anexado
        }
    }

    public static void Pausar()
    {
        Console.WriteLine("\nPressione ENTER para continuar...");
        Console.ReadLine();
    }

    public static string LerTexto(string mensagem)
    {
        while (true)
        {
            Console.Write(mensagem);
            string? valor = Console.ReadLine();

            if (valor is null)
                throw new EndOfStreamException("Entrada de dados encerrada (EOF).");

            if (!string.IsNullOrWhiteSpace(valor))
                return valor.Trim();

            Console.WriteLine("O campo não pode ficar vazio.");
        }
    }

    public static int LerInteiro(string mensagem, int minimo = 1)
    {
        while (true)
        {
            Console.Write(mensagem);
            string? entrada = Console.ReadLine();

            if (entrada is null)
                throw new EndOfStreamException("Entrada de dados encerrada (EOF).");

            if (int.TryParse(entrada, out int valor) && valor >= minimo)
                return valor;

            Console.WriteLine($"Digite um número inteiro maior ou igual a {minimo}.");
        }
    }

    public static DateTime LerData(string mensagem)
    {
        while (true)
        {
            Console.Write(mensagem);
            string? entrada = Console.ReadLine();

            if (entrada is null)
                throw new EndOfStreamException("Entrada de dados encerrada (EOF).");

            if (DateTime.TryParseExact(entrada?.Trim(), FormatosData, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out DateTime data) && data.Date <= DateTime.Today)
            {
                return data.Date;
            }

            Console.WriteLine($"Data inválida. Use uma data válida, não futura no formato dd/mm/aaaa (ex.: {DateTime.Today:dd/MM/yyyy}).");
        }
    }

    public static bool Confirmar(string mensagem)
    {
        Console.Write($"{mensagem} (S/N): ");
        return string.Equals(Console.ReadLine()?.Trim(), "S",
            StringComparison.OrdinalIgnoreCase);
    }
}
