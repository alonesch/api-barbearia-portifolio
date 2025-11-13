using BarbeariaPortifolio.API.Servicos.Interfaces;
using BarbeariaPortifolio.API.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using BarbeariaPortifolio.API.Auth;

namespace BarbeariaPortifolio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IAuthServico _auth;
        private readonly JwtOptions _authJwt;

        public LoginController(IAuthServico auth, IOptions<JwtOptions> jwt)
        {
            _auth = auth;
            _authJwt = jwt.Value;
        }

        public class LoginRequest
        {
            public string Usuario { get; set; } = string.Empty;
            public string Senha { get; set; } = string.Empty;
        }

        // 🔒 Protegido com rate limit
        [EnableRateLimiting("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.Usuario) ||
                string.IsNullOrWhiteSpace(request.Senha))
            {
                return BadRequest(new
                {
                    autenticado = false,
                    mensagem = "Credenciais inválidas."
                });
            }

            // 🔍 Valida usuário + senha
            var (sucesso, mensagem, usuario) =
                await _auth.ValidarLogin(request.Usuario, request.Senha);

            if (!sucesso || usuario == null)
            {
                return Unauthorized(new
                {
                    autenticado = false,
                    mensagem
                });
            }

            // 🔑 Gera Access Token
            var accessToken = await _auth.GerarAccessToken(usuario);

            // 🔄 Gera Refresh Token bruto + hash
            var (refreshRaw, refreshHash) = await _auth.GerarRefreshToken();

            // 💾 Salva refresh token no banco
            await _auth.SalvarRefreshToken(usuario, refreshHash, _authJwt.RefreshTokenDays);

            // 📦 Retorno final
            return Ok(new
            {
                autenticado = true,
                mensagem = "Login efetuado com sucesso.",
                token = accessToken,
                refreshToken = refreshRaw,
                usuario = new
                {
                    usuario.Id,
                    usuario.NomeUsuario,
                    usuario.NomeCompleto,
                    usuario.Cargo,
                    usuario.Role,
                    usuario.BarbeiroId
                }
            });
        }
    }
}

