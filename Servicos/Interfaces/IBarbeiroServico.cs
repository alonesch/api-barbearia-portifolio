using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.DTOs;

namespace BarbeariaPortifolio.API.Servicos.Interfaces
{
    public interface IBarbeiroServico
    {
        Task<IEnumerable<BarbeiroDTO>> ListarTodos();
        Task<Barbeiro?> BuscarPorId(int id);
        Task<Barbeiro> Cadastrar(Barbeiro barbeiro);
        Task<bool> Atualizar(int id, Barbeiro barbeiro);
        Task<bool> Excluir(int id);
    }
}