using BarbeariaPortifolio.DTOs;

namespace BarbeariaPortifolio.API.Servicos.Interfaces
{
    public interface IServicoServico
    {
        Task<IEnumerable<ServicoDTO>> ListarTodos();
        Task<ServicoDTO?> BuscarPorId(int id);
        Task<ServicoDTO> Cadastrar(ServicoDTO dto);
        Task<bool> Atualizar(int id, ServicoDTO dto);
        Task<bool> Excluir(int id);
    }
}
