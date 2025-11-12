using Microsoft.AspNetCore.Mvc;
using BarbeariaPortifolio.API.Servicos.Interfaces;
using BarbeariaPortifolio.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace BarbeariaPortifolio.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AgendamentoController : ControllerBase
{
    private readonly IAgendamentoServico _servico;

    public AgendamentoController(IAgendamentoServico servico)
    {
        _servico = servico;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var agendamentos = await _servico.ListarTodos();
        return Ok(agendamentos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Buscar(int id)
    {
        var agendamento = await _servico.BuscarPorId(id);
        if (agendamento == null)
            return NotFound("Agendamento não encontrado.");
        return Ok(agendamento);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Cadastrar([FromBody] AgendamentoDTO dto)
    {
        var novo = await _servico.Cadastrar(dto);
        return CreatedAtAction(nameof(Buscar), new { id = novo.Id }, novo);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AgendamentoDTO dto)
    {
        var atualizado = await _servico.Atualizar(id, dto);
        return atualizado ? NoContent() : BadRequest("Erro ao atualizar agendamento.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var excluido = await _servico.Excluir(id);
        return excluido ? Ok("Agendamento removido com sucesso.") : NotFound("Agendamento não encontrado.");
    }
}
