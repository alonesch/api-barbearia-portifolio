using BarbeariaPortifolio.API.Models;
using System.Threading.Tasks;

namespace BarbeariaPortifolio.API.Servicos.Interfaces
{
    public interface IAuthServico
    {
        Task<(bool sucesso, string mensagem, Usuario? usuario)> ValidarLogin(string usuario, string senha);
        Task <int?> BuscarBarbeiroId(int usuarioId);
        Task<string> GerarAccessToken(Usuario usuario);
        Task<(string rawToken, string hashToken)> GerarRefreshToken();
        Task SalvarRefreshToken(Usuario user, string hashToken, int diasExpiracao);
    }
}
