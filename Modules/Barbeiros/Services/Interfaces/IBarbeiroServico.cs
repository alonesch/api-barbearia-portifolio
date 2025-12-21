using BarbeariaPortifolio.API.Modules.Barbeiros.DTOs;

namespace BarbeariaPortifolio.API.Modules.Barbeiros.Services.Interfaces;

public interface IBarbeiroServico
{
    Task<IEnumerable<BarbeiroDTO>> ListarTodos();
    Task<BarbeiroDTO?> BuscarPorId(int id);
    Task<BarbeiroDTO> Cadastrar(CriarBarbeiroDTO dto);
    Task<bool> Atualizar(int id, CriarBarbeiroDTO dto);
    Task<bool> Excluir(int id);
}
