using BarbeariaPortifolio.API.DTOs;

namespace BarbeariaPortifolio.API.Servicos.Interfaces
{
    public interface IBarbeiroServico
    {
        Task<IEnumerable<BarbeiroDTO>> ListarTodos();
        Task<BarbeiroDTO?> BuscarPorId(int id);
        Task<BarbeiroDTO> Cadastrar(CriarBarbeiroDTO dto);
        Task<bool> Atualizar(int id, CriarBarbeiroDTO dto);
        Task<bool> Excluir(int id);
    }
}
