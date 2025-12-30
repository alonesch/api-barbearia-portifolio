using BarbeariaPortifolio.API.Modules.Barbeiros.DTOs;
using BarbeariaPortifolio.API.Modules.Usuarios.DTOs;

namespace BarbeariaPortifolio.API.Modules.Barbeiros.Services.Interfaces;

public interface IBarbeiroServico
{
    Task<IEnumerable<BarbeiroDTO>> ListarTodos();
    Task<BarbeiroDTO?> BuscarPorId(int id);
    Task<BarbeiroDTO?> BuscarPorUsuarioId(int usuarioId);
    Task<BarbeiroDTO> Cadastrar(CriarBarbeiroDTO dto);
    Task<bool> Atualizar(int id, CriarBarbeiroDTO dto);
    Task<bool> Excluir(int id);
}
