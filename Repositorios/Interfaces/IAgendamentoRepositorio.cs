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

        Task<Cliente> BuscarOuCriarCliente(string nome, string cpf, string telefone);
        Task CadastrarAgendamentoServico(AgendamentoServico item);
    }
}
