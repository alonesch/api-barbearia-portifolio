using BarbeariaPortifolio.API.Modules.Barbeiros.Models;

namespace BarbeariaPortifolio.API.Modules.Barbeiros.Repositories.Interfaces;

public interface IBarbeiroRepositorio
{
    Task<IEnumerable<Barbeiro>> ListarTodos();
    Task<Barbeiro?> BuscarPorId(int id);
    Task<Barbeiro?> BuscarUsuarioId(int id);
    Task<Barbeiro?> BuscarPorNome(string nome);
    Task<Barbeiro> Cadastrar(Barbeiro barbeiro);
    Task<bool> Atualizar(int id, Barbeiro barbeiro);
    Task<bool> Excluir(int id);
}
