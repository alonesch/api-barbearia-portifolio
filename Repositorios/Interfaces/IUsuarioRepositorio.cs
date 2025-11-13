using BarbeariaPortifolio.API.Models;

namespace BarbeariaPortifolio.API.Repositorios.Interfaces
{
    public interface IUsuarioRepositorio
    {
        Task<IEnumerable<Usuario>> ListarTodos();
        Task<Usuario?> BuscarPorId(int id);
        Task<Usuario?> BuscarPorNome(string nomeUsuario);
        Task<Usuario> Cadastrar(Usuario usuario);
        Task<bool> Atualizar(int id, Usuario usuario);
        Task<bool> Excluir(int id);
    }
}
