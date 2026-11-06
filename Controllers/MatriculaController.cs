using Microsoft.Data.Sqlite;
using SistemaCursos.Data;
using SistemaCursos.Models;
using SistemaCursos.Views;

namespace SistemaCursos.Controllers;

public class MatriculaController
{
    private readonly MatriculaRepository _repository = new();
    private readonly CursoRepository _cursoRepository = new();
    private readonly MatriculaView _view = new();
    private readonly CursoView _cursoView = new();

    public void ExecutarMenu()
    {
        bool voltar = false;

        while (!voltar)
        {
            int opcao = _view.ExibirMenu();

            try
            {
                switch (opcao)
                {
                    case 1: Cadastrar(); break;
                    case 2: Listar(); break;
                    case 3: Consultar(); break;
                    case 4: Atualizar(); break;
                    case 5: Excluir(); break;
                    case 6: ListarPorCurso(); break;
                    case 0: voltar = true; break;
                    default: _view.Mensagem("Opção inválida."); break;
                }
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                if (ex.SqliteExtendedErrorCode is 1811 or 787)
                    _view.Mensagem("Operação inválida: o curso informado não existe no banco de dados.");
                else
                    _view.Mensagem("Operação inválida: os dados informados desrespeitam restrições do banco.");
            }
            catch (Exception ex)
            {
                _view.Mensagem($"Erro na operação: {ex.Message}");
            }

            if (!voltar) _view.Pausar();
        }
    }

    private void Cadastrar()
    {
        var cursos = _cursoRepository.Listar();
        if (cursos.Count == 0)
        {
            _view.Mensagem("Cadastre pelo menos um curso antes de realizar matrículas.");
            return;
        }

        _cursoView.ExibirCursos(cursos);
        string nome = _view.LerNomeAluno();
        DateTime data = _view.LerData();
        int cursoId = _view.LerCursoId();

        if (_cursoRepository.BuscarPorId(cursoId) is null)
        {
            _view.Mensagem("O curso informado não existe. Matrícula não cadastrada.");
            return;
        }

        _repository.Inserir(new Matricula
        {
            NomeAluno = nome,
            DataMatricula = data,
            CursoId = cursoId
        });

        _view.Mensagem("Matrícula cadastrada com sucesso!");
    }

    private void Listar() => _view.ExibirMatriculas(_repository.Listar());

    private void Consultar()
    {
        Matricula? matricula = _repository.BuscarPorId(_view.LerId());

        if (matricula is null)
        {
            _view.Mensagem("Matrícula não encontrada.");
            return;
        }

        matricula.Curso = _cursoRepository.BuscarPorId(matricula.CursoId);
        _view.ExibirMatricula(matricula);
        if (matricula.Curso is not null)
        {
            Console.WriteLine($"   -> Curso associado: {matricula.Curso.Nome} ({matricula.Curso.CargaHoraria}h)");
        }
    }

    private void Atualizar()
    {
        int id = _view.LerId();
        Matricula? existente = _repository.BuscarPorId(id);

        if (existente is null)
        {
            _view.Mensagem("Matrícula não encontrada.");
            return;
        }

        _view.Mensagem("Digite os novos dados da matrícula:");
        string nome = _view.LerNomeAluno();
        DateTime data = _view.LerData();

        _cursoView.ExibirCursos(_cursoRepository.Listar());
        int cursoId = _view.LerCursoId();

        if (_cursoRepository.BuscarPorId(cursoId) is null)
        {
            _view.Mensagem("O curso informado não existe. Atualização cancelada.");
            return;
        }

        bool atualizado = _repository.Atualizar(new Matricula
        {
            Id = id,
            NomeAluno = nome,
            DataMatricula = data,
            CursoId = cursoId
        });

        _view.Mensagem(atualizado
            ? "Matrícula atualizada com sucesso."
            : "Nenhuma alteração realizada.");
    }

    private void Excluir()
    {
        int id = _view.LerId();

        if (_repository.BuscarPorId(id) is null)
        {
            _view.Mensagem("Matrícula não encontrada.");
            return;
        }

        if (!EntradaView.Confirmar("Confirma a exclusão da matrícula?"))
        {
            _view.Mensagem("Exclusão cancelada.");
            return;
        }

        _view.Mensagem(_repository.Excluir(id)
            ? "Matrícula excluída com sucesso."
            : "Não foi possível excluir a matrícula.");
    }

    private void ListarPorCurso()
    {
        _cursoView.ExibirCursos(_cursoRepository.Listar());
        int cursoId = _view.LerCursoId();

        if (_cursoRepository.BuscarPorId(cursoId) is null)
        {
            _view.Mensagem("Curso não encontrado.");
            return;
        }

        _view.ExibirMatriculas(_repository.ListarPorCurso(cursoId));
    }

    public void ListarComNomeDoCurso()
    {
        try
        {
            _view.ExibirRelatorio(_repository.ListarComNomeDoCurso());
        }
        catch (Exception ex)
        {
            _view.Mensagem($"Erro ao gerar relatório: {ex.Message}");
        }

        _view.Pausar();
    }
}
