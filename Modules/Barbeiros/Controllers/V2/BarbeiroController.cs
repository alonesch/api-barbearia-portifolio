using BarbeariaPortifolio.API.Modules.Agendamentos.DTOs;
using BarbeariaPortifolio.API.Modules.Agendamentos.Services;
using BarbeariaPortifolio.API.Modules.Agendamentos.Services.Interfaces;
using BarbeariaPortifolio.API.Modules.Barbeiros.Services.Interfaces;
using BarbeariaPortifolio.API.Modules.Usuarios.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;



namespace BarbeariaPortifolio.API.Modules.Barbeiros.Controllers.V2;

[ApiController]
[Route("api/v2/barbeiros")]
[Authorize]
public class BarbeirosController : ControllerBase
{
    private readonly IBarbeiroServico _servico;
    private readonly IUsuarioServico _usuarioServico;
    private readonly IAgendamentoServico _agendamentoServico;

    public BarbeirosController(
        IBarbeiroServico servico,
        IUsuarioServico usuarioServico,
        IAgendamentoServico agendamentoServico
    )
    {
        _servico = servico;
        _usuarioServico = usuarioServico;
        _agendamentoServico = agendamentoServico;
    }

    [Authorize(Roles = "Barbeiro")]
    [HttpGet("me/stats")]
    public async Task<IActionResult> StatsMe()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            return Unauthorized();

        var usuarioId = int.Parse(userIdClaim.Value);

        // ✅ resolve barbeiro no service CERTO
        var barbeiro = await _servico.BuscarPorUsuarioId(usuarioId);

        if (barbeiro == null)
            return NotFound("Barbeiro não encontrado");

        // ✅ stats continuam no AgendamentoService
        var stats = await _agendamentoServico.BuscarStatsBarbeiro(barbeiro.Id);

        return Ok(stats);
    }


}
