using BarbeariaPortifolio.API.Shared.Exceptions;
using BarbeariaPortifolio.API.Modules.Auth.DTOs;
using BarbeariaPortifolio.API.Modules.Clientes.DTOs;
using BarbeariaPortifolio.API.Modules.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using BarbeariaPortifolio.API.Modules.Usuarios.DTOs;



namespace BarbeariaPortifolio.API.Modules.Auth.Controllers.V2;

[ApiController]
[Route("api/v2/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthServico _auth;
    private readonly IConfiguration _config;

    public AuthController(IAuthServico auth, IConfiguration config)
    {
        _auth = auth;
        _config = config;
    }

    public class LoginRequest
    {
        public string Usuario { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }


    [HttpGet("confirmar-email")]
    [AllowAnonymous]
    [EnableRateLimiting("ConfirmarEmailPolicy")]
    public async Task<IActionResult> ConfirmarEmail([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new AppException("Token não informado.", 400);

        await _auth.ConfirmarEmailAsync(token);

        var aceito = Request.Headers.Accept.ToString();

        if (aceito.Contains("text/html"))
        {
            return Redirect($"{_config["FRONTEND_URL"]}/confirmar-email?status=success");
        }

        return Ok(new { mensagem = "Email confirmado com sucesso." });
    }


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

        return Created("", new
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


    [EnableRateLimiting("LoginPolicy")]
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

        var  usuarioDto = new UsuarioDTO
        {
            Id = usuario.Id,
            NomeUsuario = usuario.NomeUsuario,
            Cargo = usuario.Cargo,
            BarbeiroId = barbeiroId,
            FotoPerfilUrl = usuario.FotoPerfilUrl
        };

        return Ok(new
        {
            mensagem = "Login realizado com sucesso.",
            dados = new
            {
                token = accessToken,
                refreshToken = refreshToken,
                usuario = usuarioDto

            }
        });
    }


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
