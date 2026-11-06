using Microsoft.Data.Sqlite;
using SistemaCursos.Data;
using SistemaCursos.Models;
using SistemaCursos.Views;

namespace SistemaCursos.Controllers;

public class CursoController
{
    private readonly CursoRepository _repository = new();
    private readonly MatriculaRepository _matriculaRepository = new();
    private readonly CursoView _view = new();

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
                    case 0: voltar = true; break;
                    default: _view.Mensagem("Opção inválida."); break;
                }
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                if (ex.SqliteExtendedErrorCode is 1811 or 787)
                    _view.Mensagem("Não é possível concluir: existem registros vinculados a este curso no banco de dados.");
                else if (ex.SqliteExtendedErrorCode == 2067)
                    _view.Mensagem("Operação cancelada: já existe um curso cadastrado com este nome.");
                else if (ex.SqliteExtendedErrorCode == 275)
                    _view.Mensagem("Operação cancelada: a carga horária deve ser maior que zero.");
                else
                    _view.Mensagem("Não foi possível concluir. Os dados violam restrições do banco de dados.");
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
        Curso curso = _view.LerDadosCurso();
        _repository.Inserir(curso);
        _view.Mensagem("Curso cadastrado com sucesso!");
    }

    private void Listar() => _view.ExibirCursos(_repository.Listar());

    private void Consultar()
    {
        int id = _view.LerId();
        Curso? curso = _repository.BuscarPorId(id);

        if (curso is null)
        {
            _view.Mensagem("Curso não encontrado.");
            return;
        }

        curso.Matriculas = _matriculaRepository.ListarPorCurso(id);
        _view.ExibirCurso(curso);
        if (curso.Matriculas.Count > 0)
        {
            Console.WriteLine($"   -> Matrículas ativas neste curso: {curso.Matriculas.Count}");
        }
    }

    private void Atualizar()
    {
        int id = _view.LerId();
        Curso? existente = _repository.BuscarPorId(id);

        if (existente is null)
        {
            _view.Mensagem("Curso não encontrado.");
            return;
        }

        _view.Mensagem("Digite os novos dados do curso:");
        Curso atualizado = _view.LerDadosCurso();
        atualizado.Id = id;

        _view.Mensagem(_repository.Atualizar(atualizado)
            ? "Curso atualizado com sucesso."
            : "Nenhuma alteração realizada.");
    }

    private void Excluir()
    {
        int id = _view.LerId();

        if (_repository.BuscarPorId(id) is null)
        {
            _view.Mensagem("Curso não encontrado.");
            return;
        }

        int qtdMatriculas = _matriculaRepository.ListarPorCurso(id).Count;
        if (qtdMatriculas > 0)
        {
            _view.Mensagem($"Não é possível excluir o curso: existem {qtdMatriculas} matrícula(s) vinculada(s) a ele.");
            return;
        }

        if (!EntradaView.Confirmar("Confirma a exclusão do curso?"))
        {
            _view.Mensagem("Exclusão cancelada.");
            return;
        }

        _view.Mensagem(_repository.Excluir(id)
            ? "Curso excluído com sucesso."
            : "Não foi possível excluir o curso.");
    }
}
