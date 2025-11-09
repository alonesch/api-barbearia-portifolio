using BarbeariaPortifolio.API.Auth;
using BarbeariaPortifolio.API.Data;
using BarbeariaPortifolio.API.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace BarbeariaPortifolio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly TokenService _tokenService;
        private readonly JwtOptions _jwt;

        public LoginController(DataContext context, TokenService tokenService, IOptions<JwtOptions> jwt)
        {
            _context = context;
            _tokenService = tokenService;
            _jwt = jwt.Value;
        }

        // 🔒 Protegido com Rate Limiting
        [EnableRateLimiting("login")]
        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Usuario) || string.IsNullOrWhiteSpace(request.Senha))
                return BadRequest(new { autenticado = false, mensagem = "Credenciais inválidas." });

            var user = _context.Usuarios.FirstOrDefault(u => u.NomeUsuario == request.Usuario && u.Ativo);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Senha, user.Senha))
                return Unauthorized(new { autenticado = false, mensagem = "Usuário ou senha incorretos." });

            // 🔹 Gera claims do usuário
            var claims = user.ToClaims();

            // 🔹 Gera o access token
            var accessToken = _tokenService.GenerateAccessToken(claims);

            // 🔹 Cria o refresh token
            var refreshRaw = _tokenService.GenerateRefreshTokenRaw();
            var refreshHash = _tokenService.HashRefreshToken(refreshRaw);

            var rt = new RefreshToken
            {
                UsuarioId = user.Id,
                TokenHash = refreshHash,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwt.RefreshTokenDays)
            };

            _context.RefreshTokens.Add(rt);
            _context.SaveChanges();

            return Ok(new
            {
                autenticado = true,
                mensagem = "Login efetuado com sucesso.",
                token = accessToken,
                refreshToken = refreshRaw,
                usuario = new
                {
                    user.Id,
                    user.NomeUsuario,
                    user.NomeCompleto,
                    user.Cargo
                }
            });
        }

        public class LoginRequest
        {
            public string Usuario { get; set; } = string.Empty;
            public string Senha { get; set; } = string.Empty;
        }
    }
}
