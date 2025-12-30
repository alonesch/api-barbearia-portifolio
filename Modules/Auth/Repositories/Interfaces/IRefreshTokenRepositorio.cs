using BarbeariaPortifolio.API.Modules.Auth.Models;
using System.Threading.Tasks;

namespace BarbeariaPortifolio.API.Modules.Auth.Repositories.Interfaces;

public interface IRefreshTokenRepositorio
{
    Task<RefreshToken?> BuscarPorHash(string hash);
    Task<RefreshToken?> BuscarTokenValido(int usuarioId);
    Task Salvar(RefreshToken token);
    Task RevogarTokensAtivos(int usuarioId);
}
