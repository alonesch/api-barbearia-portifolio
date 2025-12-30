using BarbeariaPortifolio.API.Modules.Agendamentos.DTOs;
using BarbeariaPortifolio.API.Modules.Clientes.DTOs;
using BarbeariaPortifolio.API.Modules.Barbeiros.DTOs;
using BarbeariaPortifolio.API.Shared.DTOs;


namespace BarbeariaPortifolio.API.Modules.Agendamentos.Services.Interfaces;

public interface IAgendamentoServico
{
    Task<IEnumerable<AgendamentoDTO>> ListarTodos();
    Task<IEnumerable<AgendamentoDTO>> ListarPorBarbeiroEData(int barbeiroId, DateOnly data);
    Task<AgendamentoDTO> BuscarPorId(int id);
    Task<ClienteStatsDTO> BuscarStatsCliente(int id);
    Task<BarbeiroStatsDTO> BuscarStatsBarbeiro(int id);
    Task<IEnumerable<AgendamentoDTO>> ListarPorUsuario(int usuarioId);
    Task<PagedResultDTO<AgendamentoDTO>> ListarPorUsuarioPaginado(int usuarioId, int page, int pageSize);

    Task<IEnumerable<AgendamentoHistoricoDTO>> ListarHistoricoPorUsuario(int usuarioId );

    Task<PagedResultDTO<AgendamentoHistoricoDTO>> ListarHistoricoPorUsuarioPaginado( int usuarioId,int page,int pageSize);

    Task<IEnumerable<AgendamentoHistoricoDTO>> ListarHistoricoPorBarbeiro(int barbeiroId);

    Task<AgendamentoDTO> Cadastrar(int usuarioId, CriarAgendamentoDTO dto);
    Task<bool> Atualizar(int id, AgendamentoDTO dto);
    Task<bool> AlterarStatus(int id, int novoStatus);
    Task CancelarAgendamento(int id, int usuarioId);
    Task<bool> Excluir(int id);
}
