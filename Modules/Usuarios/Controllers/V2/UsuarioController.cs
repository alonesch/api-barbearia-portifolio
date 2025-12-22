using BarbeariaPortifolio.API.Modules.Usuarios.DTOs;
using BarbeariaPortifolio.API.Modules.Usuarios.Services;
using BarbeariaPortifolio.API.Shared.Exceptions;
using BarbeariaPortifolio.API.Modules.Usuarios.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BarbeariaPortifolio.API.Modules.Usuarios.Controllers.V2;

[ApiController]
[Route("api/v2/usuarios")]
[Authorize]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioServico _servico;

    public UsuarioController(IUsuarioServico servico)
    {
        _servico = servico;
    }

    [HttpPost("me/foto")]
    public async Task<IActionResult> AtualizarFotoPerfil([FromBody] AtualizarFotoPerfilDTO dto)
    {
        var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(usuarioIdClaim))
            return Unauthorized();

        var usuarioId = int.Parse(usuarioIdClaim);

        await _servico.AtualizarFotoPerfil(usuarioId, dto.FotoPerfilUrl);

        return NoContent();
    }


    [HttpGet("me")]
    public async Task<ActionResult<UsuarioPerfilDTO>> Me()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            return Unauthorized();

        var usuarioId = int.Parse(userIdClaim.Value);

        var usuario = await _servico.BuscarPorId(usuarioId);

        if (usuario == null)
            throw new AppException("Usuário não encontrado.", 404);

        
        var dto = UsuarioMapper.ToPerfilDTO(usuario);

        return Ok(dto);
    }
}