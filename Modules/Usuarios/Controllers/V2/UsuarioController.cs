using BarbeariaPortifolio.API.Modules.Usuarios.DTOs;
using BarbeariaPortifolio.API.Shared.Exceptions;
using BarbeariaPortifolio.API.Modules.Usuarios.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BarbeariaPortifolio.API.Modules.Usuarios.Controllers.V2;

[ApiController]
[Route("api/v2/usuarios")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioServico _servico;

    public UsuariosController(IUsuarioServico servico)
    {
        _servico = servico;
    }

    [Authorize]
    [HttpPatch("me/foto")]
    public async Task<IActionResult> AtualizarFotoPerfil([FromBody] AtualizarFotoPerfilDTO dto)
    {
        var usuarioClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(usuarioClaim))
            return Unauthorized();

        if (!int.TryParse(usuarioClaim, out var usuarioId))
            return Unauthorized();

        await _servico.AtualizarFotoPerfil(usuarioId, dto.FotoPerfilUrl);
        return NoContent();
    }

    [Authorize]
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


    [Authorize]
    [HttpPatch("me")]
    public async Task<IActionResult> AtualizarMe(
      [FromBody] AtualizarUsuarioPerfilDTO dto
  )
    {
        var usuarioId = int.Parse(User.FindFirst("id")!.Value);

        var usuario = await _servico.AtualizarMeAsync(usuarioId, dto);

        return Ok(new
        {
            mensagem = "Perfil atualizado com sucesso.",
            dados = usuario
        });
    }
}

