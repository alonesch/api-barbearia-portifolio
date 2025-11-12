using BarbeariaPortifolio.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BarbeariaPortifolio.API.Servicos.Interfaces;

public interface IAgendamentoServico
{
    Task<IEnumerable<AgendamentoDTO>> ListarTodos();
    Task<AgendamentoDTO?> BuscarPorId(int id);
    Task<AgendamentoDTO> Cadastrar(AgendamentoDTO dto);
    Task<bool> Atualizar(int id, AgendamentoDTO dto);
    Task<bool> Excluir(int id);
}
