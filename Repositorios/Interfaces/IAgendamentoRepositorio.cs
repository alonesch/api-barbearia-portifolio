using BarbeariaPortifolio.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BarbeariaPortifolio.API.Repositorios.Interfaces;

public interface IAgendamentoRepositorio
{
    Task<IEnumerable<Agendamento>> ListarTodos();
    Task<Agendamento?> BuscarPorId(int id);
    Task<Agendamento> Cadastrar(Agendamento agendamento);
    Task<bool> Atualizar(int id, Agendamento agendamento);
    Task<bool> Excluir(int id);
    Task<IEnumerable<Agendamento>> ListarPorBarbeiro(int barbeiroId);
}
