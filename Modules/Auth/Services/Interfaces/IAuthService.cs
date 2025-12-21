using BarbeariaPortifolio.API.Modules.Clientes.DTOs;
using BarbeariaPortifolio.API.Modules.Usuarios.Models;
using BarbeariaPortifolio.API.Modules.Auth.DTOs;

namespace BarbeariaPortifolio.API.Modules.Auth.Services.Interfaces;

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
