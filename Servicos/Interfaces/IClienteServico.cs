using BarbeariaPortifolio.API.DTOs;

namespace BarbeariaPortifolio.API.Servicos.Interfaces;

public interface IClienteServico
{
    Task<ClienteDTO> BuscarPorUsuario(int usuarioID);
    Task<ClienteDTO> CriarPerfil(int usuarioId, ClienteDTO dto);
    Task<bool> AtualizarPerfil(int usuarioId, ClienteDTO dto);
}
