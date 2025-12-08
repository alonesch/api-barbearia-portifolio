using BarbeariaPortifolio.API.Models;

namespace BarbeariaPortifolio.API.Repositorios.Interfaces
{
    public interface IAgendamentoRepositorio
    {
        Task<IEnumerable<Agendamento>> ListarTodos();
        Task<Agendamento?> BuscarPorId(int id);
        Task<IEnumerable<Agendamento>> ListarPorBarbeiro(int barbeiroId);

        Task<Agendamento> Cadastrar(Agendamento agendamento);
        Task<bool> Atualizar(Agendamento agendamento);
        Task<bool> Excluir(int id);

        Task CadastrarAgendamentoServico(AgendamentoServico item);

        Task<bool> ChecarHorarios(int barbeiroId, DateTime dataHora);

        Task<Agendamento?> BuscarStatusId(int id);
        Task AlterarStatus(Agendamento agendamento);
    }
}
