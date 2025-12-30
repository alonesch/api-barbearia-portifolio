using BarbeariaPortifolio.API.Modules.Clientes.DTOs;

namespace BarbeariaPortifolio.API.Modules.Clientes.Services.Interfaces;

public interface IClienteServico
{
    Task<ClienteDTO> BuscarPorUsuario(int usuarioID);
    Task<ClienteDTO> CriarPerfil(int usuarioId, ClienteDTO dto);
    Task<bool> AtualizarPerfil(int usuarioId, ClienteDTO dto);
}
