using BarbeariaPortifolio.API.Models;
using System.Threading.Tasks;

namespace BarbeariaPortifolio.API.Repositorios.Interfaces
{
    public interface IRefreshTokenRepositorio
    {
        Task<RefreshToken?> BuscarPorHash(string hash);
        Task<RefreshToken?> BuscarTokenValido(int usuarioId);
        Task Salvar(RefreshToken token);
        Task RevogarTokensAtivos(int usuarioId);
    }
}
