using BarbeariaPortifolio.API.Modules.Disponibilidades.DTOs;
using BarbeariaPortifolio.API.Shared.Exceptions;
using BarbeariaPortifolio.API.Modules.Disponibilidades.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarbeariaPortifolio.API.Modules.Disponibilidades.Controllers.V1;

[ApiController]
[Route("api/v1/disponibilidades")]
public class DisponibilidadeController : ControllerBase
{
    private readonly IDisponibilidadeServico _servico;

    public DisponibilidadeController(IDisponibilidadeServico servico)
    {
        _servico = servico;
    }

    [Authorize(Policy = "AdminOuBarbeiro")]
    [HttpPost("{barbeiroId}")]
    public async Task<IActionResult> Criar(
        int barbeiroId,
        [FromBody] CriarDisponibilidadeDto criarDto)
    {
        await _servico.CriarDisponibilidadeAsync(barbeiroId, criarDto);
        return Ok(new { mensagem = "Disponibilidade criada com sucesso." });
    }

    [Authorize]
    [HttpGet("barbeiro/{barbeiroId}")]
    public async Task<IActionResult> ListarPorData(
    int barbeiroId,
    [FromQuery] string data)
    {
        if (!DateOnly.TryParse(data, out var dataConvertida))
            throw new AppException("Data inválida.", 400);

        var lista = await _servico
            .ListarDisponibilidadesDoBarbeiroAsync(barbeiroId, dataConvertida);

        return Ok(lista);
    }




    [Authorize(Policy = "AdminOuBarbeiro")]
    [HttpPatch("{disponibilidadeId:int}/status")]
    public async Task<IActionResult> AtualizarStatus(
        [FromRoute] int disponibilidadeId,
        [FromBody] AtualizarDisponibilidade dto)
    {
        var barbeiroClaim = User.FindFirst("barbeiroId")?.Value;

        if (string.IsNullOrWhiteSpace(barbeiroClaim))
            throw new AppException("Usuário não é barbeiro.", 403);

        var barbeiroId = int.Parse(barbeiroClaim);

        await _servico.AtualizarStatusAsync(
            disponibilidadeId,
            dto.Ativo,
            barbeiroId
        );

        return Ok(new { mensagem = "Status atualizado com sucesso." });
    }
}
