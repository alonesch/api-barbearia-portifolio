using BarbeariaPortifolio.API.Models;


namespace BarbeariaPortifolio.API.Servicos.Interfaces
{
    public interface IServicoServico
    {
        Task<IEnumerable<Servico>> ListarTodos();
        Task<Servico?> BuscarPorId(int id);
        Task<Servico> Cadastrar(Servico servico);
        Task<bool> Atualizar(int id, Servico servico); 
        Task<bool> Excluir(int id);
    }
}
