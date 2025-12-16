using BarbeariaPortifolio.API.DTOs;

namespace BarbeariaPortifolio.API.Servicos.Interfaces
{
    public interface IAgendamentoServico
    {
        Task<IEnumerable<AgendamentoDTO>> ListarTodos();
        Task<IEnumerable<AgendamentoDTO>> ListarPorBarbeiroEData(int barbeiroId, DateOnly data);
        Task<AgendamentoDTO> BuscarPorId(int id);

        Task<IEnumerable<AgendamentoDTO>> ListarPorUsuario(int usuarioId);
        Task<PagedResultDTO<AgendamentoDTO>> ListarPorUsuarioPaginado(int usuarioId, int page, int pageSize);

        // 🔥 HISTÓRICO COM DTO PRÓPRIO
        Task<IEnumerable<AgendamentoHistoricoDTO>> ListarHistoricoPorBarbeiro(int barbeiroId);

        Task<AgendamentoDTO> Cadastrar(int usuarioId, CriarAgendamentoDTO dto);
        Task<bool> Atualizar(int id, AgendamentoDTO dto);
        Task<bool> AlterarStatus(int id, int novoStatus);
        Task CancelarAgendamento(int id, int usuarioId);
        Task<bool> Excluir(int id);
    }
}
