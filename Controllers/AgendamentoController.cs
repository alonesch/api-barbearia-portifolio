using Microsoft.AspNetCore.Mvc;
using BarbeariaPortifolio.API.Servicos.Interfaces;
using BarbeariaPortifolio.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace BarbeariaPortifolio.API.Controllers;


[Route("api/[controller]")]
[ApiController]
public class AgendamentoController : ControllerBase
{
    private readonly IAgendamentoService _servico;

    public AgendamentoController(IAgendamentoService servico)
    {
        _servico = servico;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var agendamentos = await _servico.ListarTodos();
        return Ok(agendamentos);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> Buscar(int id)
    {
        var agendamento = await _servico.BuscarPorId(id);
        if (agendamento == null)
            return NotFound("Agendamento não encontrado.");
        return Ok(agendamento);
    }

    [Authorize]
    [HttpGet("barbeiro/{id}")]
    public async Task<IActionResult> ListarPorBarbeiro(int id)
    {
        var agendamentos = await _servico.ListarPorBarbeiro(id);

        if (agendamentos == null || !agendamentos.Any())
            return NotFound("Nenhum agendamento encontrado para este barbeiro.");

        return Ok(agendamentos);
    }


    [HttpPost]
    public async Task<IActionResult> Cadastrar([FromBody] AgendamentoDTO dto)
    {
        var novo = await _servico.Cadastrar(dto);
        return CreatedAtAction(nameof(Buscar), new { id = novo.Id }, novo);
    }
    

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AgendamentoDTO dto)
    {
        var atualizado = await _servico.Atualizar(id, dto);
        return atualizado ? NoContent() : BadRequest("Erro ao atualizar agendamento.");
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var excluido = await _servico.Excluir(id);
        return excluido ? Ok("Agendamento removido com sucesso.") : NotFound("Agendamento não encontrado.");
    }
}
