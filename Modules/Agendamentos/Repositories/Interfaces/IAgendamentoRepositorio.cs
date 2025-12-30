using BarbeariaPortifolio.API.Modules.Agendamentos.Models;


namespace BarbeariaPortifolio.API.Modules.Agendamentos.Repositories.Interfaces;
public interface IAgendamentoRepositorio
{
    Task<IEnumerable<Agendamento>> ListarTodos();
    Task<Agendamento?> BuscarPorId(int id);
    Task<IEnumerable<Agendamento>> ListarHistoricoPorBarbeiro(int barbeiroId);
    Task<IEnumerable<Agendamento>> ListarPorUsuario(int usuarioId);

    Task<Agendamento> Cadastrar(Agendamento agendamento);
    Task<bool> Atualizar(Agendamento agendamento);
    Task<bool> Excluir(int id);


    Task<bool> ChecarHorarios(int barbeiroId,DateTime dataHora,int disponibilidadeId);

    Task CadastrarAgendamentoServico(AgendamentoServico item);
    Task<Agendamento?> BuscarStatusId(int id);
    Task AlterarStatus(Agendamento agendamento);
    IQueryable<Agendamento> QueryPorUsuario(int usuarioId);
}
