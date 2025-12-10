using BarbeariaPortifolio.API.DTOs;

namespace BarbeariaPortifolio.API.Servicos.Interfaces
{
    public interface IAgendamentoServico
    {
        Task<IEnumerable<AgendamentoDTO>> ListarTodos();
        Task<AgendamentoDTO> BuscarPorId(int id);
        Task<IEnumerable<AgendamentoDTO>> ListarPorBarbeiro(int barbeiroId);

        Task<IEnumerable<AgendamentoDTO>> ListarPorUsuario(int usuarioId);

        Task<AgendamentoDTO> Cadastrar(int usuarioId, CriarAgendamentoDTO dto);
        Task<bool> Atualizar(int id, AgendamentoDTO dto);
        Task<bool> Excluir(int id);
        Task<bool> AlterarStatus(int id, int novoStatus);
    }
}
