using BarbeariaPortifolio.DTOs;

namespace BarbeariaPortifolio.API.Servicos.Interfaces
{
    public interface IBarbeiroServico
    {
        Task<IEnumerable<BarbeiroDTO>> ListarTodos();
        Task<BarbeiroDTO?> BuscarPorId(int id);
        Task<BarbeiroDTO> Cadastrar(BarbeiroDTO barbeiro);
        Task<bool> Atualizar(int id, BarbeiroDTO barbeiro);
        Task<bool> Excluir(int id);
    }
}
