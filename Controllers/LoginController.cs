using BarbeariaPortifolio.API.Servicos.Interfaces;
using BarbeariaPortifolio.API.Auth;
using BarbeariaPortifolio.API.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;


namespace BarbeariaPortifolio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IAuthServico _auth;
        private readonly JwtOptions _jwt;

        public LoginController(IAuthServico auth, IOptions<JwtOptions> jwt)
        {
            _auth = auth;
            _jwt = jwt.Value;
        }

        public class LoginRequest
        {
            public string Usuario { get; set; } = string.Empty;
            public string Senha { get; set; } = string.Empty;
        }

        //[EnableRateLimiting("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.Usuario) ||
                string.IsNullOrWhiteSpace(request.Senha))
            {
                throw new AppException("Credenciais inválidas.", 400);
            }

            var (_, _, usuario) = await _auth.ValidarLogin(request.Usuario, request.Senha);



            var accessToken = await _auth.GerarAccessToken(usuario!);

            var (refreshRaw, refreshHash) = await _auth.GerarRefreshToken();

            await _auth.SalvarRefreshToken(usuario!, refreshHash, _jwt.RefreshTokenDays);

            var barbeiroId = await _auth.BuscarBarbeiroId(usuario!.Id);

            return Ok(new
            {
                mensagem = "Login realizado com sucesso.",
                dados = new
                {
                    token = accessToken,
                    refreshToken = refreshRaw,
                    usuario = new
                    {
                        usuario.Id,
                        usuario.NomeUsuario,
                        usuario.NomeCompleto,
                        usuario.Cargo,
                        usuario.Role,
                        barbeiroId = barbeiroId ?? null
                    }
                }

            });
        }
    }
}
