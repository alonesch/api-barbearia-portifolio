using BarbeariaPortifolio.API.Modules.Services.Models;

namespace BarbeariaPortifolio.API.Modules.Services.Repositories.Interfaces
{
    public interface IServicoRepositorio
    {
        Task<IEnumerable<Servico>> ListarTodos();
        Task<Servico?> BuscarPorId(int id);
        Task<Servico> Cadastrar(Servico servico);
        Task<bool> Atualizar(int id, Servico servico);
        Task<bool> Excluir(int id);
    }
}