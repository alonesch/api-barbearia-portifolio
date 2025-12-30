using BarbeariaPortifolio.API.Auth.DTOs;
using BarbeariaPortifolio.API.Modules.Auth.DTOs;
using BarbeariaPortifolio.API.Modules.Auth.Services.Interfaces;
using BarbeariaPortifolio.API.Modules.Clientes.DTOs;
using BarbeariaPortifolio.API.Modules.Usuarios.DTOs;
using BarbeariaPortifolio.API.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;



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

    [HttpGet("check-username")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckUsername([FromQuery] string nomeUsuario)
    {
        if (string.IsNullOrWhiteSpace(nomeUsuario))
            throw new AppException("Username inválido", 400);

        var disponivel= await _auth.UsernameDisponivel(nomeUsuario);
        return Ok(new
        {
            mensagem = disponivel
            ? "Nome de usuário disponivel."
            : "Nome de usuário indisponivel.",
            dados = new { disponivel }
        });
    }

    [HttpGet("check-email")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckEmail([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new AppException("Email inválido", 400);
        var disponivel = await _auth.EmailDisponivel(email);
        return Ok(new
        {
            mensagem = disponivel
            ? "Email disponivel."
            : "Email indisponivel.",
            dados = new { disponivel }
        });
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
    public async Task<IActionResult> Login([FromBody] LoginDTO request)

    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }


        var (accessToken, refreshToken, usuario) =
            await _auth.LoginAsync(request.Usuario, request.Senha);

        var barbeiroId = await _auth.BuscarPorUsuarioId(usuario.Id);

        var  usuarioDto = new UsuarioDTO
        {
            Id = usuario.Id,
            NomeUsuario = usuario.NomeUsuario,
            Cargo = usuario.Cargo
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
