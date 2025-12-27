using BarbeariaPortifolio.API.Modules.Usuarios.Models;
using BarbeariaPortifolio.API.Modules.Usuarios.DTOs;


namespace BarbeariaPortifolio.API.Modules.Usuarios.Services.Interfaces;

public interface IUsuarioServico
{
    // Futuro:
    // Task<IEnumerable<Usuario>> ListarTodos();

    Task<Usuario?> BuscarPorId(int id);
    Task<Usuario> Cadastrar(Usuario usuario);
    Task<bool> Atualizar(int id, Usuario usuario);

    Task<Usuario>AtualizarMeAsync(int usuarioId, AtualizarUsuarioPerfilDTO dto);
    Task<bool> Excluir(int id);
    Task AtualizarFotoPerfil(int usuarioId, string fotoPerfilUrl);
}
