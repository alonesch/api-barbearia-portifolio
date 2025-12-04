using BarbeariaPortifolio.API.Models;
using System.Threading.Tasks;

namespace BarbeariaPortifolio.API.Servicos.Interfaces
{
    public interface IUsuarioServico
    {
        // Futuro:
        // Task<IEnumerable<Usuario>> ListarTodos();

        Task<Usuario?> BuscarPorId(int id);
        Task<Usuario> Cadastrar(Usuario usuario);
        Task<bool> Atualizar(int id, Usuario usuario);
        Task<bool> Excluir(int id);
    }
}
