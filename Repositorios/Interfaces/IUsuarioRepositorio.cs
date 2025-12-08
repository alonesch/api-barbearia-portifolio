using BarbeariaPortifolio.API.Models;

namespace BarbeariaPortifolio.API.Repositorios.Interfaces
{
    public interface IUsuarioRepositorio
    {
        Task<Usuario?> BuscarPorId(int id);
        Task<Usuario?> BuscarPorNome(string nomeUsuario);
        Task<Usuario?> BuscarPorEmail(string email);

        Task<Usuario> Cadastrar(Usuario usuario);
        Task<bool> Atualizar(int id, Usuario usuario);
        Task<bool> Excluir(int id);
    }
}
