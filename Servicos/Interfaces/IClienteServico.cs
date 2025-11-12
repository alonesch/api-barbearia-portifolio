using BarbeariaPortifolio.DTOs;

namespace BarbeariaPortifolio.API.Servicos.Interfaces;

public interface IClienteServico
{
    Task<IEnumerable<ClienteDTO>> ListarTodos();
    Task<ClienteDTO?> BuscarPorId(int id);
    Task<ClienteDTO> Cadastrar(ClienteDTO dto);
    Task<bool> Atualizar(int id, ClienteDTO dto);
    Task<bool> Excluir(int id);
}
