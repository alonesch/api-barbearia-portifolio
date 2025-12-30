using BarbeariaPortifolio.API.Modules.Clientes.Models;
using BarbeariaPortifolio.API.Modules.Usuarios.Models;

namespace BarbeariaPortifolio.API.Modules.Clientes.Repositories.Interfaces;

public interface IClienteRepositorio
{
    Task<Cliente?> BuscarPorUsuario(int usuarioId);
    Task<Cliente> Criar(Cliente cliente);
    Task<bool> Atualizar(Cliente cliente);
    Task<Usuario> BuscarUsuarioPorId(int usuarioId);
}
