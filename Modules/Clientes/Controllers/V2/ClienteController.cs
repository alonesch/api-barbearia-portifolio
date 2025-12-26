using BarbeariaPortifolio.API.Modules.Agendamentos.Services.Interfaces;
using BarbeariaPortifolio.API.Modules.Clientes.Services.Interfaces;
using BarbeariaPortifolio.API.Modules.Usuarios.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BarbeariaPortifolio.API.Modules.Clientes.Controllers.V2;

[ApiController]
[Route("api/v2/clientes")]
[Authorize]

public class ClientesController : ControllerBase
{
    private readonly IClienteServico _servico;
    private readonly IAgendamentoServico _agendamentoServico;
    private readonly IUsuarioServico _usuarioServico;
    public ClientesController
        (IClienteServico servico,
        IAgendamentoServico agendamentoServico,
        IUsuarioServico usuarioServico)
    {
        _servico = servico;
        _agendamentoServico = agendamentoServico;
        _usuarioServico = usuarioServico;
    }

    [Authorize(Roles = "Cliente")]
    [HttpGet("me/stats")]
    public async Task<IActionResult> StatsMe()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            return Unauthorized();

        var usuarioId = int.Parse(userIdClaim.Value);

        var stats = await _agendamentoServico.BuscarStatsCliente(usuarioId);
        return Ok(stats);
    }

}
