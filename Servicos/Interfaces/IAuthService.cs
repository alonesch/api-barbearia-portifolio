using BarbeariaPortifolio.API.DTOs;
using BarbeariaPortifolio.API.Models;

namespace BarbeariaPortifolio.API.Servicos.Interfaces
{
    public interface IAuthServico
    {
        Task<Usuario> RegistrarAsync(RegistrarNovoClienteDTO dto);

        Task<(string accessToken, string refreshToken, Usuario usuario)>
            LoginAsync(string login, string senha);

        Task<string> GerarAccessToken(Usuario usuario);

        Task<(string rawToken, string hashToken)> GerarRefreshToken();

        Task SalvarRefreshToken(Usuario user, string hashToken, int diasExpiracao);

        Task<int?> BuscarBarbeiroId(int usuarioId);

        Task ReenviarConfirmacaoEmailAsync(ReenviarConfirmacaoEmailDto dto);

        Task ConfirmarEmailAsync(string token);

    }
}
