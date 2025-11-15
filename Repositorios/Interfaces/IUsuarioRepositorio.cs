using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.DTOs;

namespace BarbeariaPortifolio.API.Repositorios.Interfaces
{
    public interface IUsuarioRepositorio
    {
        //Task<IEnumerable<UsuarioDTO>> ListarTodos();
        Task<Usuario?> BuscarPorId(int id);
        Task<Usuario?> BuscarPorNome(string nomeUsuario);
        Task<Usuario> Cadastrar(Usuario usuario);
        Task<bool> Atualizar(int id, Usuario usuario);
        Task<bool> Excluir(int id);
    }
}
