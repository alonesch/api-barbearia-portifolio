using BarbeariaPortifolio.API.DTOs;
using BarbeariaPortifolio.API.Exceptions;
using BarbeariaPortifolio.API.Servicos.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BarbeariaPortifolio.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServico _auth;

        public AuthController(IAuthServico auth)
        {
            _auth = auth;
        }

        public class LoginRequest
        {
            public string Usuario { get; set; } = string.Empty; // email OU username
            public string Senha { get; set; } = string.Empty;
        }

        // ✅ CONFIRMAÇÃO DE EMAIL
        [HttpGet("confirmar-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmarEmail([FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new AppException("Token não informado.", 400);

            await _auth.ConfirmarEmailAsync(token);

            return Ok(new { mensagem = "Email confirmado com sucesso." });
        }

        // ✅ REGISTER
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegistrarNovoClienteDTO dto)
        {
            if (dto == null ||
                string.IsNullOrWhiteSpace(dto.NomeCompleto) ||
                string.IsNullOrWhiteSpace(dto.NomeUsuario) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Senha))
            {
                throw new AppException("Todos os campos são obrigatórios.", 400);
            }

            var usuario = await _auth.RegistrarAsync(dto);

            return Ok(new
            {
                mensagem = "Registro realizado com sucesso.",
                usuario = new
                {
                    usuario.Id,
                    usuario.NomeUsuario,
                    usuario.NomeCompleto,
                    usuario.Email,
                    usuario.Cargo
                }
            });
        }

        // ✅ LOGIN
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.Usuario) ||
                string.IsNullOrWhiteSpace(request.Senha))
            {
                throw new AppException("Credenciais inválidas.", 400);
            }

            var (accessToken, refreshToken, usuario) =
                await _auth.LoginAsync(request.Usuario, request.Senha);

            var barbeiroId = await _auth.BuscarBarbeiroId(usuario.Id);

            return Ok(new
            {
                mensagem = "Login realizado com sucesso.",
                dados = new
                {
                    token = accessToken,
                    refreshToken = refreshToken,
                    usuario = new
                    {
                        usuario.Id,
                        usuario.NomeUsuario,
                        usuario.NomeCompleto,
                        usuario.Email,
                        usuario.Cargo,
                        barbeiroId = barbeiroId
                    }
                }
            });
        }

        // ✅ REENVIO DE CONFIRMAÇÃO (NEUTRO + SEGURO)
        [HttpPost("reenviar-confirmacao")]
        [AllowAnonymous]
        public async Task<IActionResult> ReenviarConfirmacao([FromBody] ReenviarConfirmacaoEmailDto dto)
        {
            await _auth.ReenviarConfirmacaoEmailAsync(dto);

            return Ok(new
            {
                mensagem = "Se existir uma conta com este e-mail, um novo link de confirmação foi enviado."
            });
        }
    }
}
