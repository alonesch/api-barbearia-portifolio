using BarbeariaPortifolio.API.Modules.Clientes.Services.Interfaces;
using BarbeariaPortifolio.API.Modules.Clientes.DTOs;
using BarbeariaPortifolio.API.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BarbeariaPortifolio.API.Modules.Clientes.Controllers.V1;

[ApiController]
[Route("api/[controller]")]
public class ClienteController : ControllerBase
{
    private readonly IClienteServico _servico;


    public ClienteController(IClienteServico servico)
    {
        _servico = servico;
    }

    [Authorize(Roles = "Cliente")]
    [HttpGet("me")]


    public async Task<IActionResult> Me()
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        return Ok(await _servico.BuscarPorUsuario(usuarioId));
    }

    [Authorize(Roles = "Cliente")]
    [HttpPost("me")]
    public async Task<IActionResult> CriarPerfil([FromBody] ClienteDTO dto)
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        return Ok(await _servico.CriarPerfil(usuarioId, dto));
    }
   
    [Authorize(Roles = "Cliente")]
    [HttpPut("me")]
    public async Task<IActionResult> AtualizarPerfil([FromBody] ClienteDTO dto)
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _servico.AtualizarPerfil(usuarioId, dto);
        return Ok(new { mensagem = "Perfil de cliente atualizado com sucesso." });
    }
}

