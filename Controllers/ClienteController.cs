using BarbeariaPortifolio.API.Servicos.Interfaces;
using BarbeariaPortifolio.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BarbeariaPortifolio.API.Exceptions;
using BarbeariaPortifolio.API.Servicos;

namespace BarbeariaPortifolio.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClienteController : ControllerBase
{
    private readonly IClienteServico _servico;


    public ClienteController(IClienteServico servico)
    {
        _servico = servico; 
    }
    
    
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var usuarioId = int.Parse(User.FindFirst("userId")!.Value);
        return Ok(await _servico.BuscarPorUsuario(usuarioId));
    }

    [HttpPost("me")]
    public async Task<IActionResult> CriarPerfil([FromBody] ClienteDTO dto)
    {
        var usuarioId = int.Parse(User.FindFirst("userId")!.Value);
        return Ok(await _servico.CriarPerfil(usuarioId, dto));
    }

    [HttpPut("me")]
    public async Task<IActionResult> AtualizarPerfil([FromBody] ClienteDTO dto)
    {
        var usuarioId = int.Parse(User.FindFirst("userId")!.Value);
        await _servico.AtualizarPerfil(usuarioId, dto);
        return Ok(new { mensagem = "Perfil de cliente atualizado com sucesso." });
    }
}

