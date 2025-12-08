using BarbeariaPortifolio.API.Models;

namespace BarbeariaPortifolio.API.Repositorios.Interfaces;

public interface IClienteRepositorio
{
    Task<Cliente?> BuscarPorUsuario(int usuarioId);
    Task<Cliente> Criar(Cliente cliente);
    Task<bool> Atualizar(Cliente cliente);
    Task<Usuario> BuscarUsuarioPorId(int usuarioId);
}
