using BarbeariaPortifolio.API.Modules.Agendamentos.DTOs;
using BarbeariaPortifolio.API.Shared.Exceptions;
using BarbeariaPortifolio.API.Modules.Agendamentos.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BarbeariaPortifolio.API.Modules.Agendamentos.Controllers;

[ApiController]
[Route("api/v1/agendamentos")]
public class AgendamentosController : ControllerBase
{
    private readonly IAgendamentoServico _servico;

    public AgendamentosController(IAgendamentoServico servico)
    {
        _servico = servico;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        return Ok(await _servico.ListarTodos());
    }
   

    [Authorize]
    [HttpGet("barbeiro/{barbeiroId}/data")]
    public async Task<IActionResult> ListarPorBarbeiroEData(
        int barbeiroId,
        [FromQuery] string data)
    {
        if (!DateOnly.TryParse(data, out var dataConvertida))
            throw new AppException("Data inválida.", 400);

        var agendamentos = await _servico.ListarPorBarbeiroEData(barbeiroId, dataConvertida);
        return Ok(agendamentos);
    }


    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> Buscar(int id)
    {
        return Ok(await _servico.BuscarPorId(id));
    }

    [Authorize(Roles = "Barbeiro")]
    [HttpGet("barbeiro/{barbeiroId}/historico")]
    public async Task<IActionResult> ListarHistorico(int barbeiroId)
    {
        var resultado = await _servico.ListarHistoricoPorBarbeiro(barbeiroId);
        return Ok(resultado);
    }


    [Authorize(Roles = "Cliente")]
    [HttpGet("me")]
    public async Task<IActionResult> ListarMeus([FromQuery] int? page, [FromQuery] int? pagesize)
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        if(page.HasValue)
        {
            return Ok(await _servico.ListarHistoricoPorUsuarioPaginado
                (usuarioId,
                page.Value,
                pagesize ?? 10
                ));
        }
        return Ok(await _servico.ListarHistoricoPorUsuario(usuarioId));
    }

    [Authorize(Roles = "Cliente")]
    [HttpPost]
    public async Task<IActionResult> Cadastrar([FromBody] CriarAgendamentoDTO dto)
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var novo = await _servico.Cadastrar(usuarioId, dto);

        return CreatedAtAction(nameof(Buscar), new { Id = novo.Id }, new
        {
            mensagem = "Agendamento criado com sucesso!",
            dados = novo
        });
    }

    [Authorize(Roles = "Barbeiro")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AgendamentoDTO dto)
    {
        await _servico.Atualizar(id, dto);
        return Ok(new { mensagem = "Agendamento atualizado com sucesso." });
    }

    [Authorize]
    [HttpPatch("status/{id}")]
    public async Task<IActionResult> AlterarStatus(int id, [FromBody] StatusDTO dto)
    {
        await _servico.AlterarStatus(id, dto.Status);
        return Ok(new { mensagem = "Status do agendamento atualizado com sucesso." });
    }

    [Authorize(Roles = "Cliente")]
    [HttpPatch("{id}/cancelar")]
    public async Task<IActionResult> Cancelar(int id)
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _servico.CancelarAgendamento(id, usuarioId);

        return Ok(new { mensagem = "Agendamento cancelado com sucesso." });
    }

    [Authorize(Policy = "AdminOuBarbeiro")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        await _servico.Excluir(id);
        return Ok(new { mensagem = "Agendamento excluído com sucesso." });
    }
}
