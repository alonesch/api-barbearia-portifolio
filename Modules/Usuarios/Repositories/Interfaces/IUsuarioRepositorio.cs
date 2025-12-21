using BarbeariaPortifolio.API.Modules.Usuarios.Models;

namespace BarbeariaPortifolio.API.Modules.Usuarios.Repositories.Interfaces;

public interface IUsuarioRepositorio
{
    Task<Usuario?> BuscarPorId(int id);
    Task<Usuario?> BuscarPorNome(string nomeUsuario);
    Task<Usuario?> BuscarPorEmail(string email);

    Task<Usuario> Cadastrar(Usuario usuario);
    Task<bool> Atualizar(int id, Usuario usuario);
    Task<bool> Excluir(int id);
}
